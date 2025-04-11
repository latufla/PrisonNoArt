using Honeylab.Gameplay.World;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public abstract class CraftBuildingViewBase : WorldObjectComponentVisual
    {
        [SerializeField] private CraftBuildingAnimations _animations;
        [SerializeField] private Renderer _skinRenderer;
        public Renderer SkinRenderer => _skinRenderer;


        public abstract bool IsCraftPopupShown();
        public abstract void ShowCraftPopup();
        public abstract void HideCraftPopup();
        public abstract void UpdateCraftPopup(CraftBuildingStates state);


        public void PlayIdle()
        {
            if (_animations != null)
            {
                _animations.PlayIdle();
            }
        }


        public void PlayWork()
        {
            if (_animations != null)
            {
                _animations.PlayWork();
            }
        }
    }
}
