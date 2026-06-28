using AddonFusion.Behaviours.AddonComponents;
using HarmonyLib;
using LegaFusionCore.Registries;
using System.Collections.Generic;

namespace AddonFusion.Patches;

public class GameNetworkManagerPatch
{
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.SaveItemsInShip))]
    [HarmonyPostfix]
    public static void SaveAddons(GameNetworkManager __instance)
    {
        List<GrabbableObject> grabbableObjects = LFCSpawnRegistry.GetAllAs<GrabbableObject>();
        if (grabbableObjects == null || grabbableObjects.Count == 0 || StartOfRound.Instance.isChallengeFile)
        {
            ES3.DeleteKey("shipAddonIDs", __instance.currentSaveFileName);
            return;
        }

        List<string> shipAddonIDs = [];
        for (int i = 0; i < grabbableObjects.Count && i <= StartOfRound.Instance.maxShipItemCapacity; i++)
        {
            if (!StartOfRound.Instance.allItemsList.itemsList.Contains(grabbableObjects[i].itemProperties) || grabbableObjects[i].deactivated || grabbableObjects[i].itemUsedUp)
            {
                if (grabbableObjects[i].itemProperties.spawnPrefab == null)
                    AddonFusion.mls.LogError($"Item '{grabbableObjects[i].itemProperties.itemName}' has no spawn prefab set!");
                continue;
            }
            for (int j = 0; j < StartOfRound.Instance.allItemsList.itemsList.Count; j++)
            {
                if (StartOfRound.Instance.allItemsList.itemsList[j] == grabbableObjects[i].itemProperties && AFUtilities.TryGetAddonComponent(grabbableObjects[i], out AddonComponent addonComponent))
                    shipAddonIDs.Add($"{j},{addonComponent.AddonName}");
            }
        }
        if (shipAddonIDs.Count > 0)
            ES3.Save("shipAddonIDs", shipAddonIDs.ToArray(), __instance.currentSaveFileName);
    }
}
