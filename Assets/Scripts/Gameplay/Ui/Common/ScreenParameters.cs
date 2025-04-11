namespace Honeylab.Gameplay.Ui
{
    public enum ScreenOpenType
    {
        Cheat,
        RequiredClick,
        Auto
    }

    public enum ScreenEntryPointType
    {
        MainUI,
        Top,
        Bot,
        Left,
        Right
    }

    public class ScreenParameters
    {
        public const string Close = "close";
        public const string Enter = "enter";
        public const string AutoClose = "autoclose";
        public const string AppClose = "appclose";
        public const string Hard = "hard";
        public const string Soft = "soft";
        public const string Daily = "daily";
        public const string Weekly = "weekly";
        public const string Claim = "claim";
        public const string ClaimChest = "claim_chest";
        public const string Chest = "chest";
        public const string Go = "go";
        public const string Shop = "shop";
        public const string Inventory = "inventory";
        public const string Equipment = "equipment";
        public const string Reward = "reward";
        public const string Upgrade = "upgrade";
        public const string Resources = "resources";
        public const string Cards = "cards";
        public const string Finds = "finds";
        public const string Weapon = "weapon";
        public const string WeaponClose = "weapon_close";
        public const string EquipmentClose = "equipment_close";
        public const string EquipmentUpgrade = "equipment_upgrade";
        public const string Leave = "leave";
        public const string MusicEnabled = "music_enabled";
        public const string MusicDisabled = "music_disabled";
        public const string SfxEnabled = "sound_enabled";
        public const string SfxDisabled = "sound_disabled";
        public const string VibrationEnabled = "vibration_enabled";
        public const string VibrationDisabled = "vibration_disabled";
    }

    public class ScreenName
    {
        public const string AdOffer = "ad_offer";
        public const string CombatPower = "combat_power";
        public const string CraftStatus = "craft_status";
        public const string Expeditions = "expeditions";
        public const string ExpeditionsRequirement = "expeditions_requirement";
        public const string ExtraConsumables = "extra_consumables";
        public const string ExpeditionsResult = "expeditions_result";
        public const string EquipmentUpgrade = "equipment_upgrade";
        public const string ExpeditionsRevive = "expedition_revive";
        public const string Finds = "finds";
        public const string WeaponInfo = "weapon_info";
        public const string PlayerInventory = "inventory";
        public const string Minimap = "minimap";
        public const string Quests = "quests";
        public const string Shop = "shop";
        public const string Pause = "pause";
        public const string SpeedUp = "speed_up";
        public const string GetScreen = "get";
        public const string WeaponUpgrade = "weapon_upgrade";
        public const string Health = "health";
    }

    public static class ScreenEntryPoint
    {
        private const string MainUI = "mainUI";
        private const string Top = "ui_top";
        private const string Bot = "ui_bot";
        private const string Left = "ui_left";
        private const string Right = "ui_right";

        public static string GetEntryPointName(ScreenEntryPointType type)
        {
            return type switch
            {
                ScreenEntryPointType.MainUI => MainUI,
                ScreenEntryPointType.Top => Top,
                ScreenEntryPointType.Bot => Bot,
                ScreenEntryPointType.Left => Left,
                ScreenEntryPointType.Right => Right,
                _ => throw new System.NotImplementedException()
            };
        }
    }
}
