using System;


namespace Honeylab.Gameplay.Ui
{
    [Flags]
    public enum BillboardAxis
    {
        None = 0,
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
        All = X | Y | Z
    }
}
