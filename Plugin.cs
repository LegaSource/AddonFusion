using BepInEx.Configuration;
using BepInEx;
using HarmonyLib;
using AddonFusion.Patches;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using LethalLib.Modules;
using AddonFusion.Behaviours;
using BepInEx.Logging;
using AddonFusion.AddonValues;

namespace AddonFusion
{
    [BepInPlugin(modGUID, modName, modVersion)]
    internal class AddonFusion : BaseUnityPlugin
    {
        private const string modGUID = "Lega.AddonFusion";
        private const string modName = "Addon Fusion";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);
        internal static ManualLogSource mls;
        public static ConfigFile configFile;

        public static List<CustomItem> customItems = new List<CustomItem>();
        public static List<LensValue> lensValues = new List<LensValue>();

        public void Awake()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource("AddonFusion");
            configFile = Config;
            ConfigManager.Load();
            lensValues = ConfigManager.GetLensValuesFromConfig();

            NetcodePatcher();
            LoadItems();            

            harmony.PatchAll(typeof(GrabbableObjectPatch));
            harmony.PatchAll(typeof(AddonFusion));
            harmony.PatchAll(typeof(FlashlightItemPatch));
            harmony.PatchAll(typeof(EnemyAIPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));
        }

        private static void NetcodePatcher()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }

        public static void LoadItems()
        {
            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "addonfusion");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);
            
            customItems = new List<CustomItem>
            {
                new CustomItem(true, typeof(FlashlightLens), bundle.LoadAsset<Item>("Assets/Lens/FlashlightLensItem.asset"), ConfigManager.isLensSpawnable.Value, ConfigManager.lensRarity.Value, ConfigManager.isLensPurchasable.Value, "This is info about item\n\n", ConfigManager.lensPrice.Value)
            };

            foreach (CustomItem customItem in customItems)
            {
                if (customItem.IsEnabled)
                {
                    var script = customItem.Item.spawnPrefab.AddComponent(customItem.Type) as PhysicsProp;
                    script.grabbable = true;
                    script.grabbableToEnemies = true;
                    script.itemProperties = customItem.Item;

                    NetworkPrefabs.RegisterNetworkPrefab(customItem.Item.spawnPrefab);
                    Utilities.FixMixerGroups(customItem.Item.spawnPrefab);

                    if (customItem.IsSpawnable)
                    {
                        Items.RegisterScrap(customItem.Item, customItem.Rarity, Levels.LevelTypes.All);
                    }

                    if (customItem.IsPurchasable)
                    {
                        TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
                        node.clearPreviousText = true;
                        node.displayText = customItem.Description;
                        Items.RegisterShopItem(customItem.Item, null, null, node, customItem.Price);
                    }
                }
            }

            /*FlashlightLens script = flashlightLens.spawnPrefab.AddComponent<FlashlightLens>();
            script.grabbable = true;
            script.grabbableToEnemies = true;
            script.itemProperties = flashlightLens;

            NetworkPrefabs.RegisterNetworkPrefab(flashlightLens.spawnPrefab);
            Utilities.FixMixerGroups(flashlightLens.spawnPrefab);
            Items.RegisterScrap(flashlightLens, 1000, Levels.LevelTypes.All);

            TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
            node.clearPreviousText = true;
            node.displayText = "This is info about item\n\n";
            Items.RegisterShopItem(flashlightLens, null, null, node, 0);*/
        }
    }
}
