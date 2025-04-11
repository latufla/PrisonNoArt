using UnityEngine;

namespace Honeylab.Utils.Arrows
{
    public class UiArrowView : ArrowView
    {
        [SerializeField] private Canvas _canvas;

        public Canvas Canvas => _canvas;


        public void ActiveSortingLayer(bool active = true)
        {
            _canvas.overrideSorting = active;
        }

        public override void HideImmediately()
        {
            ActiveSortingLayer(false);
            base.HideImmediately();
        }
    }
}
