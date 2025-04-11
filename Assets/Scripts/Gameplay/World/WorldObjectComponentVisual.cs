using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.World
{
    public abstract class WorldObjectComponentVisual : WorldObjectComponentBase
    {
        private Dictionary<Material, Color> _materialToInitialColor;


        protected override void OnClear()
        {
            _materialToInitialColor?.Clear();
            _materialToInitialColor = null;
        }


        public bool ChangeColorState(float inDuration,
            float outDuration,
            Ease easeEnum,
            CancellationToken ct)
        {
            var skinRenderers = GetSkinRenderers();
            if (skinRenderers == null || skinRenderers.Count == 0)
            {
                return false;
            }

            _materialToInitialColor ??= new Dictionary<Material, Color>();

            foreach (Renderer skinRenderer in skinRenderers)
            {
                if (skinRenderer == null)
                {
                    continue;
                }

                DOTween.Kill(skinRenderer.gameObject);

                const string emissionProperty = "_EmissionColor";
                if (!_materialToInitialColor.ContainsKey(skinRenderer.material))
                {
                    if (!skinRenderer.material.HasColor(emissionProperty))
                    {
                        continue;
                    }
                    Color initialColor = skinRenderer.material.GetColor(emissionProperty);
                    _materialToInitialColor.Add(skinRenderer.material, initialColor);
                }

                Color color = new(1.0f, 1.0f, 1.0f);

                DOTween.Sequence()
                    .SetLink(skinRenderer.gameObject)
                    .Append(skinRenderer.material.DOColor(color, emissionProperty, inDuration))
                    .Append(skinRenderer.material.DOColor(_materialToInitialColor[skinRenderer.material],
                        emissionProperty,
                        outDuration))
                    .SetEase(easeEnum)
                    .OnKill(() =>
                        skinRenderer.material.SetColor(emissionProperty,
                            _materialToInitialColor[skinRenderer.material]))
                    .ToUniTask(cancellationToken: ct)
                    .Forget();
            }

            return true;
        }


        protected abstract List<Renderer> GetSkinRenderers();
    }
}
