using AddonFusion.AddonValues;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Globalization;

namespace AddonFusion
{
    internal class ConfigManager
    {
        // CAPSULE HOI-POI
        public static ConfigEntry<bool> isCapsuleEnabled;
        public static ConfigEntry<bool> isCapsuleSpawnable;
        public static ConfigEntry<int> capsuleRarity;
        public static ConfigEntry<bool> isCapsulePurchasable;
        public static ConfigEntry<int> capsulePrice;
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
        // FLASHLIGHT LENS
        public static ConfigEntry<bool> isLensEnabled;
        public static ConfigEntry<bool> isLensSpawnable;
        public static ConfigEntry<int> lensRarity;
        public static ConfigEntry<bool> isLensPurchasable;
        public static ConfigEntry<int> lensPrice;
        public static ConfigEntry<string> lensEntityValues;
        public static ConfigEntry<string> lensExclusions;
        // PROTECTIVE CORD
        public static ConfigEntry<bool> isSharpenerEnabled;
        public static ConfigEntry<bool> isSharpenerSpawnable;
        public static ConfigEntry<int> sharpenerRarity;
        public static ConfigEntry<bool> isSharpenerPurchasable;
        public static ConfigEntry<int> sharpenerPrice;

        internal static void Load()
        {
            // CAPSULE HOI-POI
            isCapsuleEnabled = AddonFusion.configFile.Bind<bool>("Capsule Hoi-Poi", "Enable", true, "Is the capsule Hoi-Poi enabled?");
            isCapsuleSpawnable = AddonFusion.configFile.Bind<bool>("Capsule Hoi-Poi", "Spawnable", true, "Is the capsule Hoi-Poi spawnable?");
            capsuleRarity = AddonFusion.configFile.Bind<int>("Capsule Hoi-Poi", "Rarity", 1000, "Capsule Hoi-Poi spawn rarity");
            isCapsulePurchasable = AddonFusion.configFile.Bind<bool>("Capsule Hoi-Poi", "Purchasable", true, "Is the capsule Hoi-Poi purchasable?");
            capsulePrice = AddonFusion.configFile.Bind<int>("Capsule Hoi-Poi", "Price", 30, "Capsule Hoi-Poi price");
            capsuleItemValues = AddonFusion.configFile.Bind<string>("Capsule Hoi-Poi", "Values", "default:600", "Values per item, the format is ItemName:ChargeTime.\nChargeTime: Time to charge the item from empty to full capacity when it's in the capsule, in seconds.");
            // SALT TANK
            isSaltTankEnabled = AddonFusion.configFile.Bind<bool>("Salt Tank", "Enable", true, "Is the salt tank enabled?");
            isSaltTankSpawnable = AddonFusion.configFile.Bind<bool>("Salt Tank", "Spawnable", true, "Is the salt tank spawnable?");
            saltTankRarity = AddonFusion.configFile.Bind<int>("Salt Tank", "Rarity", 1000, "Salt tank spawn rarity");
            isSaltTankPurchasable = AddonFusion.configFile.Bind<bool>("Salt Tank", "Purchasable", true, "Is the salt tank purchasable?");
            saltTankPrice = AddonFusion.configFile.Bind<int>("Salt Tank", "Price", 30, "Salt tank price");
            saltTankEntityValues = AddonFusion.configFile.Bind<string>("Salt Tank", "Values", "default:20:10", "Values per entity, the format is EntityName:BaseChance:AdditionalChance.\nBaseChance: Base chance for the entity to stop chasing when hit by a salted graffiti.\nAdditionalChance: Additional chance for the entity to stop chasing when hit by a salted graffiti.");
            // PROTECTIVE CORD
            isCordEnabled = AddonFusion.configFile.Bind<bool>("Protective Cord", "Enable", true, "Is the protective cord enabled?");
            isCordSpawnable = AddonFusion.configFile.Bind<bool>("Protective Cord", "Spawnable", true, "Is the protective cord spawnable?");
            cordRarity = AddonFusion.configFile.Bind<int>("Protective Cord", "Rarity", 1000, "Protective cord spawn rarity");
            isCordPurchasable = AddonFusion.configFile.Bind<bool>("Protective Cord", "Purchasable", true, "Is the protective cord purchasable?");
            cordPrice = AddonFusion.configFile.Bind<int>("Protective Cord", "Price", 30, "Protective cord price");
            // FLASHLIGHT LENS
            isLensEnabled = AddonFusion.configFile.Bind<bool>("Flashlight Lens", "Enable", true, "Is the flashlight lens enabled?");
            isLensSpawnable = AddonFusion.configFile.Bind<bool>("Flashlight Lens", "Spawnable", true, "Is the flashlight lens spawnable?");
            lensRarity = AddonFusion.configFile.Bind<int>("Flashlight Lens", "Rarity", 1000, "Flashlight lens spawn rarity");
            isLensPurchasable = AddonFusion.configFile.Bind<bool>("Flashlight Lens", "Purchasable", true, "Is the flashlight lens purchasable?");
            lensPrice = AddonFusion.configFile.Bind<int>("Flashlight Lens", "Price", 30, "Flashlight lens price");
            lensEntityValues = AddonFusion.configFile.Bind<string>("Flashlight Lens", "Values", "default:2:3:15:120:10:10:0,Player:2:0:10:60:10:0:0,ForestGiant:2:4:30:120:20:20:0", "Values per entity, the format is EntityName:FlashTime:StunTime:LightAngle:EntityAngle:EntityDistance:ImmunityTime:BatteryConsumption.");
            lensExclusions = AddonFusion.configFile.Bind<string>("Flashlight Lens", "Exclusion list", "MouthDog", "List of creatures that will not be affected by the stun.");
            // BLADE SHARPENER
            isSharpenerEnabled = AddonFusion.configFile.Bind<bool>("Blade Sharpener", "Enable", true, "Is the blade sharpener enabled?");
            isSharpenerSpawnable = AddonFusion.configFile.Bind<bool>("Blade Sharpener", "Spawnable", true, "Is the blade sharpener spawnable?");
            sharpenerRarity = AddonFusion.configFile.Bind<int>("Blade Sharpener", "Rarity", 1000, "Blade sharpener spawn rarity");
            isSharpenerPurchasable = AddonFusion.configFile.Bind<bool>("Blade Sharpener", "Purchasable", true, "Is the blade sharpener purchasable?");
            sharpenerPrice = AddonFusion.configFile.Bind<int>("Blade Sharpener", "Price", 30, "Blade sharpener price");
        }

        internal static List<CapsuleHoiPoiValue> GetCapsuleValuesFromConfig()
        {
            List<CapsuleHoiPoiValue> capsuleValues = new List<CapsuleHoiPoiValue>();
            string[] items = capsuleItemValues.Value.Split(',');
            foreach (string itemValue in items)
            {
                string[] values = itemValue.Split(':');
                if (values.Length == 8)
                {
                    capsuleValues.Add(new CapsuleHoiPoiValue(values[0],
                        int.Parse(values[1])));
                }
            }
            return capsuleValues;
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

        internal static List<SaltTankValue> GetSaltTankValuesFromConfig()
        {
            List<SaltTankValue> saltTankValues = new List<SaltTankValue>();
            string[] entities = saltTankEntityValues.Value.Split(',');
            foreach (string entityValue in entities)
            {
                string[] values = entityValue.Split(':');
                if (values.Length == 8)
                {
                    saltTankValues.Add(new SaltTankValue(values[0],
                        int.Parse(values[1]),
                        int.Parse(values[2])));
                }
            }
            return saltTankValues;
        }
    }
}
