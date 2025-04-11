using System;
using System.ComponentModel;


public partial class SROptions
{
    private const string EquipmentsCategory = "Equipments";

    public event Action EquipmentsChangeRequested;


    [Category(EquipmentsCategory)] [Sort(0)]
    public string EquipmentName { get; set; } = "Backpack";

    [Category(EquipmentsCategory)] [Sort(1)]
    public int EquipmentUpgradeLevel { get; set; } = 1;

    [Category(EquipmentsCategory)] [Sort(2)]
    public void EquipmentsLevel_Change() => EquipmentsChangeRequested?.Invoke();
}
