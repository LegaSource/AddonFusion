using AddonFusion.AddonValues;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class SprayPaintItemPatch
    {
        public static List<EnemyAI> immunedEnemies = new List<EnemyAI>();
        public static Dictionary<EnemyAI, int> enemiesChanceToSwitchHauntingPlayer = new Dictionary<EnemyAI, int>();

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

                    Addon addon = __instance.GetComponent<Addon>();
                    if (addon != null && !string.IsNullOrEmpty(addon.addonName) && addon.addonName.Equals("Salt Tank"))
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
                    ManageCollision(collisionDetect.mainScript);
                }
            }
        }

        public static bool IsDressGirlChasing(ref EnemyAICollisionDetect collisionDetect)
        {
            if (collisionDetect!= null
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
                && GameNetworkManager.Instance.localPlayerController == (PlayerControllerB)AccessTools.Field(herobrineType, "hauntingPlayer").GetValue(collisionDetect.mainScript))
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
                herobrineType.GetMethod("SetEnemyVisible").Invoke(enemy, new object[] { false });
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
                    herobrineType.GetMethod("SwitchState", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(enemy, new object[] { 1 });
                    if (UnityEngine.Random.Range(0, 100) < 50)
                    {
                        herobrineType.GetMethod("RequestNewTargetServerRpc", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(enemy, null);
                    }
                }
                else if (enemy is DressGirlAI girl)
                {
                    AccessTools.Method(typeof(DressGirlAI), "StopChasing").Invoke(girl, null);
                    AccessTools.Method(typeof(DressGirlAI), "ChooseNewHauntingPlayerClientRpc").Invoke(girl, null);
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
                    herobrineType.GetMethod("SetEnemyVisible").Invoke(enemy, new object[] { true });
                }
                else
                {
                    enemy.EnableEnemyMesh(true, true);
                }
            }
        }
    }
}
