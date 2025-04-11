using Honeylab.Gameplay.World;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public class CraftBuildingAnimations : WorldObjectAnimationsBase
    {
        private static readonly int Idle = Animator.StringToHash(nameof(Idle));
        private static readonly int Work = Animator.StringToHash(nameof(Work));


        public void PlayIdle()
        {
            ChangeState(Idle);
        }


        public void PlayWork()
        {
            ChangeState(Work);
        }
    }
}
