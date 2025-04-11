using System;
using System.ComponentModel;


public partial class SROptions
{
    private const string LevelsCategory = "Levels";

    public event Action Level1Requested;
    public event Action Level2Requested;


    [Category(LevelsCategory)]
    public void Level_1() => Level1Requested?.Invoke();

    [Category(LevelsCategory)]
    public void Level_2() => Level2Requested?.Invoke();
}
