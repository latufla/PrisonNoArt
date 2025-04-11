using Honeylab.Gameplay.Equipments;
using System;
using Zenject;


namespace Honeylab.Project.Cheats
{
    public class EquipmentsCheat : IInitializable, IDisposable
    {
        private readonly EquipmentsData _data;
        private readonly EquipmentsService _equipmentsService;

        private SROptions _srOptions;

        public EquipmentsCheat(EquipmentsData data, EquipmentsService equipmentsService)
        {
            _data = data;
            _equipmentsService = equipmentsService;
        }

        public void Initialize()
        {
            _srOptions = SROptions.Current;
            _srOptions.EquipmentsChangeRequested += Change;
        }


        public void Dispose()
        {
            _srOptions.EquipmentsChangeRequested -= Change;
        }

        private void Change()
        {
            EquipmentData data = _data.GetData(_srOptions.EquipmentName);
            _equipmentsService.TryEquipmentLevelChange(data.Id, _srOptions.EquipmentUpgradeLevel - 1);
        }
    }
}
