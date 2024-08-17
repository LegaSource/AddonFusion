using AddonFusion.Behaviours;
using GameNetcodeStuff;
using HarmonyLib;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class StartOfRoundPatch
    {
        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyBefore(["evaisa.lethallib"])]
        [HarmonyPostfix]
        private static void StartRound(StartOfRound __instance)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                if (AddonFusionNetworkManager.Instance == null)
                {
                    GameObject gameObject = Object.Instantiate(AddonFusion.managerPrefab, __instance.transform.parent);
                    gameObject.GetComponent<NetworkObject>().Spawn();
                    AddonFusion.mls.LogInfo("Spawning AddonFusionNetworkManager");
                }
                LoadItems(__instance);
            }
            if (ConfigManager.isEphemeralEnabled.Value && AddonFusion.customEphemeralItems.Count <= 0)
            {
                LoadCustomEphemeralItems(__instance);
            }
        }

        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientConnect))]
        [HarmonyPostfix]
        private static void SyncItems(ref StartOfRound __instance)
        {
            if (__instance.IsServer)
            {
                EphemeralItem ephemeralItem;
                foreach (GrabbableObject grabbableObject in Object.FindObjectsByType<GrabbableObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                {
                    if ((ephemeralItem = AFUtilities.GetEphemeralItem(grabbableObject)) != null)
                    {
                        int maxUse = ephemeralItem.maxUse;
                        if (grabbableObject is FlashlightItem flashlight)
                        {
                            maxUse = (int)(flashlight.insertedBattery.charge * 100f);
                        }
                        else if (grabbableObject is SprayPaintItem sprayPaintItem)
                        {
                            maxUse = (int)(sprayPaintItem.sprayCanTank * 100f);
                        }
                        else if (grabbableObject is TetraChemicalItem tetraChemicalItem)
                        {
                            maxUse = (int)(tetraChemicalItem.fuel * 100f);
                        }
                        AddonFusionNetworkManager.Instance.SetEphemeralClientRpc(grabbableObject.GetComponent<NetworkObject>(), maxUse);
                    }
                }
                Addon addon;
                foreach (GrabbableObject grabbableObject in Object.FindObjectsByType<GrabbableObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                {
                    if ((addon = AFUtilities.GetAddonInstalled(grabbableObject)) != null)
                    {
                        AddonFusionNetworkManager.Instance.SetAddonClientRpc(grabbableObject.GetComponent<NetworkObject>(), addon.addonName);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnPlayerConnectedClientRpc))]
        [HarmonyPostfix]
        private static void PlayerConnection(ref StartOfRound __instance)
        {
            foreach (PlayerControllerB player in __instance.allPlayerScripts)
            {
                if (player.isPlayerControlled && player.GetComponent<PlayerAFBehaviour>() == null)
                {
                    player.gameObject.AddComponent<PlayerAFBehaviour>();
                }
            }
        }

        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnDisable))]
        [HarmonyPostfix]
        public static void OnDisable()
        {
            AddonFusionNetworkManager.Instance = null;
        }

        public static void LoadCustomEphemeralItems(StartOfRound startOfRound)
        {
            foreach (Item item in startOfRound.allItemsList.itemsList.Where(i => !i.isScrap || i.itemName.Equals("Kitchen knife")))
            {
                if (ConfigManager.isEphemeralFlashEnabled.Value && item.spawnPrefab?.GetComponent<FlashlightItem>() != null && !item.itemName.Equals("Laser pointer"))
                {
                    AddonFusion.customEphemeralItems.Add(new CutomEphemeralItem(item,
                        ConfigManager.ephemeralFlashRarity.Value,
                        ConfigManager.ephemeralFlashAddonRarity.Value,
                        ConfigManager.ephemeralFlashMinUse.Value,
                        ConfigManager.ephemeralFlashMaxUse.Value));
                }
                else if (ConfigManager.isEphemeralShovelEnabled.Value && item.spawnPrefab?.GetComponent<Shovel>() != null)
                {
                    AddonFusion.customEphemeralItems.Add(new CutomEphemeralItem(item,
                        ConfigManager.ephemeralShovelRarity.Value,
                        ConfigManager.ephemeralShovelAddonRarity.Value,
                        ConfigManager.ephemeralShovelMinUse.Value,
                        ConfigManager.ephemeralShovelMaxUse.Value));
                }
                else if (item.spawnPrefab?.GetComponent<SprayPaintItem>() != null)
                {
                    SprayPaintItem sprayPaint = item.spawnPrefab.GetComponent<SprayPaintItem>();
                    if (ConfigManager.isEphemeralWeedKillerEnabled.Value && sprayPaint.isWeedKillerSprayBottle)
                    {
                        AddonFusion.customEphemeralItems.Add(new CutomEphemeralItem(item,
                            ConfigManager.ephemeralWeedKillerRarity.Value,
                            ConfigManager.ephemeralWeedKillerAddonRarity.Value,
                            ConfigManager.ephemeralWeedKillerMinUse.Value,
                            ConfigManager.ephemeralWeedKillerMaxUse.Value));
                    }
                    else if (ConfigManager.isEphemeralSprayPaintEnabled.Value && !sprayPaint.isWeedKillerSprayBottle)
                    {
                        AddonFusion.customEphemeralItems.Add(new CutomEphemeralItem(item,
                            ConfigManager.ephemeralSprayPaintRarity.Value,
                            ConfigManager.ephemeralSprayPaintAddonRarity.Value,
                            ConfigManager.ephemeralSprayPaintMinUse.Value,
                            ConfigManager.ephemeralSprayPaintMaxUse.Value));
                    }
                }
                else if (ConfigManager.isEphemeralKnifeEnabled.Value && item.spawnPrefab?.GetComponent<KnifeItem>() != null)
                {
                    AddonFusion.customEphemeralItems.Add(new CutomEphemeralItem(item,
                        ConfigManager.ephemeralKnifeRarity.Value,
                        ConfigManager.ephemeralKnifeAddonRarity.Value,
                        ConfigManager.ephemeralKnifeMinUse.Value,
                        ConfigManager.ephemeralKnifeMaxUse.Value));
                }
                else if (ConfigManager.isEphemeralTZPEnabled.Value && item.spawnPrefab?.GetComponent<TetraChemicalItem>() != null)
                {
                    AddonFusion.customEphemeralItems.Add(new CutomEphemeralItem(item,
                        ConfigManager.ephemeralTZPRarity.Value,
                        ConfigManager.ephemeralTZPAddonRarity.Value,
                        ConfigManager.ephemeralTZPMinUse.Value,
                        ConfigManager.ephemeralTZPMaxUse.Value));
                }
            }
        }

        private static void LoadItems(StartOfRound startOfRound)
        {
            if (!ES3.KeyExists("shipAddonFusionItemIDs", GameNetworkManager.Instance.currentSaveFileName))
            {
                AddonFusion.mls.LogWarning("Key 'shipAddonFusionItemIDs' does not exist");
                return;
            }
            string[] listAddonFusionItemIDs = ES3.Load<string[]>("shipAddonFusionItemIDs", GameNetworkManager.Instance.currentSaveFileName);
            if (listAddonFusionItemIDs == null || listAddonFusionItemIDs == null)
            {
                AddonFusion.mls.LogError("Ship addon fusion list loaded from file returns a null value!");
                return;
            }
            foreach (string addonFusionItemID in listAddonFusionItemIDs)
            {
                string[] values = addonFusionItemID.Split(',');
                int itemID = int.Parse(values[0]);
                string addonName = values[1];
                int maxUse = int.Parse(values[2]);

                if (startOfRound.allItemsList.itemsList[itemID] != null)
                {
                    GrabbableObject grabbableObject = Object.FindObjectsByType<GrabbableObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                        .FirstOrDefault(g => g.itemProperties == startOfRound.allItemsList.itemsList[itemID]
                            && AFUtilities.GetAddonInstalled(g) == null
                            && AFUtilities.GetEphemeralItem(g) == null);
                    if (grabbableObject != null)
                    {
                        if (maxUse != -1)
                        {
                            AddonFusionNetworkManager.Instance.SetEphemeralItem(grabbableObject, maxUse);
                        }
                        if (!string.IsNullOrEmpty(addonName))
                        {
                            AddonFusionNetworkManager.Instance.SetAddon(grabbableObject, addonName);
                        }
                    }
                }
            }
        }

        private static void LoadAddons(StartOfRound startOfRound)
        {
            if (!ES3.KeyExists("shipAddonItemIDs", GameNetworkManager.Instance.currentSaveFileName))
            {
                AddonFusion.mls.LogWarning("Key 'shipAddonItemIDs' does not exist");
                return;
            }
            string[] listAddonItemIDs = ES3.Load<string[]>("shipAddonItemIDs", GameNetworkManager.Instance.currentSaveFileName);
            if (listAddonItemIDs == null || listAddonItemIDs == null)
            {
                AddonFusion.mls.LogError("Ship addon list loaded from file returns a null value!");
                return;
            }
            foreach (string addonItemID in listAddonItemIDs)
            {
                string[] values = addonItemID.Split(',');
                int itemID = int.Parse(values[0]);
                string addonName = values[1];

                if (startOfRound.allItemsList.itemsList[itemID] != null)
                {
                    GrabbableObject grabbableObject = Object.FindObjectsByType<GrabbableObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                        .FirstOrDefault(g => g.itemProperties == startOfRound.allItemsList.itemsList[itemID]);
                    if (grabbableObject != null)
                    {
                        string toolTip = AddonFusion.customItems.Where(i => i.Item.spawnPrefab?.GetComponent<AddonProp>() != null && i.Item.itemName.Equals(addonName)).Select(i => i.Item.spawnPrefab.GetComponent<AddonProp>().ToolTip).FirstOrDefault();
                        AddonFusionNetworkManager.Instance.SetAddon(grabbableObject, addonName);
                    }
                }
            }
        }

        private static void LoadEphemeralItems(StartOfRound startOfRound)
        {
            if (!ES3.KeyExists("shipEphemeralItemIDs", GameNetworkManager.Instance.currentSaveFileName))
            {
                AddonFusion.mls.LogWarning("Key 'shipEphemeralItemIDs' does not exist");
                return;
            }
            string[] listEphemeralItemIDs = ES3.Load<string[]>("shipEphemeralItemIDs", GameNetworkManager.Instance.currentSaveFileName);
            if (listEphemeralItemIDs == null || listEphemeralItemIDs == null)
            {
                AddonFusion.mls.LogError("Ship ephemeral item list loaded from file returns a null value!");
                return;
            }
            foreach (string ephemeralItemID in listEphemeralItemIDs)
            {
                string[] values = ephemeralItemID.Split(',');
                int itemID = int.Parse(values[0]);
                int maxUse = int.Parse(values[1]);

                if (startOfRound.allItemsList.itemsList[itemID] != null)
                {
                    GrabbableObject grabbableObject = Object.FindObjectsByType<GrabbableObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                        .FirstOrDefault(g => g.itemProperties == startOfRound.allItemsList.itemsList[itemID]);
                    if (grabbableObject != null)
                    {
                        AddonFusionNetworkManager.Instance.SetEphemeralItem(grabbableObject, maxUse);
                    }
                }
            }
        }
    }
}
