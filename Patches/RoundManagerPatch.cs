using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Unity.Netcode;
using AddonFusion.Behaviours;

namespace AddonFusion.Patches
{
    internal class RoundManagerPatch
    {
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.SpawnScrapInLevel))]
        [HarmonyPostfix]
        private static void SpawnScraps(ref RoundManager __instance)
        {
            AddNewItems(ref __instance);
        }

        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.DetectElevatorIsRunning))]
        [HarmonyPostfix]
        private static void EndGame()
        {
            FlashlightItemPatch.blindableEnemies.Clear();
            if (ConfigManager.isEphemeralDestroyEnabled.Value)
            {
                foreach (GrabbableObject grabbableObject in UnityEngine.Object.FindObjectsByType<GrabbableObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(g => AFUtilities.GetEphemeralItem(g) != null))
                {
                    AddonFusionNetworkManager.Instance.DestroyObjectServerRpc(grabbableObject.GetComponent<NetworkObject>());
                }
            }
        }

        private static void AddNewItems(ref RoundManager roundManager)
        {
            AddItems(ref roundManager);
            AddEphemeralItems(ref roundManager);
        }

        private static void AddItems(ref RoundManager roundManager)
        {
            foreach (CustomItem customItem in AddonFusion.customItems.Where(i => i.IsSpawnable))
            {
                int nbActiveItem = 0;
                AddonProp addonProp = customItem.Item.spawnPrefab.GetComponent<AddonProp>();
                Addon addon;
                foreach (GrabbableObject grabbableObject in UnityEngine.Object.FindObjectsOfType<GrabbableObject>()
                    .Where(g => (!g.itemProperties.isScrap || g.itemProperties.itemName.Equals("Kitchen knife"))
                        && g.gameObject.GetComponent(addonProp.AddonType) != null
                        && addonProp.CheckSpecificItem(g.gameObject)))
                {
                    if ((addon = grabbableObject.gameObject.GetComponent<Addon>()) != null && !addon.hasAddon) nbActiveItem++;
                }
                int nbActiveAddon = UnityEngine.Object.FindObjectsOfType<GrabbableObject>().Where(g => g.itemProperties == customItem.Item).Count();
                for (int i = 0; i < nbActiveItem + ConfigManager.spawnAddonPerItem.Value - nbActiveAddon; i++)
                {
                    if (new System.Random().Next(1, 100) <= customItem.Rarity)
                    {
                        SpawnNewItem(ref roundManager, customItem.Item);
                    }
                }
            }
        }

        private static void AddEphemeralItems(ref RoundManager roundManager)
        {
            if (ConfigManager.isEphemeralEnabled.Value)
            {
                int nbEphemeralItem = new System.Random().Next(ConfigManager.minEphemeralItem.Value, ConfigManager.maxEphemeralItem.Value);
                int iteratorEphemeralItem = 0;
                while (iteratorEphemeralItem < nbEphemeralItem)
                {
                    List<CutomEphemeralItem> customEphemeralItems = AddonFusion.customEphemeralItems.Where(i => new System.Random().Next(1, 100) <= i.Rarity).ToList();
                    if (customEphemeralItems.Count <= 0)
                    {
                        AddonFusion.mls.LogWarning("No ephemeral items could be found");
                        break;
                    }
                    foreach (CutomEphemeralItem customEphemeralItem in customEphemeralItems)
                    {
                        GrabbableObject grabbableObject = SpawnNewItem(ref roundManager, customEphemeralItem.Item);
                        if (grabbableObject != null)
                        {
                            AddonFusionNetworkManager.Instance.SetEphemeralServerRpc(grabbableObject.GetComponent<NetworkObject>(), new System.Random().Next(customEphemeralItem.MinUse, customEphemeralItem.MaxUse));
                            if (new System.Random().Next(1, 100) <= customEphemeralItem.AddonRarity)
                            {
                                List<AddonProp> addonProps = new List<AddonProp>();
                                // Recherche des addons liés
                                foreach (Item item in AddonFusion.customItems.Select(i => i.Item).Where(i => i.spawnPrefab?.GetComponent<AddonProp>() != null))
                                {
                                    AddonProp addonProp = item.spawnPrefab.GetComponent<AddonProp>();
                                    if (customEphemeralItem.Item.spawnPrefab.GetComponent(addonProp.AddonType) != null && addonProp.CheckSpecificItem(customEphemeralItem.Item.spawnPrefab))
                                    {
                                        addonProps.Add(addonProp);
                                    }
                                }
                                // Affectation d'un addon lié aléatoire
                                if (addonProps.Count > 0)
                                {
                                    AddonProp addonProp = addonProps[new System.Random().Next(0, addonProps.Count - 1)];
                                    if (addonProp != null)
                                    {
                                        AddonFusionNetworkManager.Instance.SetAddonServerRpc(grabbableObject.GetComponent<NetworkObject>(), addonProp.itemProperties.itemName);
                                    }
                                }
                            }
                        }
                        iteratorEphemeralItem++;
                        if (iteratorEphemeralItem >= nbEphemeralItem)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private static GrabbableObject SpawnNewItem(ref RoundManager roundManager, Item itemToSpawn)
        {
            try
            {
                System.Random random = new System.Random();
                List<RandomScrapSpawn> listRandomScrapSpawn = UnityEngine.Object.FindObjectsOfType<RandomScrapSpawn>().Where(s => !s.spawnUsed).ToList();

                if (listRandomScrapSpawn.Count <= 0) return null;

                int indexRandomScrapSpawn = random.Next(0, listRandomScrapSpawn.Count);
                RandomScrapSpawn randomScrapSpawn = listRandomScrapSpawn[indexRandomScrapSpawn];
                if (randomScrapSpawn.spawnedItemsCopyPosition)
                {
                    randomScrapSpawn.spawnUsed = true;
                    listRandomScrapSpawn.RemoveAt(indexRandomScrapSpawn);
                }
                else
                {
                    randomScrapSpawn.transform.position = roundManager.GetRandomNavMeshPositionInBoxPredictable(randomScrapSpawn.transform.position, randomScrapSpawn.itemSpawnRange, roundManager.navHit, roundManager.AnomalyRandom) + Vector3.up * itemToSpawn.verticalOffset;
                }

                Vector3 position = randomScrapSpawn.transform.position + Vector3.up * 0.5f;
                return SpawnScrap(ref itemToSpawn.spawnPrefab, ref position);
            }
            catch (Exception arg)
            {
                AddonFusion.mls.LogError($"Error in SpawnNewItem: {arg}");
            }
            return null;
        }

        public static GrabbableObject SpawnScrap(ref GameObject spawnPrefab, ref Vector3 position)
        {
            if (GameNetworkManager.Instance.localPlayerController.IsServer || GameNetworkManager.Instance.localPlayerController.IsHost)
            {
                try
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate(spawnPrefab, position, Quaternion.identity, StartOfRound.Instance.propsContainer);
                    GrabbableObject scrap = gameObject.GetComponent<GrabbableObject>();
                    scrap.fallTime = 0f;
                    gameObject.GetComponent<NetworkObject>().Spawn();
                    return scrap;
                }
                catch (Exception arg)
                {
                    AddonFusion.mls.LogError($"Error in SpawnScrap: {arg}");
                }
            }
            return null;
        }
    }
}
