using AddonFusion.AddonValues;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Globalization;

namespace AddonFusion
{
    internal class ConfigManager
    {
        // GLOBAL
        public static ConfigEntry<bool> isEphemeralEnabled;
        public static ConfigEntry<int> minEphemeralItem;
        public static ConfigEntry<int> maxEphemeralItem;
        public static ConfigEntry<int> spawnAddonPerItem;
        // CAPSULE HOI-POI
        public static ConfigEntry<bool> isCapsuleEnabled;
        public static ConfigEntry<bool> isCapsuleSpawnable;
        public static ConfigEntry<int> capsuleRarity;
        public static ConfigEntry<bool> isCapsulePurchasable;
        public static ConfigEntry<int> capsulePrice;
        public static ConfigEntry<bool> isCapsuleCharge;
        public static ConfigEntry<string> capsuleItemValues;
        // SALT TANK
        public static ConfigEntry<bool> isSaltTankEnabled;
        public static ConfigEntry<bool> isSaltTankSpawnable;
        public static ConfigEntry<int> saltTankRarity;
        public static ConfigEntry<bool> isSaltTankPurchasable;
        public static ConfigEntry<int> saltTankPrice;
        public static ConfigEntry<string> saltTankEntityValues;
        // PROTECTIVE CORD
        public static ConfigEntry<bool> isCordEnabled;
        public static ConfigEntry<bool> isCordSpawnable;
        public static ConfigEntry<int> cordRarity;
        public static ConfigEntry<bool> isCordPurchasable;
        public static ConfigEntry<int> cordPrice;
        public static ConfigEntry<float> cordWindowDuration;
        public static ConfigEntry<float> cordSpamCooldown;
        public static ConfigEntry<string> cordEntityValues;
        // FLASHLIGHT LENS
        public static ConfigEntry<bool> isLensEnabled;
        public static ConfigEntry<bool> isLensSpawnable;
        public static ConfigEntry<int> lensRarity;
        public static ConfigEntry<bool> isLensPurchasable;
        public static ConfigEntry<int> lensPrice;
        public static ConfigEntry<string> lensEntityValues;
        public static ConfigEntry<string> lensExclusions;
        // BLADE SHARPENER
        public static ConfigEntry<bool> isSharpenerEnabled;
        public static ConfigEntry<bool> isSharpenerSpawnable;
        public static ConfigEntry<int> sharpenerRarity;
        public static ConfigEntry<bool> isSharpenerPurchasable;
        public static ConfigEntry<int> sharpenerPrice;
        public static ConfigEntry<string> sharpenerEntityValues;
        // SENZU
        public static ConfigEntry<bool> isSenzuEnabled;
        public static ConfigEntry<bool> isSenzuSpawnable;
        public static ConfigEntry<int> senzuRarity;
        public static ConfigEntry<bool> isSenzuPurchasable;
        public static ConfigEntry<int> senzuPrice;
        public static ConfigEntry<float> senzuReviveDuration;
        public static ConfigEntry<float> senzuHealthRegenDuration;
        public static ConfigEntry<float> senzuStaminaRegenDuration;
        // PYRETHRIN TANK
        public static ConfigEntry<bool> isPyrethrinTankEnabled;
        public static ConfigEntry<bool> isPyrethrinTankSpawnable;
        public static ConfigEntry<int> pyrethrinTankRarity;
        public static ConfigEntry<bool> isPyrethrinTankPurchasable;
        public static ConfigEntry<int> pyrethrinTankPrice;
        public static ConfigEntry<string> pyrethrinTankEntityValues;
        // REPAIR MODULE
        public static ConfigEntry<bool> isRepairModuleEnabled;
        public static ConfigEntry<bool> isRepairModuleSpawnable;
        public static ConfigEntry<int> repairModuleRarity;
        public static ConfigEntry<bool> isRepairModulePurchasable;
        public static ConfigEntry<int> repairModulePrice;
        public static ConfigEntry<int> repairModuleDuration;
        public static ConfigEntry<int> repairModuleProfit;
        // EPHEMERAL FLASHLIGHT
        public static ConfigEntry<bool> isEphemeralFlashEnabled;
        public static ConfigEntry<int> ephemeralFlashRarity;
        public static ConfigEntry<int> ephemeralFlashAddonRarity;
        public static ConfigEntry<int> ephemeralFlashMinUse;
        public static ConfigEntry<int> ephemeralFlashMaxUse;
        // EPHEMERAL SHOVEL
        public static ConfigEntry<bool> isEphemeralShovelEnabled;
        public static ConfigEntry<int> ephemeralShovelRarity;
        public static ConfigEntry<int> ephemeralShovelAddonRarity;
        public static ConfigEntry<int> ephemeralShovelMinUse;
        public static ConfigEntry<int> ephemeralShovelMaxUse;
        // EPHEMERAL SPRAY PAINT
        public static ConfigEntry<bool> isEphemeralSprayPaintEnabled;
        public static ConfigEntry<int> ephemeralSprayPaintRarity;
        public static ConfigEntry<int> ephemeralSprayPaintAddonRarity;
        public static ConfigEntry<int> ephemeralSprayPaintMinUse;
        public static ConfigEntry<int> ephemeralSprayPaintMaxUse;
        // EPHEMERAL KNIFE
        public static ConfigEntry<bool> isEphemeralKnifeEnabled;
        public static ConfigEntry<int> ephemeralKnifeRarity;
        public static ConfigEntry<int> ephemeralKnifeAddonRarity;
        public static ConfigEntry<int> ephemeralKnifeMinUse;
        public static ConfigEntry<int> ephemeralKnifeMaxUse;
        // EPHEMERAL WEED KILLER
        public static ConfigEntry<bool> isEphemeralWeedKillerEnabled;
        public static ConfigEntry<int> ephemeralWeedKillerRarity;
        public static ConfigEntry<int> ephemeralWeedKillerAddonRarity;
        public static ConfigEntry<int> ephemeralWeedKillerMinUse;
        public static ConfigEntry<int> ephemeralWeedKillerMaxUse;
        // EPHEMERAL TZP-INHALANT
        public static ConfigEntry<bool> isEphemeralTZPEnabled;
        public static ConfigEntry<int> ephemeralTZPRarity;
        public static ConfigEntry<int> ephemeralTZPAddonRarity;
        public static ConfigEntry<int> ephemeralTZPMinUse;
        public static ConfigEntry<int> ephemeralTZPMaxUse;

