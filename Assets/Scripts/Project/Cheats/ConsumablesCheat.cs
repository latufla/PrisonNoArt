using Honeylab.Consumables;
using Honeylab.Gameplay.Ui;
using Honeylab.Persistence;
using System;
using Zenject;


namespace Honeylab.Project.Cheats
{
    public class ConsumablesCheat : IInitializable, IDisposable
    {
        private readonly ConsumablesData _data;
        private readonly ConsumablesService _consumables;
        private readonly GameplayScreen _gameplayScreen;

        private SROptions _srOptions;


        public ConsumablesCheat(ConsumablesData data, ConsumablesService consumables, GameplayScreen gameplayScreen)
        {
            _data = data;
            _consumables = consumables;
            _gameplayScreen = gameplayScreen;
        }


        public void Initialize()
        {
            _srOptions = SROptions.Current;
            _srOptions.ConsumablesAddRequested += Add;
            _srOptions.ConsumablesResetAllRequested += Reset_All;
        }


        public void Dispose()
        {
            _srOptions.ConsumablesAddRequested -= Add;
            _srOptions.ConsumablesResetAllRequested -= Reset_All;
        }


        private void Add()
        {
            ConsumableData data = _data.GetData(_srOptions.ConsumableName);
            _consumables.ChangeAmount(data.Id, _srOptions.ConsumableAmount, null);

            ConsumableCounterView view = _gameplayScreen.GetConsumableCounter(data.Id);
            view.ChangeAmount(_srOptions.ConsumableAmount, true);
        }


        private void Reset_All()
        {
            var data = _data.GetData();
            foreach (ConsumableData d in data)
            {
                if (d.ConsumableType == ConsumableType.Hard)
                {
                    ObservableValuePersistentComponent amount = _consumables.GetObservableAmount(d.Id);
                    _consumables.ChangeAmount(d.Id, -amount.Value, null);
                }
                else
                {
                    var amount = _consumables.GetAmountProp(d.Id);
                    _consumables.ChangeAmount(d.Id, -amount.Value, null);
                }
            }
        }
    }
}
