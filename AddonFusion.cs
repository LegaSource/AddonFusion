using AddonFusion.Behaviours.AddonComponents;
using AddonFusion.Behaviours.AddonProps;
using AddonFusion.Managers;
using AddonFusion.Patches;
using AddonFusion.Registries;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LethalLib.Extras;
using LethalLib.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using static LegaFusionCore.Registries.LFCSpawnableItemRegistry;

namespace AddonFusion;

[BepInPlugin(modGUID, modName, modVersion)]
public class AddonFusion : BaseUnityPlugin
{
    public const string modGUID = "Lega.AddonFusion";
    public const string modName = "Addon Fusion";
    public const string modVersion = "2.0.1";

    private readonly Harmony harmony = new Harmony(modGUID);
    private static readonly AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "addonfusion"));
    internal static ManualLogSource mls;
    public static ConfigFile configFile;

    public static GameObject managerPrefab = NetworkPrefabs.CreateNetworkPrefab("AddonFusionNetworkManager");

    public static GameObject parryAudio;

    public void Awake()
    {
        mls = BepInEx.Logging.Logger.CreateLogSource("AddonFusion");
        configFile = Config;
        ConfigManager.Load();

        LoadManager();
        NetcodePatcher();
        LoadItems();
        LoadUnlockables();
        LoadNetworkPrefabs();

        harmony.PatchAll(typeof(GameNetworkManagerPatch));
        harmony.PatchAll(typeof(StartOfRoundPatch));
        harmony.PatchAll(typeof(PlayerControllerBPatch));
        harmony.PatchAll(typeof(GrabbableObjectPatch));
        harmony.PatchAll(typeof(ShotgunItemPatch));
        harmony.PatchAll(typeof(SprayPaintItemPatch));
        harmony.PatchAll(typeof(EnemyAIPatch));
    }

    public static void LoadManager()
    {
        Utilities.FixMixerGroups(managerPrefab);
        _ = managerPrefab.AddComponent<AddonFusionNetworkManager>();
    }

    private static void NetcodePatcher()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (Type type in types)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (MethodInfo method in methods)
            {
                object[] attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                if (attributes.Length > 0)
                    _ = method.Invoke(null, null);
            }
        }
    }

    public void LoadItems()
    {
        RegisterAddon(addonType: typeof(BladeSharpener),
            addonName: Constants.BLADE_SHARPENER,
            itemType: typeof(BladeSharpenerItem),
            item: bundle.LoadAsset<Item>("Assets/AddonProps/BladeSharpenerItem.asset"),
            minSpawn: 0,
            maxSpawn: 2,
            rarity: ConfigManager.bladeSharpenerRarity.Value,
            minValue: ConfigManager.bladeSharpenerMinValue.Value,
            maxValue: ConfigManager.bladeSharpenerMaxValue.Value,
            isPurchasable: ConfigManager.isBladeSharpenerPurchasable.Value,
            price: ConfigManager.bladeSharpenerPrice.Value,
            nodeText: "");
        RegisterAddon(addonType: typeof(EchoScanner),
            addonName: Constants.ECHO_SCANNER,
            itemType: typeof(EchoScannerItem),
            item: bundle.LoadAsset<Item>("Assets/AddonProps/EchoScannerItem.asset"),
            minSpawn: 0,
            maxSpawn: 2,
            rarity: ConfigManager.echoScannerRarity.Value,
            minValue: ConfigManager.echoScannerMinValue.Value,
            maxValue: ConfigManager.echoScannerMaxValue.Value,
            isPurchasable: ConfigManager.isEchoScannerPurchasable.Value,
            price: ConfigManager.echoScannerPrice.Value,
            nodeText: "");
        RegisterAddon(addonType: typeof(FlashlightLens),
            addonName: Constants.FLASHLIGHT_LENS,
            itemType: typeof(FlashlightLensItem),
            item: bundle.LoadAsset<Item>("Assets/AddonProps/FlashlightLensItem.asset"),
            minSpawn: 0,
            maxSpawn: 2,
            rarity: ConfigManager.flashlightLensRarity.Value,
            minValue: ConfigManager.flashlightLensMinValue.Value,
            maxValue: ConfigManager.flashlightLensMaxValue.Value,
            isPurchasable: ConfigManager.isFlashlightLensPurchasable.Value,
            price: ConfigManager.flashlightLensPrice.Value,
            nodeText: "");
        RegisterAddon(addonType: typeof(MarkerCompound),
            addonName: Constants.MARKER_COMPOUND,
            itemType: typeof(MarkerCompoundItem),
            item: bundle.LoadAsset<Item>("Assets/AddonProps/MarkerCompoundItem.asset"),
            minSpawn: 0,
            maxSpawn: 2,
            rarity: ConfigManager.markerCompoundRarity.Value,
            minValue: ConfigManager.markerCompoundMinValue.Value,
            maxValue: ConfigManager.markerCompoundMaxValue.Value,
            isPurchasable: ConfigManager.isMarkerCompoundPurchasable.Value,
            price: ConfigManager.markerCompoundPrice.Value,
            nodeText: "");
        RegisterAddon(addonType: typeof(ProtectiveCord),
            addonName: Constants.PROTECTIVE_CORD,
            itemType: typeof(ProtectiveCordItem),
            item: bundle.LoadAsset<Item>("Assets/AddonProps/ProtectiveCordItem.asset"),
            minSpawn: 0,
            maxSpawn: 2,
            rarity: ConfigManager.protectiveCordRarity.Value,
            minValue: ConfigManager.protectiveCordMinValue.Value,
            maxValue: ConfigManager.protectiveCordMaxValue.Value,
            isPurchasable: ConfigManager.isProtectiveCordPurchasable.Value,
            price: ConfigManager.protectiveCordPrice.Value,
            nodeText: "");
        RegisterAddon(addonType: typeof(SaltTank),
            addonName: Constants.SALT_TANK,
            itemType: typeof(SaltTankItem),
            item: bundle.LoadAsset<Item>("Assets/AddonProps/SaltTankItem.asset"),
            minSpawn: 0,
            maxSpawn: 2,
            rarity: ConfigManager.saltTankRarity.Value,
            minValue: ConfigManager.saltTankMinValue.Value,
            maxValue: ConfigManager.saltTankMaxValue.Value,
            isPurchasable: ConfigManager.isSaltTankPurchasable.Value,
            price: ConfigManager.saltTankPrice.Value,
            nodeText: "");
        RegisterAddon(addonType: typeof(SpectralFrequency),
            addonName: Constants.SPECTRAL_FREQUENCY,
            itemType: typeof(SpectralFrequencyItem),
            item: bundle.LoadAsset<Item>("Assets/AddonProps/SpectralFrequencyItem.asset"),
            minSpawn: 0,
            maxSpawn: 2,
            rarity: ConfigManager.spectralFrequencyRarity.Value,
            minValue: ConfigManager.spectralFrequencyMinValue.Value,
            maxValue: ConfigManager.spectralFrequencyMaxValue.Value,
            isPurchasable: ConfigManager.isSpectralFrequencyPurchasable.Value,
            price: ConfigManager.spectralFrequencyPrice.Value,
            nodeText: "");
    }

    public static void RegisterAddon(Type addonType, string addonName, Type itemType, Item item, int minSpawn, int maxSpawn, int rarity, int minValue, int maxValue, bool isPurchasable, int price, string nodeText)
    {
        Add(itemType, item, minSpawn, maxSpawn, rarity, minValue, maxValue);

        if (isPurchasable)
        {
            TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
            node.clearPreviousText = true;
            node.displayText = nodeText;
            Items.RegisterShopItem(item, null, null, node, price);
        }
        AddonObjectRegistry.Add(addonType, addonName, item.spawnPrefab);
    }

    public void LoadUnlockables()
    {
        UnlockableItemDef unlockable = bundle.LoadAsset<UnlockableItemDef>("Assets/AddonCapsule/AddonCapsuleItem.asset");
        NetworkPrefabs.RegisterNetworkPrefab(unlockable.unlockable.prefabObject);
        Utilities.FixMixerGroups(unlockable.unlockable.prefabObject);
        Unlockables.RegisterUnlockable(unlockable, StoreType.ShipUpgrade, itemInfo: bundle.LoadAsset<TerminalNode>("Assets/AddonCapsule/AddonCapsuleTN.asset"), price: 30);
    }

    public void LoadNetworkPrefabs()
    {
        HashSet<GameObject> gameObjects =
        [
            // Audios
            (parryAudio = bundle.LoadAsset<GameObject>("Assets/SFX/ParryAudio.prefab"))
        ];

        foreach (GameObject gameObject in gameObjects)
        {
            NetworkPrefabs.RegisterNetworkPrefab(gameObject);
            Utilities.FixMixerGroups(gameObject);
        }
    }
}