        internal static void Load()
        {
            // GLOBAL
            isEphemeralEnabled = AddonFusion.configFile.Bind<bool>("_Global_", "Enable Ephemeral Items", true, "Is the ephemeral items enabled?");
            minEphemeralItem = AddonFusion.configFile.Bind<int>("_Global_", "Min Ephemeral Item", 40, "Minimum number of ephemeral items that will spawn in the dungeon.");
            maxEphemeralItem = AddonFusion.configFile.Bind<int>("_Global_", "Max Ephemeral Item", 80, "Maximum number of ephemeral items that will spawn in the dungeon.");
            spawnAddonPerItem = AddonFusion.configFile.Bind<int>("_Global_", "Spawn Addon per Item", 1, "If this option is enabled (set to -1 to disable), it limits the number of spawnable addons based on the associated item + configured value." +
                "\nExample: If there is one flashlight without an addon, then one Flashlight Lens + 1 (default value) can spawn; if there are two flashlights, two Flashlight Lenses + 1 can spawn; if there are no flashlights, the number of Flashlight Lenses will equal the configured value.");
            // CAPSULE HOI-POI
            isCapsuleEnabled = AddonFusion.configFile.Bind<bool>("Capsule Hoi-Poi", "Enable", true, "Is the capsule Hoi-Poi enabled?");
            isCapsuleSpawnable = AddonFusion.configFile.Bind<bool>("Capsule Hoi-Poi", "Spawnable", false, "Is the capsule Hoi-Poi spawnable?");
            capsuleRarity = AddonFusion.configFile.Bind<int>("Capsule Hoi-Poi", "Rarity", 5, "Capsule Hoi-Poi spawn rarity");
            isCapsulePurchasable = AddonFusion.configFile.Bind<bool>("Capsule Hoi-Poi", "Purchasable", true, "Is the capsule Hoi-Poi purchasable?");
            capsulePrice = AddonFusion.configFile.Bind<int>("Capsule Hoi-Poi", "Price", 30, "Capsule Hoi-Poi price");
            isCapsuleCharge = AddonFusion.configFile.Bind<bool>("Capsule Hoi-Poi", "Charge", true, "Can the capsule Hoi-Poi charge items?");
            capsuleItemValues = AddonFusion.configFile.Bind<string>("Capsule Hoi-Poi", "Values", "default:600", "Values per item, the format is ItemName:ChargeDuration." +
                "\nChargeDuration: Time required to charge the item from empty to full capacity when it's in the capsule, in seconds.");
            // SALT TANK
            isSaltTankEnabled = AddonFusion.configFile.Bind<bool>("Salt Tank", "Enable", true, "Is the salt tank enabled?");
            isSaltTankSpawnable = AddonFusion.configFile.Bind<bool>("Salt Tank", "Spawnable", true, "Is the salt tank spawnable?");
            saltTankRarity = AddonFusion.configFile.Bind<int>("Salt Tank", "Rarity", 5, "Salt tank spawn rarity");
            isSaltTankPurchasable = AddonFusion.configFile.Bind<bool>("Salt Tank", "Purchasable", false, "Is the salt tank purchasable?");
            saltTankPrice = AddonFusion.configFile.Bind<int>("Salt Tank", "Price", 30, "Salt tank price");
            saltTankEntityValues = AddonFusion.configFile.Bind<string>("Salt Tank", "Values", "default:20:10", "Values per entity, the format is EntityName:BaseChance:AdditionalChance." +
                "\nBaseChance: Base chance for the entity to stop chasing when hit by a salted graffiti." +
                "\nAdditionalChance: Additional chance for the entity to stop chasing when hit by a salted graffiti.");
            // PROTECTIVE CORD
            isCordEnabled = AddonFusion.configFile.Bind<bool>("Protective Cord", "Enable", true, "Is the protective cord enabled?");
            isCordSpawnable = AddonFusion.configFile.Bind<bool>("Protective Cord", "Spawnable", true, "Is the protective cord spawnable?");
            cordRarity = AddonFusion.configFile.Bind<int>("Protective Cord", "Rarity", 100, "Protective cord spawn rarity");
            isCordPurchasable = AddonFusion.configFile.Bind<bool>("Protective Cord", "Purchasable", false, "Is the protective cord purchasable?");
            cordPrice = AddonFusion.configFile.Bind<int>("Protective Cord", "Price", 30, "Protective cord price");
            cordWindowDuration = AddonFusion.configFile.Bind<float>("Protective Cord", "Window duration", 0.35f, "Parrying window duration");
            cordSpamCooldown = AddonFusion.configFile.Bind<float>("Protective Cord", "Spam cooldown", 1f, "Cooldown duration per use");
            cordEntityValues = AddonFusion.configFile.Bind<string>("Protective Cord", "Values", "default:10:1.5:5:100:10", "Values per entity, the format is EntityName:CooldownDuration:StunDuration:SpeedBoostDuration:SpeedMultiplier:StaminaRegen." +
                "\nSpeedMultiplier: Speed multiplier percentage." +
                "\nStaminaRegen: Stamina regen percentage.");
            // FLASHLIGHT LENS
            isLensEnabled = AddonFusion.configFile.Bind<bool>("Flashlight Lens", "Enable", true, "Is the flashlight lens enabled?");
            isLensSpawnable = AddonFusion.configFile.Bind<bool>("Flashlight Lens", "Spawnable", true, "Is the flashlight lens spawnable?");
            lensRarity = AddonFusion.configFile.Bind<int>("Flashlight Lens", "Rarity", 100, "Flashlight lens spawn rarity");
            isLensPurchasable = AddonFusion.configFile.Bind<bool>("Flashlight Lens", "Purchasable", false, "Is the flashlight lens purchasable?");
            lensPrice = AddonFusion.configFile.Bind<int>("Flashlight Lens", "Price", 30, "Flashlight lens price");
            lensEntityValues = AddonFusion.configFile.Bind<string>("Flashlight Lens", "Values", "default:2:3:15:120:10:10:0,Player:2:0:10:60:10:0:0,ForestGiant:2:4:30:120:20:20:0", "Values per entity, the format is EntityName:FlashDuration:StunDuration:LightAngle:EntityAngle:EntityDistance:ImmunityDuration:BatteryConsumption." +
                "\nFlashDuration: Time required to blind an enemy." +
                "\nLightAngle: Angle of the light in relation to the enemy's eyes - increasing this value makes aiming easier." +
                "\nEntityAngle: Angle of the enemy in relation to the flashlight - increasing this value makes blinding from an angle easier.");
            lensExclusions = AddonFusion.configFile.Bind<string>("Flashlight Lens", "Exclusion list", "MouthDog", "List of creatures that will not be affected by the stun.");
            // BLADE SHARPENER
            isSharpenerEnabled = AddonFusion.configFile.Bind<bool>("Blade Sharpener", "Enable", true, "Is the blade sharpener enabled?");
            isSharpenerSpawnable = AddonFusion.configFile.Bind<bool>("Blade Sharpener", "Spawnable", true, "Is the blade sharpener spawnable?");
            sharpenerRarity = AddonFusion.configFile.Bind<int>("Blade Sharpener", "Rarity", 5, "Blade sharpener spawn rarity");
            isSharpenerPurchasable = AddonFusion.configFile.Bind<bool>("Blade Sharpener", "Purchasable", false, "Is the blade sharpener purchasable?");
            sharpenerPrice = AddonFusion.configFile.Bind<int>("Blade Sharpener", "Price", 30, "Blade sharpener price");
            sharpenerEntityValues = AddonFusion.configFile.Bind<string>("Blade Sharpener", "Values", "default:15:10", "Values per entity, the format is EntityName:CriticalSuccessChance:CriticalFailChance." +
                "\nCriticalSuccessChance: Chance for the player to score a critical hit and kill the enemy in one strike." +
                "\nCriticalFailChance: Chance for the player to perform a critical failure, causing the player's weapon to drop on the ground without dealing damage.");
            // SENZU
            isSenzuEnabled = AddonFusion.configFile.Bind<bool>("Senzu", "Enable", true, "Is the senzu enabled?");
            isSenzuSpawnable = AddonFusion.configFile.Bind<bool>("Senzu", "Spawnable", false, "Is the senzu spawnable?");
            senzuRarity = AddonFusion.configFile.Bind<int>("Senzu", "Rarity", 5, "Senzu spawn rarity");
            isSenzuPurchasable = AddonFusion.configFile.Bind<bool>("Senzu", "Purchasable", true, "Is the senzu purchasable?");
            senzuPrice = AddonFusion.configFile.Bind<int>("Senzu", "Price", 30, "Senzu price");
            senzuReviveDuration = AddonFusion.configFile.Bind<float>("Senzu", "Revive Duration", 30f, "Duration during which a player can be revived.");
            senzuHealthRegenDuration = AddonFusion.configFile.Bind<float>("Senzu", "Health Regen Duration", 25f, "Time required to regen the health from 0 to max, in seconds.");
            senzuStaminaRegenDuration = AddonFusion.configFile.Bind<float>("Senzu", "Stamina Regen Duration", 35f, "Time required to regen the stamina from 0 to max, in seconds.");
            // PYRETHRIN TANK
            isPyrethrinTankEnabled = AddonFusion.configFile.Bind<bool>("Pyrethrin Tank", "Enable", true, "Is the pyrethrin tank enabled?");
            isPyrethrinTankSpawnable = AddonFusion.configFile.Bind<bool>("Pyrethrin Tank", "Spawnable", true, "Is the pyrethrin tank spawnable?");
            pyrethrinTankRarity = AddonFusion.configFile.Bind<int>("Pyrethrin Tank", "Rarity", 5, "Pyrethrin tank spawn rarity");
            isPyrethrinTankPurchasable = AddonFusion.configFile.Bind<bool>("Pyrethrin Tank", "Purchasable", false, "Is the pyrethrin tank purchasable?");
            pyrethrinTankPrice = AddonFusion.configFile.Bind<int>("Pyrethrin Tank", "Price", 30, "Pyrethrin tank price");
            pyrethrinTankEntityValues = AddonFusion.configFile.Bind<string>("Pyrethrin Tank", "Values", "Red Locust Bees:10,Butler Bees:10,Bunker Spider:5,Hoarding bug:5,Centipede:5", "Values per entity, the format is EntityName:FleeDuration." +
                "\nFleeDuration: Duration for which the entity runs away from the player.");
            // REPAIR MODULE
            isRepairModuleEnabled = AddonFusion.configFile.Bind<bool>("Repair Module", "Enable", true, "Is the repair module enabled?");
            isRepairModuleSpawnable = AddonFusion.configFile.Bind<bool>("Repair Module", "Spawnable", true, "Is the repair module spawnable?");
            repairModuleRarity = AddonFusion.configFile.Bind<int>("Repair Module", "Rarity", 5, "Repair module spawn rarity");
            isRepairModulePurchasable = AddonFusion.configFile.Bind<bool>("Repair Module", "Purchasable", false, "Is the repair module purchasable?");
            repairModulePrice = AddonFusion.configFile.Bind<int>("Repair Module", "Price", 30, "Repair module price");
            repairModuleDuration = AddonFusion.configFile.Bind<int>("Repair Module", "Repair Duration", 600, "Time required to repair the item and add its profit value.");
            repairModuleProfit = AddonFusion.configFile.Bind<int>("Repair Module", "Profit", 50, "Profit percentage relative to the item's initial value.");
            // EPHEMERAL FLASHLIGHT
            isEphemeralFlashEnabled = AddonFusion.configFile.Bind<bool>("Ephemeral Flashlight", "Enable", true, "Is the ephemeral flashlight enabled?");
            ephemeralFlashRarity = AddonFusion.configFile.Bind<int>("Ephemeral Flashlight", "Rarity", 100, "Ephemeral flashlight spawn rarity");
            ephemeralFlashAddonRarity = AddonFusion.configFile.Bind<int>("Ephemeral Flashlight", "Addon Rarity", 50, "Chance for the ephemeral flashlight to spawn with an addon.");
            ephemeralFlashMinUse = AddonFusion.configFile.Bind<int>("Ephemeral Flashlight", "Min Battery", 30, "Minimum battery before the ephemeral flashlight breaks (1 - 100).");
            ephemeralFlashMaxUse = AddonFusion.configFile.Bind<int>("Ephemeral Flashlight", "Max Battery", 60, "Maximum battery before the ephemeral flashlight breaks (1 - 100).");
            // EPHEMERAL SHOVEL
            isEphemeralShovelEnabled = AddonFusion.configFile.Bind<bool>("Ephemeral Shovel", "Enable", true, "Is the ephemeral shovel enabled?");
            ephemeralShovelRarity = AddonFusion.configFile.Bind<int>("Ephemeral Shovel", "Rarity", 100, "Ephemeral shovel spawn rarity");
            ephemeralShovelAddonRarity = AddonFusion.configFile.Bind<int>("Ephemeral Shovel", "Addon Rarity", 50, "Chance for the ephemeral shovel to spawn with an addon.");
            ephemeralShovelMinUse = AddonFusion.configFile.Bind<int>("Ephemeral Shovel", "Min Use", 6, "Minimum number of uses before the ephemeral shovel breaks.");
            ephemeralShovelMaxUse = AddonFusion.configFile.Bind<int>("Ephemeral Shovel", "Max Use", 10, "Maximum number of uses before the ephemeral shovel breaks.");
            // EPHEMERAL SPRAY PAINT
            isEphemeralSprayPaintEnabled = AddonFusion.configFile.Bind<bool>("Ephemeral Spray Paint", "Enable", true, "Is the ephemeral spray paint enabled?");
            ephemeralSprayPaintRarity = AddonFusion.configFile.Bind<int>("Ephemeral Spray Paint", "Rarity", 100, "Ephemeral spray paint spawn rarity");
            ephemeralSprayPaintAddonRarity = AddonFusion.configFile.Bind<int>("Ephemeral Spray Paint", "Addon Rarity", 50, "Chance for the ephemeral spray paint to spawn with an addon.");
            ephemeralSprayPaintMinUse = AddonFusion.configFile.Bind<int>("Ephemeral Spray Paint", "Min Tank", 30, "Minimum tank value before the ephemeral spray paint breaks (1 - 100).");
            ephemeralSprayPaintMaxUse = AddonFusion.configFile.Bind<int>("Ephemeral Spray Paint", "Max Tank", 60, "Maximum tank value before the ephemeral spray paint breaks (1 - 100).");
            // EPHEMERAL KNIFE
            isEphemeralKnifeEnabled = AddonFusion.configFile.Bind<bool>("Ephemeral Knife", "Enable", true, "Is the ephemeral knife enabled?");
            ephemeralKnifeRarity = AddonFusion.configFile.Bind<int>("Ephemeral Knife", "Rarity", 100, "Ephemeral knife spawn rarity");
            ephemeralKnifeAddonRarity = AddonFusion.configFile.Bind<int>("Ephemeral Knife", "Addon Rarity", 50, "Chance for the ephemeral knife to spawn with an addon.");
            ephemeralKnifeMinUse = AddonFusion.configFile.Bind<int>("Ephemeral Shovel", "Min Use", 3, "Minimum number of uses before the ephemeral knife breaks.");
            ephemeralKnifeMaxUse = AddonFusion.configFile.Bind<int>("Ephemeral Shovel", "Max Use", 6, "Maximum number of uses before the ephemeral knife breaks.");
            // EPHEMERAL WEED KILLER
            isEphemeralWeedKillerEnabled = AddonFusion.configFile.Bind<bool>("Ephemeral Weed Killer", "Enable", true, "Is the ephemeral weed killer enabled?");
            ephemeralWeedKillerRarity = AddonFusion.configFile.Bind<int>("Ephemeral Weed Killer", "Rarity", 100, "Ephemeral weed killer spawn rarity");
            ephemeralWeedKillerAddonRarity = AddonFusion.configFile.Bind<int>("Ephemeral Weed Killer", "Addon Rarity", 50, "Chance for the ephemeral weed killer to spawn with an addon.");
            ephemeralWeedKillerMinUse = AddonFusion.configFile.Bind<int>("Ephemeral Weed Killer", "Min Fuel", 30, "Minimum fuel before the ephemeral weed killer breaks (1 - 100).");
            ephemeralWeedKillerMaxUse = AddonFusion.configFile.Bind<int>("Ephemeral Weed Killer", "Max Fuel", 60, "Maximum fuel before the ephemeral weed killer breaks (1 - 100).");
            // EPHEMERAL TZP-INHALANT
            isEphemeralTZPEnabled = AddonFusion.configFile.Bind<bool>("Ephemeral TZP-Inhalant", "Enable", true, "Is the ephemeral TZP-Inhalant enabled?");
            ephemeralTZPRarity = AddonFusion.configFile.Bind<int>("Ephemeral TZP-Inhalant", "Rarity", 100, "Ephemeral TZP-Inhalant spawn rarity");
            ephemeralTZPAddonRarity = AddonFusion.configFile.Bind<int>("Ephemeral TZP-Inhalant", "Addon Rarity", 50, "Chance for the ephemeral TZP-Inhalant to spawn with an addon.");
            ephemeralTZPMinUse = AddonFusion.configFile.Bind<int>("Ephemeral TZP-Inhalant", "Min Fuel", 30, "Minimum fuel before the ephemeral TZP-Inhalant breaks (1 - 100).");
            ephemeralTZPMaxUse = AddonFusion.configFile.Bind<int>("Ephemeral TZP-Inhalant", "Max Fuel", 60, "Maximum fuel before the ephemeral TZP-Inhalant breaks (1 - 100).");
        }

