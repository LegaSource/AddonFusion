using AddonFusion.AddonValues;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Globalization;

namespace AddonFusion
{
    internal class ConfigManager
    {
        // FLASHLIGHT LENS
        public static ConfigEntry<bool> isLensEnabled;
        public static ConfigEntry<bool> isLensSpawnable;
        public static ConfigEntry<int> lensRarity;
        public static ConfigEntry<bool> isLensPurchasable;
        public static ConfigEntry<int> lensPrice;
        public static ConfigEntry<string> lensEntityValues;
        public static ConfigEntry<string> lensExclusions;

        internal static void Load()
        {
            // FLASHLIGHT LENS
            isLensEnabled = AddonFusion.configFile.Bind<bool>("Flashlight Lens", "Enable", true, "Is the flashlight lens enabled?");
            isLensSpawnable = AddonFusion.configFile.Bind<bool>("Flashlight Lens", "Spawnable", true, "Is the flashlight lens spawnable?");
            lensRarity = AddonFusion.configFile.Bind<int>("Flashlight Lens", "Rarity", 1000, "Flashlight lens spawn rarity");
            isLensPurchasable = AddonFusion.configFile.Bind<bool>("Flashlight Lens", "Purchasable", true, "Is the flashlight lens purchasable?");
            lensPrice = AddonFusion.configFile.Bind<int>("Flashlight Lens", "Price", 20, "Flashlight lens price");
            lensEntityValues = AddonFusion.configFile.Bind<string>("Flashlight Lens", "Values", "default:2:3:15:120:10:10:0,Player:2:0:10:120:10:0:0,ForestGiant:2:4:30:120:20:20:0", "Values per entity, the format is EntityName:FlashTime:StunTime:LightAngle:EntityAngle:EntityDistance:ImmunityTime:BatteryConsumption.");
            lensExclusions = AddonFusion.configFile.Bind<string>("Flashlight Lens", "Exclusion list", null, "List of creatures that will not be affected by the stun.");
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
    }
}
