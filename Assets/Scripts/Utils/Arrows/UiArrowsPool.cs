using Honeylab.Utils.Pool;
using UnityEngine;


namespace Honeylab.Utils.Arrows
{
    public class UiArrowsPool : ArrowsPoolBase
    {
        public UiArrowsPool(IGameObjectPool gameObjectPool,
            Transform defaultParentWhenShowed,
            ArrowsService arrowsService) : base(gameObjectPool, defaultParentWhenShowed, arrowsService) { }
    }
}
