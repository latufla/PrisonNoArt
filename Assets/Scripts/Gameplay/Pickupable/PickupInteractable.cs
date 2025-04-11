using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.Equipments;
using Honeylab.Gameplay.Interactables;
using Honeylab.Gameplay.Interactables.World;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.Weapons;
using Honeylab.Sounds;
using Honeylab.Sounds.Data;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Vfx;
using UnityEngine;

namespace Honeylab.Gameplay.Pickup
{
    public class PickupInteractable : InteractableBase
    {
        [SerializeField] private ConfigIdProvider _configId;
        [SerializeField] private VfxId _collectVfxId;
        [SerializeField] private SoundId _collectSoundId;

        private SoundService _soundService;
        private ToastsService _toastsService;
        private ConsumablesService _consumablesService;
        private VfxService _vfxService;
        private PickupFlow _flow;
        private PickupConfig _config;
        private ConsumablesData _consumablesData;
        private EquipmentsService _equipmentsService;
        private EquipmentsData _equipmentsData;
        private WeaponsData _weaponsData;


        protected override void OnInit()
        {
            _flow = GetFlow<PickupFlow>();

            _soundService = _flow.Resolve<SoundService>();
            _toastsService = _flow.Resolve<ToastsService>();

            _consumablesService = _flow.Resolve<ConsumablesService>();
            _consumablesData = _flow.Resolve<ConsumablesData>();

            _vfxService = _flow.Resolve<VfxService>();

            IConfigsService configs = _flow.Resolve<IConfigsService>();
            _config = configs.Get<PickupConfig>(_configId.Id);

            _equipmentsService = _flow.Resolve<EquipmentsService>();
            _equipmentsData = _flow.Resolve<EquipmentsData>();
            _weaponsData = _flow.Resolve<WeaponsData>();
        }


        protected override UniTask OnInteractAsync(IInteractAgent agent, CancellationToken ct)
        {
            if (_flow.IsDeactivatePersistence.Value)
            {
                return UniTask.CompletedTask;
            }

            GiveReward(agent);

            PlayCollectSound();
            PlayCollectVfx(agent);

            _flow.IsDeactivatePersistence.Value = true;
            return UniTask.CompletedTask;
        }


        private void GiveReward(IInteractAgent agent)
        {
            foreach (RewardAmountConfig reward in _config.Rewards)
            {
                if (_consumablesService.TryGiveAmount(reward,
                        new TransactionSource(_flow.name, TransactionType.Pickup)))
                {
                    Vector3 toastPosition = _flow.transform.position + Vector3.up * 5.0f;
                    ConsumableData data = _consumablesData.GetData(reward.Name);
                    ConsumablePersistenceId rewardId = data.Id;
                    _toastsService.ShowConsumableToast(new ConsumableToastArgs(rewardId,
                        reward.Amount,
                        toastPosition));

                    continue;
                }

                if (_equipmentsService.TryEquipmentLevelChange(reward.Name, 0))
                {
                    Vector3 toastPosition = _flow.transform.position + Vector3.up * 5.0f;
                    EquipmentData data = _equipmentsData.GetData(reward.Name);
                    Sprite sprite = data.Levels.First().Sprite;
                    _toastsService.ShowIconToast(sprite, 1, toastPosition);
                }

                if (agent.WeaponAgent.TryAddWeapon(reward.Name))
                {
                    Vector3 toastPosition = _flow.transform.position + Vector3.up * 5.0f;
                    WeaponData data = _weaponsData.GetData(reward.Name);
                    Sprite sprite = data.Levels.First().Sprite;
                    if (sprite != null)
                    {
                        _toastsService.ShowIconToast(sprite, 1, toastPosition);
                    }
                }
            }
        }


        private void PlayCollectSound()
        {
            _soundService.RequestSoundPlay(_collectSoundId);
        }


        private void PlayCollectVfx(IInteractAgent agent)
        {
            _vfxService.PlayOnceAsync(_collectVfxId, agent.ConsumablesOutAnchor, agent.ConsumablesOutAnchor)
                .Forget();
        }
    }
}