using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.World;
using Honeylab.Sounds.Data;
using Honeylab.Utils.Vfx;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings.View
{
    public abstract class UnlockBuildingViewBase : WorldObjectComponentVisual
    {
        [SerializeField] private Transform _unlockVfxAnchor;
        [SerializeField] private Renderer _skinRenderer;
        [SerializeField] private VfxId _unlockVfxId;
        [SerializeField] private SoundId _unlockSoundId;


        protected UnlockBuildingFlow Flow;


        public Renderer SkinRenderer => _skinRenderer;


        protected override void OnInit()
        {
            Flow = GetFlow<UnlockBuildingFlow>();
        }


        public abstract bool IsUnlockPopupShown();
        public abstract void ShowUnlockPopup();
        public abstract void HideUnlockPopup();
        public abstract void UpdateUnlockPopup(UnlockBuildingStates state);


        public void PlayUnlockVfx()
        {
            if (_unlockVfxAnchor == null)
            {
                return;
            }

            Flow.Vfxs.PlayOnceAsync(_unlockVfxId, _unlockVfxAnchor.position, Quaternion.identity)
                .Forget();
        }


        public void PlayUnlockSound()
        {
            Flow.Sounds.RequestSoundPlay(_unlockSoundId);
        }


        public void PlayUnlockVibration()
        {
            Flow.Vibrations.Vibrate();
        }
    }
}
