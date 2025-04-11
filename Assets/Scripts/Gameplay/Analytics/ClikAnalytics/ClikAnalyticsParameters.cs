namespace Honeylab.Gameplay.Analytics
{
    public class ClikAnalyticsParameters
    {
        public const string MissionId = "missionID";
        public const string MissionRequirements = "missionRequirements";
        public const string UserAtk = "userAtk";
        public const string UserHp = "userHP";
        public const string WeaponLevel = "weaponLevel";
        public const string WeaponLevelRequirement = "weaponLevelRequired";
        public const string WorldName = "WorldName";
        public const string ExpeditionId = "expeditionID";
        public const string EnemyHealth = "enemyHealth";
        public const string EnemyWeaponLevel = "enemyWeaponLevel";
        public const string TransactionType = "transactionType";
        public const string ResourceType = "resourceType";
        public const string Amount = "amount";
        public const string Balance = "balance";
        public const string Source = "source";
        public const string StartedType = "startedType";
        public const string ExpeditionsPlayed = "expeditionsPlayed";
        public const string Chests = "chests";
        public const string Enemies = "enemies";
        public const string SpecialItem = "specialItem";
        public const string ButtonName = "buttonName";
        public const string Equipment = "equipment";
        public const string ItemLevel = "itemLevel";
        public const string PowerRequired = "powerRequired";
        public const string GasolineRequired = "gasolineRequired";
        public const string Day = "day";
        public const string TaskID = "taskID";
        public const string TaskName = "taskName";
        public const string GoldenStars = "goldenStars";
        public const string ProgressBar = "progressBar";
        public const string Week = "week";
        public const string ShopName = "shopName";
        public const string ProductName = "productName";
        public const string ProductID = "productID";
        public const string ProductType = "productType";
        public const string SaleOptionTimer = "sale_option_timer";
        public const string Area = "area";
        public const string Location = "location";
        public const string LocationDetailed = "locationDetailed";
        public const string PopupName = "popupName";
        public const string PopupResult = "popupResult";
        public const string PopupOpenType = "popupOpenType";
        public const string EntryPoint = "entryPoint";
        public const string Category = "category";
        public const string Daily = "daily";
        public const string Weekly = "weekly";
    }


    public class TransactionSource
    {
        public string Name;
        public string Type;

        public TransactionSource(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }


    public class TransactionName
    {
        public const string DailyRewards = "dailyRewards";
        public const string WeeklyRewards = "weeklyRewards";
        public const string DailyRewardsChest = "dailyRewardsFinal";
        public const string WeeklyRewardsChest = "weeklyRewardsFinal";
        public const string Skip = "Skip";
        public const string Get = "Get";
        public const string HardShop = "HardShop";
        public const string SoftShop = "SoftShop";
        public const string SpecialShop = "SpecialShop";
        public const string RewardedAd = "RewardedAd";
        public const string EnterExpedition = "EnterExpedition";
        public const string EquipmentUpgrade = "EquipmentUpgrade";
        public const string Tutorial = "Tutorial";
    }


    public class TransactionType
    {
        public const string IAP = "IAP";
        public const string RV = "RV";
        public const string DailyRewards = "DailyRewards";
        public const string WeeklyRewards = "WeeklyRewards";
        public const string Equipments = "Equipments";
        public const string Upgrade = "Upgrade";
        public const string Unlock = "Unlock";
        public const string Craft = "Craft";
        public const string Skip = "Skip";
        public const string Get = "Get";
        public const string Weapon = "Weapon";
        public const string Pickup = "Pickup";
        public const string Expedition = "Expedition";
    }
}
