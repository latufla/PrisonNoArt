using Honeylab.Gameplay.World;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Weapons
{
    public class WeaponView : WorldObjectComponentBase
    {
        [SerializeField] private List<Transform> _skins;

        private Transform _initialParent;
        private WeaponFlow _flow;

        private CompositeDisposable _disposable;


        protected override void OnInit()
        {
            _initialParent = transform.parent;

            _flow = GetFlow<WeaponFlow>();

            _disposable = new CompositeDisposable();
            IDisposable upgrade = _flow.UpgradeLevel.Subscribe(UpdateSkin);
            _disposable.Add(upgrade);
        }


        protected override void OnClear()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        public void SetSlot(Transform slot)
        {
            transform.parent = slot != null ? slot : _initialParent;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }


        private void UpdateSkin(int upgradeLevel)
        {
            if (_skins.Count <= 0)
            {
                return;
            }
            _skins.ForEach(it => it.gameObject.SetActive(false));
            _skins[upgradeLevel].gameObject.SetActive(true);
        }
    }
}
