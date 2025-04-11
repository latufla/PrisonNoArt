using Honeylab.Gameplay.Booster;
using Honeylab.Gameplay.Equipments;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.World;
using Honeylab.Pools;
using Honeylab.Utils.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Player
{
    public class PlayerSkinView : WorldObjectComponentBase
    {
        [SerializeField] private EquipmentSkinInfo[] _skinInfos;
        [SerializeField] private ParticleSystem _vfxDamageBoost;

        private EquipmentsPool _equipmentsPool;
        private EquipmentsService _equipmentsService;
        private BoosterService _boosterService;
        private CompositeDisposable _disposable;


        protected override void OnInit()
        {
            WorldObjectFlow flow = GetFlow();
            _equipmentsService = flow.Resolve<EquipmentsService>();
            GameplayPoolsService pools = flow.Resolve<GameplayPoolsService>();
            _equipmentsPool = pools.Get<EquipmentsPool>();
            _boosterService = flow.Resolve<BoosterService>();

            _disposable = new CompositeDisposable();

            IDisposable onEquip = _equipmentsService.OnEquipmentLevelChangeAsObservable()
                .Subscribe(SkinUpdate);
            _disposable.Add(onEquip);

            foreach (EquipmentSkinInfo info in _skinInfos)
            {
                SkinUpdate(info.Id);
            }

            IDisposable onDamageBoost = _boosterService.OnDamageBoostedAsObservable()
                .Subscribe(VfxDamageBoostActive);
            _disposable.Add(onDamageBoost);
        }


        private void SkinUpdate(EquipmentId id)
        {
            EquipmentSkinInfo info = _skinInfos.FirstOrDefault(it => it.Id.Equals(id));

            if (info == null)
            {
                return;
            }

            bool isActive = _equipmentsService.HasEquipment(info.Id) &&
                _equipmentsService.GetEquipmentLevel(info.Id).Value >= 0;

            foreach (GameObject go in info.DeactiveGameObjects)
            {
                go.SetActive(!isActive);
            }

            if (isActive)
            {
                if (info.EquipmentsObjects.Count > 0)
                {
                    return;
                }

                GameObject equipmentGo = _equipmentsPool.Pop(id);
                info.EquipmentsObjects.Add(equipmentGo);
                EquipmentSetParent(equipmentGo, info.Slot);

                if (info.MirrorSlot != null)
                {
                    equipmentGo = _equipmentsPool.Pop(id);
                    info.EquipmentsObjects.Add(equipmentGo);
                    EquipmentSetParent(equipmentGo, info.MirrorSlot, true);
                }
            }
        }


        private void VfxDamageBoostActive(bool active)
        {
            if (active)
            {
                _vfxDamageBoost.Play();
            }
            else
            {
                _vfxDamageBoost.Stop();
            }
        }


        private void EquipmentSetParent(GameObject equipment, Transform parent, bool inverted = false)
        {
            equipment.transform.SetParent(parent);
            equipment.transform.localPosition = Vector3.zero;
            equipment.transform.localRotation = Quaternion.identity;
            equipment.transform.localScale = new Vector3(inverted ? -1 : 1, 1, 1);
        }


        protected override void OnClear()
        {
            _disposable?.Clear();
            _disposable?.Dispose();
            _disposable = null;

            foreach (EquipmentSkinInfo info in _skinInfos)
            {
                foreach (GameObject go in info.EquipmentsObjects)
                {
                    _equipmentsPool.Push(info.Id, go);
                }

                info.EquipmentsObjects.Clear();
            }
        }


        [Serializable]
        private class EquipmentSkinInfo
        {
            [SerializeField] private EquipmentId _id;
            [SerializeField] private Transform _slot;
            [SerializeField] private Transform _mirrorSlot;
            [SerializeField] private List<GameObject> _deactiveGameObjects;

            public EquipmentId Id => _id;
            public Transform Slot => _slot;
            public Transform MirrorSlot => _mirrorSlot;
            public List<GameObject> DeactiveGameObjects => _deactiveGameObjects;
            public List<GameObject> EquipmentsObjects { get; set; } = new List<GameObject>();
        }
    }
}
