using System;
using System.ComponentModel;


public partial class SROptions
{
    private const string ConsumablesCategory = "Consumables";

    public event Action ConsumablesAddRequested;
    public event Action ConsumablesResetAllRequested;


    [Category(ConsumablesCategory)] [Sort(0)]
    public string ConsumableName { get; set; } = "Wood";

    [Category(ConsumablesCategory)] [Sort(1)]
    public int ConsumableAmount { get; set; } = 10;


    [Category(ConsumablesCategory)] [Sort(2)]
    public void Consumable_Add() => ConsumablesAddRequested?.Invoke();


    [Category(ConsumablesCategory)] [Sort(3)]
    public void Consumable_Reset_All() => ConsumablesResetAllRequested?.Invoke();
}
