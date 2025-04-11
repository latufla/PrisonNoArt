using System;
using System.ComponentModel;


public partial class SROptions
{
    private const string ExpeditionsCategory = "Expeditions";

    public event Action ExpeditionLoadRequested;
    public event Action ExpeditionsScreenRequested;
    public event Action ExpeditionsBasementScreenRequested;
    public event Action ExpeditionsBossLoadRequested;

    [Category(ExpeditionsCategory)] [Sort(0)]
    public int ExpeditionIndex { get; set; } = 0;

    [Category(ExpeditionsCategory)] [Sort(1)]
    public void Expedition_Load() => ExpeditionLoadRequested?.Invoke();

    [Category(ExpeditionsCategory)] [Sort(2)]
    public void Expeditions_Screen() => ExpeditionsScreenRequested?.Invoke();

    [Category(ExpeditionsCategory)] [Sort(3)]
    public void Expeditions_Basement_Screen() => ExpeditionsBasementScreenRequested?.Invoke();

    [Category(ExpeditionsCategory)] [Sort(4)]
    public void Expeditions_Boss_Load() => ExpeditionsBossLoadRequested?.Invoke();
}
