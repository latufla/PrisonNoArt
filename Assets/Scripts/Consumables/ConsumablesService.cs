using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils.Data;
using Honeylab.Utils.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


namespace Honeylab.Consumables
{
    public struct ConsumablesChangeAmount
    {
        public ConsumablePersistenceId Id;
        public int Amount;
        public TransactionSource Source;
    }


    public class ConsumablesService : IDisposable
    {
        private readonly SharedPersistenceService _sharedPersistenceService;
        private readonly LevelPersistenceService _levelPersistenceService;
        private readonly ConsumablesData _consumablesData;
        private bool _isInitialized;

        private readonly Subject<ConsumablesChangeAmount> _subjectChangeAmount = new();


        public IObservable<ConsumablesChangeAmount> OnConsumablesChangeAmountAsObservable() =>
            _subjectChangeAmount.AsObservable();


        public ConsumablesService(SharedPersistenceService sharedPersistenceService,
            LevelPersistenceService levelPersistenceService,
            ConsumablesData consumablesData)
        {
            _sharedPersistenceService = sharedPersistenceService;
            _levelPersistenceService = levelPersistenceService;
            _consumablesData = consumablesData;
        }


        public void Init()
        {
            var consumables = _consumablesData.GetData();
            var hard = consumables.Where(it => it.ConsumableType == ConsumableType.Hard);
            foreach (ConsumableData data in hard)
            {
                if (_sharedPersistenceService.TryGetComponent(data.Id, out ObservableValuePersistentComponent _))
                {
                    continue;
                }

                ObservableValuePersistentComponent component = _sharedPersistenceService.Create(data.Id)
                    .Add<ObservableValuePersistentComponent>();

                component.Value = data.InitialAmount;
            }

            var other = consumables.Where(it => it.ConsumableType != ConsumableType.Hard);
            foreach (ConsumableData data in other)
            {
                if (_levelPersistenceService.TryGetComponent(data.Id, out ConsumableAmountPersistentComponent _))
                {
                    continue;
                }

                ConsumableAmountPersistentComponent component = _levelPersistenceService.Create(data.Id)
                    .Add<ConsumableAmountPersistentComponent>();
                component.Value = data.InitialAmount;
            }

            _isInitialized = true;
        }


        public void Dispose() { }


        public IReadOnlyReactiveProperty<int> GetAmountProp(ConsumablePersistenceId id) => _levelPersistenceService
            .GetComponent<ConsumableAmountPersistentComponent>(id)
            .ValueProperty;


        public ObservableValuePersistentComponent GetObservableAmount(ConsumablePersistenceId id) =>
            _sharedPersistenceService
                .GetComponent<ObservableValuePersistentComponent>(id);


        public bool TryGetAmount(ConsumablePersistenceId id, out int amount)
        {
            if (!_isInitialized)
            {
                amount = default;
                return false;
            }

            ConsumableData data = _consumablesData.GetData(id);
            amount = data.ConsumableType == ConsumableType.Hard ?
                GetObservableAmount(id).Value :
                GetAmountProp(id).Value;

            return true;
        }


        public int ChangeAmount(ConsumablePersistenceId id, int changeDelta, TransactionSource source)
        {
            ConsumableData data = _consumablesData.GetData(id);
            int newValue;
            if (data.ConsumableType == ConsumableType.Hard)
            {
                ObservableValuePersistentComponent component =
                    _sharedPersistenceService.GetComponent<ObservableValuePersistentComponent>(id);

                newValue = component.Value + changeDelta;
                component.Value = newValue;
            }
            else
            {
                ConsumableAmountPersistentComponent component =
                    _levelPersistenceService.GetComponent<ConsumableAmountPersistentComponent>(id);
                newValue = component.Value + changeDelta;
                component.Value = newValue;
            }

            _subjectChangeAmount.OnNext(new ConsumablesChangeAmount
                { Id = id, Amount = changeDelta, Source = source });
            return newValue;
        }


        public int ChangeAmount(string name, int changeDelta, TransactionSource source)
        {
            ConsumableData data = _consumablesData.GetData(name);
            return ChangeAmount(data.Id, changeDelta, source);
        }


        private bool HasEnoughAmount(ConsumablePersistenceId id, int amount)
        {
            TryGetAmount(id, out int a);
            return a >= amount;
        }


        public bool HasEnoughAmount(string name, int amount)
        {
            ConsumableData data = _consumablesData.GetData(name);
            return HasEnoughAmount(data.Id, amount);
        }


        public bool HasEnoughAmount(List<RewardAmountConfig> configs) => configs.All(HasEnoughAmount);
        public bool HasEnoughAmount(RewardAmountConfig config) => HasEnoughAmount(config.Name, config.Amount);


        public bool Contains(string name) => _consumablesData.Contains(name);
        public bool Contains(ConsumablePersistenceId id) => _consumablesData.Contains(id);


        public void TryGiveAmount(List<RewardAmountConfig> rewards,
            TransactionSource source,
            float multiplier = 1.0f)
        {
            rewards?.ForEach(it => TryGiveAmount(it, source, multiplier));
        }


        public bool TryGiveAmount(RewardAmountConfig reward,
            TransactionSource source,
            float multiplier = 1.0f)
        {
            if (!Contains(reward.Name))
            {
                return false;
            }

            ConsumableData data = _consumablesData.GetData(reward.Name);
            int amount = Mathf.CeilToInt(reward.Amount * multiplier);
            ConsumablePersistenceId rewardId = data.Id;
            ChangeAmount(rewardId, amount, source);
            return true;
        }


        public bool TryGiveAmount(ConsumablePersistenceId id,
            int rewardAmount,
            TransactionSource source,
            float multiplier = 1.0f)
        {
            if (!Contains(id))
            {
                return false;
            }

            ConsumableData data = _consumablesData.GetData(id);
            int amount = Mathf.CeilToInt(rewardAmount * multiplier);
            ChangeAmount(data.Name, amount, source);
            return true;
        }


        public bool TryGiveAmount(string consumableName,
            int rewardAmount,
            TransactionSource source,
            float multiplier = 1.0f)
        {
            if (!Contains(consumableName))
            {
                return false;
            }

            ConsumableData data = _consumablesData.GetData(consumableName);
            int amount = Mathf.CeilToInt(rewardAmount * multiplier);
            ChangeAmount(data.Name, amount, source);
            return true;
        }


        public void WithdrawAmount(List<RewardAmountConfig> rewards, TransactionSource source, float multiplier = 1.0f)
        {
            rewards?.ForEach(it => WithdrawAmount(it, source, multiplier));
        }


        private void WithdrawAmount(RewardAmountConfig reward, TransactionSource source, float multiplier = 1.0f)
        {
            ConsumableData data = _consumablesData.GetData(reward.Name);
            int amount = Mathf.CeilToInt(reward.Amount * multiplier);
            ChangeAmount(data.Id, -amount, source);
        }


        public void WithdrawAmount(string rewardName,
            int rewardAmount,
            TransactionSource source,
            float multiplier = 1.0f)
        {
            ConsumableData data = _consumablesData.GetData(rewardName);
            int amount = Mathf.CeilToInt(rewardAmount * multiplier);
            ChangeAmount(data.Id, -amount, source);
        }
    }
}