        internal static List<CapsuleHoiPoiValue> GetCapsuleValuesFromConfig()
        {
            List<CapsuleHoiPoiValue> capsuleValues = new List<CapsuleHoiPoiValue>();
            string[] items = capsuleItemValues.Value.Split(',');
            foreach (string itemValue in items)
            {
                string[] values = itemValue.Split(':');
                if (values.Length == 2)
                {
                    capsuleValues.Add(new CapsuleHoiPoiValue(values[0],
                        int.Parse(values[1])));
                }
            }
            return capsuleValues;
        }

        internal static List<SaltTankValue> GetSaltTankValuesFromConfig()
        {
            List<SaltTankValue> saltTankValues = new List<SaltTankValue>();
            string[] entities = saltTankEntityValues.Value.Split(',');
            foreach (string entityValue in entities)
            {
                string[] values = entityValue.Split(':');
                if (values.Length == 3)
                {
                    saltTankValues.Add(new SaltTankValue(values[0],
                        int.Parse(values[1]),
                        int.Parse(values[2])));
                }
            }
            return saltTankValues;
        }

        internal static List<ProtectiveCordValue> GetProtectiveCordValuesFromConfig()
        {
            List<ProtectiveCordValue> protectiveCordValues = new List<ProtectiveCordValue>();
            string[] entities = cordEntityValues.Value.Split(',');
            foreach (string entityValue in entities)
            {
                string[] values = entityValue.Split(':');
                if (values.Length == 6)
                {
                    protectiveCordValues.Add(new ProtectiveCordValue(values[0],
                        float.Parse(values[1], CultureInfo.InvariantCulture),
                        float.Parse(values[2], CultureInfo.InvariantCulture),
                        float.Parse(values[3], CultureInfo.InvariantCulture),
                        int.Parse(values[4]),
                        int.Parse(values[5])));
                }
            }
            return protectiveCordValues;
        }

