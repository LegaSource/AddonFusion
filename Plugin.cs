﻿using BepInEx.Configuration;
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
        private const string modVersion = "1.0.4";

        private readonly Harmony harmony = new Harmony(modGUID);
        private readonly static AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "addonfusion"));
        internal static ManualLogSource mls;
        public static ConfigFile configFile;

        public static GameObject managerPrefab = NetworkPrefabs.CreateNetworkPrefab("AddonFusionNetworkManager");

        public static List<CustomItem> customItems = new List<CustomItem>();
        public static List<CapsuleHoiPoiValue> capsuleHoiPoiValues = new List<CapsuleHoiPoiValue>();
        public static List<SaltTankValue> saltTankValues = new List<SaltTankValue>();
        public static List<ProtectiveCordValue> protectiveCordValues = new List<ProtectiveCordValue>();
        public static List<LensValue> lensValues = new List<LensValue>();
        public static List<BladeSharpenerValue> bladeSharpenerValues = new List<BladeSharpenerValue>();

        public static GameObject stunParticle;
        public static GameObject parrySound;

        public void Awake()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource("AddonFusion");
            configFile = Config;
            ConfigManager.Load();
            capsuleHoiPoiValues = ConfigManager.GetCapsuleValuesFromConfig();
            saltTankValues = ConfigManager.GetSaltTankValuesFromConfig();
            protectiveCordValues = ConfigManager.GetProtectiveCordValuesFromConfig();
            lensValues = ConfigManager.GetLensValuesFromConfig();
            bladeSharpenerValues = ConfigManager.GetBladeSharpenerValuesFromConfig();

            LoadManager();
            NetcodePatcher();
            LoadItems();
            //LoadParticles();
            LoadAudios();

            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(VehicleControllerPatch));
            harmony.PatchAll(typeof(AddonFusion));
            harmony.PatchAll(typeof(GrabbableObjectPatch));
            harmony.PatchAll(typeof(FlashlightItemPatch));
            harmony.PatchAll(typeof(SprayPaintItemPatch));
            harmony.PatchAll(typeof(EnemyAIPatch));
            harmony.PatchAll(typeof(StormyWeatherPatch));
        }

        public static void LoadManager()
        {
            Utilities.FixMixerGroups(managerPrefab);
            managerPrefab.AddComponent<AddonFusionNetworkManager>();
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
            customItems = new List<CustomItem>
            {
                new CustomItem(ConfigManager.isCapsuleEnabled.Value, typeof(CapsuleHoiPoi), bundle.LoadAsset<Item>("Assets/CapsuleHoiPoi/CapsuleHoiPoiItem.asset"), ConfigManager.isCapsuleSpawnable.Value, ConfigManager.capsuleRarity.Value, ConfigManager.isCapsulePurchasable.Value, "This capsule was created by Capsule Corporation, it allows you to store any object\n\n", ConfigManager.capsulePrice.Value),
                new CustomItem(ConfigManager.isSaltTankEnabled.Value, typeof(SaltTank), bundle.LoadAsset<Item>("Assets/SaltTank/SaltTankItem.asset"), ConfigManager.isSaltTankSpawnable.Value, ConfigManager.saltTankRarity.Value, ConfigManager.isSaltTankPurchasable.Value, "A salt tank to use with the spray paint, allowing you to repel spirits\n\n", ConfigManager.saltTankPrice.Value),
                new CustomItem(ConfigManager.isCordEnabled.Value, typeof(ProtectiveCord), bundle.LoadAsset<Item>("Assets/ProtectiveCord/ProtectiveCordItem.asset"), ConfigManager.isCordSpawnable.Value, ConfigManager.cordRarity.Value, ConfigManager.isCordPurchasable.Value, "A protective cord to use with the shovel, blocks damage\n\n", ConfigManager.cordPrice.Value),
                new CustomItem(ConfigManager.isLensEnabled.Value, typeof(FlashlightLens), bundle.LoadAsset<Item>("Assets/Lens/FlashlightLensItem.asset"), ConfigManager.isLensSpawnable.Value, ConfigManager.lensRarity.Value, ConfigManager.isLensPurchasable.Value, "A lens that optimises the power of the light\n\n", ConfigManager.lensPrice.Value),
                new CustomItem(ConfigManager.isSharpenerEnabled.Value, typeof(BladeSharpener), bundle.LoadAsset<Item>("Assets/BladeSharpener/BladeSharpenerItem.asset"), ConfigManager.isSharpenerSpawnable.Value, ConfigManager.sharpenerRarity.Value, ConfigManager.isSharpenerPurchasable.Value, "A blade sharpener to use with the knife, deal critical damage\n\n", ConfigManager.sharpenerPrice.Value)
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
                    Items.RegisterItem(customItem.Item);

                    if (customItem.IsPurchasable)
                    {
                        TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
                        node.clearPreviousText = true;
                        node.displayText = customItem.Description;
                        Items.RegisterShopItem(customItem.Item, null, null, node, customItem.Price);
                    }
                }
            }
        }

        public static void LoadParticles()
        {
            stunParticle = bundle.LoadAsset<GameObject>("Assets/Stun/StunParticule.prefab");
            NetworkPrefabs.RegisterNetworkPrefab(stunParticle);
            Utilities.FixMixerGroups(stunParticle);
        }

        public static void LoadAudios()
        {
            parrySound = bundle.LoadAsset<GameObject>("Assets/ParrySound.prefab");
            NetworkPrefabs.RegisterNetworkPrefab(parrySound);
            Utilities.FixMixerGroups(parrySound);
        }
    }
}
