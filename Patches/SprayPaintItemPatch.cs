using AddonFusion.AddonValues;
using AddonFusion.Behaviours;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class SprayPaintItemPatch
    {
        public static List<EnemyAI> immunedEnemies = new List<EnemyAI>();
        public static Dictionary<EnemyAI, int> enemiesChanceToSwitchHauntingPlayer = new Dictionary<EnemyAI, int>();

        [HarmonyPatch(typeof(SprayPaintItem), nameof(SprayPaintItem.LateUpdate))]
        [HarmonyPrefix]
        private static void DestroyEphemeralSpray(ref SprayPaintItem __instance)
        {
            if (__instance.IsOwner
                && AFUtilities.GetEphemeralItem(__instance) != null
                && __instance.sprayCanTank <= 0f)
            {
                AddonFusionNetworkManager.Instance.DestroyObjectServerRpc(__instance.GetComponent<NetworkObject>());
            }
        }

        [HarmonyPatch(typeof(SprayPaintItem), "TrySprayingWeedKillerBottle")]
        [HarmonyPrefix]
        private static bool HitEnemyWithWeedKiller(ref SprayPaintItem __instance)
        {
            if (Physics.Raycast(new Ray(__instance.playerHeldBy.gameplayCamera.transform.position, __instance.playerHeldBy.gameplayCamera.transform.forward), out RaycastHit hit, 4.5f, 524288, QueryTriggerInteraction.Collide))
            {
                EnemyAICollisionDetect collisionDetect = hit.collider.GetComponent<EnemyAICollisionDetect>();
                if (collisionDetect != null)
                {
                    Debug.Log("Collision avec " + collisionDetect.mainScript.enemyType.enemyName);
                    if (AddonFusion.pyrethrinTankValues.Select(e => e.EntityName).Contains(collisionDetect.mainScript.enemyType.enemyName))
                    {
                        collisionDetect.mainScript.SwitchToBehaviourState(99);
                    }
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(SprayPaintItem), "AddSprayPaintLocal")]
        [HarmonyPostfix]
        private static void AddColliderSprayPaint(ref SprayPaintItem __instance, ref bool __result, ref List<GameObject> ___sprayPaintDecals, ref int ___sprayPaintDecalsIndex, ref RaycastHit ___sprayHit)
        {
            if (__result)
            {
                GameObject gameObject = ___sprayPaintDecals[___sprayPaintDecalsIndex];
                if (gameObject.GetComponent<Collider>() == null)
                {
                    BoxCollider collider = gameObject.AddComponent<BoxCollider>();
                    if (AFUtilities.GetAddonInstalled(__instance, "Salt Tank") != null)
                    {
                        collider.name = "GraffitiCollision";
                    }
                    else
                    {
                        collider.name = "GraffitiCollisionNoAddon";
                    }
                    collider.size = new Vector3(4f, 4f, 4f);
                    collider.center = new Vector3(0f, 0f, 0f);
                    GraffitiCollision graffitiCollision = gameObject.AddComponent<GraffitiCollision>();
                    graffitiCollision.Initialize(__instance, gameObject);
                }

                if (Physics.Raycast(new Ray(__instance.playerHeldBy.gameplayCamera.transform.position, __instance.playerHeldBy.gameplayCamera.transform.forward), out RaycastHit hit, 10f, 524288, QueryTriggerInteraction.Collide))
                {
                    EnemyAICollisionDetect collisionDetect = hit.collider.GetComponent<EnemyAICollisionDetect>();
                    if (IsDressGirlChasing(ref collisionDetect) || IsHerobrineChasing(ref collisionDetect))
                    {
                        ManageCollision(collisionDetect.mainScript);
                    }
                }
            }
        }

        public static bool IsDressGirlChasing(ref EnemyAICollisionDetect collisionDetect)
        {
            if (collisionDetect != null
                && collisionDetect.mainScript is DressGirlAI girl
                && girl.currentBehaviourStateIndex == 1
                && GameNetworkManager.Instance.localPlayerController == girl.hauntingPlayer)
            {
                return true;
            }
            return false;
        }

        public static bool IsHerobrineChasing(ref EnemyAICollisionDetect collisionDetect)
        {
            Type herobrineType = Type.GetType("Kittenji.HerobrineMod.HerobrineAI, HerobrineMod");
            if (collisionDetect!= null
                && herobrineType != null
                && herobrineType.IsInstanceOfType(collisionDetect.mainScript)
                && collisionDetect.mainScript.currentBehaviourStateIndex == 2
                && GameNetworkManager.Instance.localPlayerController == (PlayerControllerB)herobrineType.GetField("hauntingPlayer").GetValue(collisionDetect.mainScript))
            {
                return true;
            }
            return false;
        }

        public static void ManageCollision(EnemyAI enemy)
        {
            if (!immunedEnemies.Contains(enemy))
            {
                if (!enemiesChanceToSwitchHauntingPlayer.Any(e => e.Key.Equals(enemy)))
                {
                    SaltTankValue saltTankValue = AddonFusion.saltTankValues.Where(v => v.EntityName.Equals(enemy.enemyType.enemyName)).FirstOrDefault()
                        ?? AddonFusion.saltTankValues.Where(v => v.EntityName.Equals("default")).FirstOrDefault();
                    enemiesChanceToSwitchHauntingPlayer.Add(enemy, saltTankValue.BaseChance);
                }
                enemy.StartCoroutine(ManageCollisionCoroutine(enemy));
            }
        }

        public static IEnumerator ManageCollisionCoroutine(EnemyAI enemy)
        {
            bool stopChasing = false;
            Type herobrineType = Type.GetType("Kittenji.HerobrineMod.HerobrineAI, HerobrineMod");
            SaltTankValue saltTankValue = AddonFusion.saltTankValues.Where(v => v.EntityName.Equals(enemy.enemyType.enemyName)).FirstOrDefault()
                ?? AddonFusion.saltTankValues.Where(v => v.EntityName.Equals("default")).FirstOrDefault();
            immunedEnemies.Add(enemy);

            if (herobrineType != null && herobrineType.IsInstanceOfType(enemy))
            {
                herobrineType.GetMethod("SetEnemyVisible").Invoke(enemy, [false]);
            }
            else
            {
                enemy.EnableEnemyMesh(false, true);
            }

            KeyValuePair<EnemyAI, int> enemyChance = enemiesChanceToSwitchHauntingPlayer.FirstOrDefault(e => e.Key.Equals(enemy));
            if (new System.Random().Next(1, 100) <= enemyChance.Value)
            {
                stopChasing = true;
                if (enemiesChanceToSwitchHauntingPlayer.ContainsKey(enemyChance.Key))
                {
                    enemiesChanceToSwitchHauntingPlayer[enemyChance.Key] = saltTankValue.BaseChance;
                }

                if (herobrineType != null && herobrineType.IsInstanceOfType(enemy))
                {
                    herobrineType.GetMethod("SwitchState", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(enemy, [1]);
                    if (UnityEngine.Random.Range(0, 100) < 50)
                    {
                        herobrineType.GetMethod("RequestNewTargetServerRpc", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(enemy, null);
                    }
                }
                else if (enemy is DressGirlAI girl)
                {
                    girl.StopChasing();
                    girl.ChooseNewHauntingPlayerClientRpc();
                }
            }

            yield return new WaitForSeconds(0.25f);

            immunedEnemies.Remove(enemy);
            if (!stopChasing)
            {
                if (enemiesChanceToSwitchHauntingPlayer.ContainsKey(enemyChance.Key))
                {
                    enemiesChanceToSwitchHauntingPlayer[enemyChance.Key] += saltTankValue.AdditionalChance;
                }
                if (herobrineType != null && herobrineType.IsInstanceOfType(enemy))
                {
                    herobrineType.GetMethod("SetEnemyVisible").Invoke(enemy, [true]);
                }
                else
                {
                    enemy.EnableEnemyMesh(true, true);
                }
            }
        }
    }
}