        internal static List<LensValue> GetLensValuesFromConfig()
        {
            List<LensValue> lensValues = new List<LensValue>();
            string[] entities = lensEntityValues.Value.Split(',');
            foreach (string entityValue in entities)
            {
                string[] values = entityValue.Split(':');
                if (values.Length == 8)
                {
                    lensValues.Add(new LensValue(values[0],
                        float.Parse(values[1], CultureInfo.InvariantCulture),
                        float.Parse(values[2], CultureInfo.InvariantCulture),
                        float.Parse(values[3], CultureInfo.InvariantCulture),
                        float.Parse(values[4], CultureInfo.InvariantCulture),
                        int.Parse(values[5]),
                        float.Parse(values[6], CultureInfo.InvariantCulture),
                        float.Parse(values[7], CultureInfo.InvariantCulture)));
                }
            }
            return lensValues;
        }

        internal static List<BladeSharpenerValue> GetBladeSharpenerValuesFromConfig()
        {
            List<BladeSharpenerValue> bladeSharpenerValues = new List<BladeSharpenerValue>();
            string[] entities = sharpenerEntityValues.Value.Split(',');
            foreach (string entityValue in entities)
            {
                string[] values = entityValue.Split(':');
                if (values.Length == 3)
                {
                    bladeSharpenerValues.Add(new BladeSharpenerValue(values[0],
                        int.Parse(values[1]),
                        int.Parse(values[2])));
                }
            }
            return bladeSharpenerValues;
        }

        internal static List<PyrethrinTankValue> GetPyrethrinTankValuesFromConfig()
        {
            List<PyrethrinTankValue> pyrethrinTankValues = new List<PyrethrinTankValue>();
            string[] entities = pyrethrinTankEntityValues.Value.Split(',');
            foreach (string entityValue in entities)
            {
                string[] values = entityValue.Split(':');
                if (values.Length == 2)
                {
                    pyrethrinTankValues.Add(new PyrethrinTankValue(values[0],
                        float.Parse(values[1])));
                }
            }
            return pyrethrinTankValues;
        }
    }
}
