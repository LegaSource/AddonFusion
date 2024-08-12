using AddonFusion.Behaviours;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class GameNetworkManagerPatch
    {
        [HarmonyPatch(typeof(GameNetworkManager), "SaveItemsInShip")]
        [HarmonyPostfix]
        private static void SaveItems(ref GameNetworkManager __instance)
        {
            GrabbableObject[] grabbableObjects = UnityEngine.Object.FindObjectsByType<GrabbableObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (grabbableObjects == null || grabbableObjects.Length == 0)
            {
                ES3.DeleteKey("shipAddonFusionItemIDs", __instance.currentSaveFileName);
            }
            else
            {
                if (StartOfRound.Instance.isChallengeFile)
                {
                    return;
                }
                List<string> listAddonFusionItemIDs = new List<string>();
                for (int i = 0; i < grabbableObjects.Length && i <= StartOfRound.Instance.maxShipItemCapacity; i++)
                {
                    if (!StartOfRound.Instance.allItemsList.itemsList.Contains(grabbableObjects[i].itemProperties) || grabbableObjects[i].deactivated)
                    {
                        continue;
                    }
                    if (grabbableObjects[i].itemProperties.spawnPrefab == null)
                    {
                        AddonFusion.mls.LogError("Item '" + grabbableObjects[i].itemProperties.itemName + "' has no spawn prefab set!");
                    }
                    else
                    {
                        if (grabbableObjects[i].itemUsedUp)
                        {
                            continue;
                        }
                        for (int j = 0; j < StartOfRound.Instance.allItemsList.itemsList.Count; j++)
                        {
                            string addonName = null;
                            Addon addon;
                            if (StartOfRound.Instance.allItemsList.itemsList[j] == grabbableObjects[i].itemProperties
                                && (addon = AFUtilities.GetAddonInstalled(grabbableObjects[i])) != null)
                            {
                                addonName = addon.addonName;
                            }
                            int maxUse = -1;
                            EphemeralItem ephemeralItem;
                            if (StartOfRound.Instance.allItemsList.itemsList[j] == grabbableObjects[i].itemProperties
                                && (ephemeralItem = AFUtilities.GetEphemeralItem(grabbableObjects[i])) != null)
                            {
                                if (grabbableObjects[i] is FlashlightItem flashlight)
                                {
                                    maxUse = (int)(flashlight.insertedBattery.charge * 100f);
                                }
                                else if (grabbableObjects[i] is SprayPaintItem sprayPaintItem)
                                {
                                    maxUse = (int)(sprayPaintItem.sprayCanTank * 100f);
                                }
                                else if (grabbableObjects[i] is TetraChemicalItem tetraChemicalItem)
                                {
                                    maxUse = (int)(tetraChemicalItem.fuel * 100f);
                                }
                                else
                                {
                                    maxUse = ephemeralItem.maxUse - ephemeralItem.use;
                                }
                            }
                            if (!string.IsNullOrEmpty(addonName) || maxUse != -1)
                            {
                                listAddonFusionItemIDs.Add(j + "," + addonName + "," + maxUse);
                            }
                        }
                    }
                }
                if (listAddonFusionItemIDs.Count <= 0)
                {
                    AddonFusion.mls.LogDebug("No value to save.");
                }
                else
                {
                    ES3.Save("shipAddonFusionItemIDs", listAddonFusionItemIDs.ToArray(), __instance.currentSaveFileName);
                }
            }
        }
    }
}
