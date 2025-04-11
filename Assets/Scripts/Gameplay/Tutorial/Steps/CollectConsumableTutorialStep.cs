using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Upgrades;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    [Serializable]
    public class CollectOneTimeResource
    {
        [field: SerializeField] public WorldObjectId TargetWorldObjectId { get; private set; }

        [field: SerializeField] public bool OneTimeResourceActive { get; private set; }

        [field: SerializeField] public bool SetValueFromField { get; private set; }

        [field: SerializeField] public int FieldAmount { get; private set; }
    }

    public class CollectConsumableTutorialStep : TutorialStepBase
    {
        [SerializeField] private ConsumablePersistenceId _consumableId;
        [SerializeField] private WorldObjectId _meleeWeaponTypeId;
        [SerializeField] private WorldObjectId _unlockWorldId;
        [SerializeField] private TutorialInfo _info;
        [SerializeField] private bool _useExistingAmount;

        [SerializeField] private CollectOneTimeResource _collectOneTimeResource;

        public (bool exist, Vector3 position) Position { get; private set; }

        private CompositeDisposable _focusHighlightDisposables = new();


        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await CollectAsync(ct)
        };


        private async UniTask CollectAsync(CancellationToken ct)
        {
            var amountProp = GetConsumableAmountProp(_consumableId);
            _info.ConsumableId = _consumableId;
            int amountToCollect = _info.MaxAmountToCollect;
            int collectedAmount = 0;

            if (_collectOneTimeResource.TargetWorldObjectId != null)
            {
                WorldObjectFlow target =
                    await GetObjectAsync<WorldObjectFlow>(_collectOneTimeResource.TargetWorldObjectId, ct);
                Position = (true, target.transform.position);
            }
            else
            {
                Position = (false, Vector3.zero);
            }

            SendStepStartAnalytics(_info);
            if (CheckUnlock())
            {
                return;
            }

            if (_useExistingAmount)
            {
                if (amountProp.Value >= amountToCollect)
                {
                    //ShowScreen(_info);
                    //ScreenSetCollectedAmount(amountToCollect);
                    return;
                }

                collectedAmount = amountProp.Value;
            }

            PlayerFlow player = await GetObjectFirstAsync<PlayerFlow>(ct);

            ShowScreen(_info);
            ScreenSetCollectedAmount(collectedAmount);

            WorldObjectId weaponId = player.Get<WeaponAgent>().GetWeaponByTypeFirstOrDefault(_meleeWeaponTypeId);
            UpgradeFlow weaponUpgrade = GetObject<UpgradeFlow>(weaponId);
            UpdateFocusTarget(player, weaponUpgrade, ct);

            if (_info.FocusOnStart)
            {
                ConsumableData consumableData = GetConsumableData(_consumableId);
                string consumableName = consumableData.Name;
                WeaponAttackTarget focusTarget = GetWeaponAttackTarget(player, weaponUpgrade, consumableName);
                if (focusTarget != null)
                {
                    FocusTargetAsync(focusTarget.transform);
                }
            }

            while (collectedAmount < amountToCollect)
            {
                int amount = amountProp.Value;

                await UniTask.WaitUntil(() => amountProp.Value != amount,
                    cancellationToken: ct); // TODO: await property change

                int delta = amountProp.Value - amount;
                if (delta > 0)
                {
                    collectedAmount += delta;
                    collectedAmount = Mathf.Clamp(collectedAmount, 0, amountToCollect);

                    ScreenSetCollectedAmount(collectedAmount);

                    UpdateFocusTarget(player, weaponUpgrade, ct);
                }
            }

            _info.ConsumableId = null;
        }


        private void UpdateFocusTarget(PlayerFlow player, UpgradeFlow weaponUpgrade, CancellationToken ct)
        {
            if (weaponUpgrade == null)
            {
                return;
            }

            ConsumableData consumableData = GetConsumableData(_consumableId);
            string consumableName = consumableData.Name;
            WeaponAttackTarget focusTarget = GetWeaponAttackTarget(player, weaponUpgrade, consumableName);

            _focusHighlightDisposables?.Dispose();
            _focusHighlightDisposables = new CompositeDisposable();

            if (focusTarget != null)
            {
                IDisposable onClickHighlightIcon =
                    SetHighlightInfoIcon(focusTarget.GetFlow(), consumableData.Sprite, ct);
                _focusHighlightDisposables.Add(onClickHighlightIcon);
                SetFocusTarget(focusTarget.transform);
            }
        }


        private WeaponAttackTarget GetWeaponAttackTarget(PlayerFlow player,
            UpgradeFlow weaponUpgrade,
            string consumableName)
        {
            WeaponAttackTarget focusTarget = GetObjects<WorldObjectFlow>()
                .Select(it => it.Get<WeaponAttackTarget>())
                .Where(it =>
                    it != null && it.CanHit() && it.CanDamage(weaponUpgrade.UpgradeLevelPersistence.Value)
                    && it.GetReward(consumableName) > 0)
                .OrderBy(it => Vector3.Distance(player.transform.position, it.transform.position))
                .FirstOrDefault();

            return focusTarget;
        }


        public override TutorialInfo GetTutorialInfo() => _info;


        private bool CheckUnlock()
        {
            bool isUnlocked = false;
            if (_unlockWorldId != null)
            {
                var building = GetObject<UnlockBuildingFlow>(_unlockWorldId);
                if (building != null && building.State.Value == UnlockBuildingStates.Unlocked)
                {
                    isUnlocked = true;
                }
            }

            return isUnlocked;
        }


        protected override void OnSubStepComplete()
        {
            SendStepEndAnalytics();
            _focusHighlightDisposables?.Dispose();
        }


        public override void Clear()
        {
            _focusHighlightDisposables?.Dispose();
            _focusHighlightDisposables = null;
        }
    }
}