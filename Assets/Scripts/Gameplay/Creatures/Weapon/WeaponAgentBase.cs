using Honeylab.Gameplay.World;


namespace Honeylab.Gameplay.Weapons
{
    public abstract class WeaponAgentBase : WorldObjectComponentBase
    {
        public abstract WeaponFlow Weapon { get; }
    }
}
