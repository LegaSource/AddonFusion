using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Unity.Netcode;

namespace AddonFusion.Patches
{
    internal class RoundManagerPatch
    {
        public static List<Item> itemsToSpawn = new List<Item>();

        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.SpawnScrapInLevel))]
        [HarmonyPostfix]
        private static void SpawnScraps(ref RoundManager __instance)
        {
            AddNewItems();
            SpawnNewItems(ref __instance);
        }

        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.DetectElevatorIsRunning))]
        [HarmonyPostfix]
        private static void EndGame()
        {
            FlashlightItemPatch.blindableEnemies.Clear();
        }

        private static void AddNewItems()
        {
            foreach (Item item in StartOfRound.Instance.allItemsList.itemsList)
            {
                CustomItem customItem = AddonFusion.customItems.FirstOrDefault(i => i.IsSpawnable && i.Item == item);
                if (customItem != null && new System.Random().Next(1, 100) <= customItem.Rarity)
                {
                    itemsToSpawn.Add(item);
                }
            }
        }

        private static void SpawnNewItems(ref RoundManager __instance)
        {
            try
            {
                System.Random random = new System.Random();
                List<RandomScrapSpawn> listRandomScrapSpawn = UnityEngine.Object.FindObjectsOfType<RandomScrapSpawn>().Where(s => !s.spawnUsed).ToList();
                foreach (Item itemToSpawn in itemsToSpawn)
                {
                    if (listRandomScrapSpawn.Count <= 0) break;

                    int indexRandomScrapSpawn = random.Next(0, listRandomScrapSpawn.Count);
                    RandomScrapSpawn randomScrapSpawn = listRandomScrapSpawn[indexRandomScrapSpawn];
                    if (randomScrapSpawn.spawnedItemsCopyPosition)
                    {
                        randomScrapSpawn.spawnUsed = true;
                        listRandomScrapSpawn.RemoveAt(indexRandomScrapSpawn);
                    }
                    else
                    {
                        randomScrapSpawn.transform.position = __instance.GetRandomNavMeshPositionInBoxPredictable(randomScrapSpawn.transform.position, randomScrapSpawn.itemSpawnRange, __instance.navHit, __instance.AnomalyRandom) + Vector3.up * itemToSpawn.verticalOffset;
                    }

                    Vector3 position = randomScrapSpawn.transform.position + Vector3.up * 0.5f;
                    SpawnScrap(ref itemToSpawn.spawnPrefab, ref position);
                }
            }
            catch (Exception arg)
            {
                Debug.LogError($"Error in SpawnNewItems: {arg}");
            }
        }

        public static void SpawnScrap(ref GameObject spawnPrefab, ref Vector3 position)
        {
            if (GameNetworkManager.Instance.localPlayerController.IsServer || GameNetworkManager.Instance.localPlayerController.IsHost)
            {
                try
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate(spawnPrefab, position, Quaternion.identity, StartOfRound.Instance.propsContainer);
                    GrabbableObject scrap = gameObject.GetComponent<GrabbableObject>();
                    scrap.fallTime = 0f;
                    gameObject.GetComponent<NetworkObject>().Spawn();
                }
                catch (Exception arg)
                {
                    Debug.LogError($"Error in SpawnScrap: {arg}");
                }
            }
        }
    }
}
