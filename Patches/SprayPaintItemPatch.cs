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
            if (AFUtilities.GetAddonInstalled(__instance, "Pyrethrin Tank") != null
                && Physics.Raycast(new Ray(__instance.playerHeldBy.gameplayCamera.transform.position, __instance.playerHeldBy.gameplayCamera.transform.forward), out RaycastHit hit, 4.5f, 524288, QueryTriggerInteraction.Collide))
            {
                EnemyAICollisionDetect collisionDetect = hit.collider.GetComponent<EnemyAICollisionDetect>();
                if (collisionDetect != null)
                {
                    if (AddonFusion.pyrethrinTankValues.Select(e => e.EntityName).Contains(collisionDetect.mainScript.enemyType.enemyName))
                    {
                        EnemyAFBehaviour enemyAFBehaviour = collisionDetect.mainScript.GetComponent<EnemyAFBehaviour>();
                        if (enemyAFBehaviour != null
                            && !enemyAFBehaviour.isPyrethrinTankActive
                            && enemyAFBehaviour.pyrethrinTankBehaviourIndex != -1)
                        {
                            enemyAFBehaviour.playerHitBy = __instance.playerHeldBy;
                            collisionDetect.mainScript.SwitchToBehaviourState(enemyAFBehaviour.pyrethrinTankBehaviourIndex);
                        }
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
                    if (IsDressGirlChasing(ref collisionDetect) || IsKittenjiEnemyChasing(ref collisionDetect))
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

        public static bool IsKittenjiEnemyChasing(ref EnemyAICollisionDetect collisionDetect)
        {
            Type[] enemyTypes = { Type.GetType("Kittenji.HerobrineMod.HerobrineAI, HerobrineMod"),
                Type.GetType("Kittenji.FootballEntity.FootballAI, FootballEntity") };
            foreach (Type enemyType in enemyTypes)
            {
                if (collisionDetect != null
                    && enemyType != null
                    && enemyType.IsInstanceOfType(collisionDetect.mainScript)
                    && collisionDetect.mainScript.currentBehaviourStateIndex == 2
                    && GameNetworkManager.Instance.localPlayerController == (PlayerControllerB)enemyType.GetField("hauntingPlayer").GetValue(collisionDetect.mainScript))
                {
                    return true;
                }
            }
            return false;
        }

        public static void ManageCollision(EnemyAI enemy)
        {
            EnemyAFBehaviour enemyAFBehaviour = enemy.GetComponent<EnemyAFBehaviour>();
            if (enemyAFBehaviour != null && !enemyAFBehaviour.isSaltImmuned)
            {
                SaltTankValue saltTankValue = AddonFusion.saltTankValues.Where(v => v.EntityName.Equals(enemy.enemyType.enemyName)).FirstOrDefault()
                    ?? AddonFusion.saltTankValues.Where(v => v.EntityName.Equals("default")).FirstOrDefault();
                enemyAFBehaviour.chanceToSwitchHauntingPlayer = saltTankValue.BaseChance;
                enemy.StartCoroutine(ManageCollisionCoroutine(enemy, enemyAFBehaviour));
            }
        }

        public static IEnumerator ManageCollisionCoroutine(EnemyAI enemy, EnemyAFBehaviour enemyAFBehaviour)
        {
            bool stopChasing = false;
            Type enemyType = enemy.GetType();
            SaltTankValue saltTankValue = AddonFusion.saltTankValues.FirstOrDefault(v => v.EntityName.Equals(enemy.enemyType.enemyName))
                ?? AddonFusion.saltTankValues.FirstOrDefault(v => v.EntityName.Equals("default"));

            enemyAFBehaviour.isSaltImmuned = true;

            Action<bool> setEnemyVisibility = CreateVisibilitySetter(enemyType, enemy);
            setEnemyVisibility?.Invoke(false);

            if (new System.Random().Next(1, 100) <= enemyAFBehaviour.chanceToSwitchHauntingPlayer)
            {
                stopChasing = true;
                enemyAFBehaviour.chanceToSwitchHauntingPlayer = saltTankValue.BaseChance;

                Action switchStateAndRequestNewTarget = CreateStateSwitcher(enemyType, enemy);
                switchStateAndRequestNewTarget?.Invoke();
            }

            yield return new WaitForSeconds(0.25f);

            enemyAFBehaviour.isSaltImmuned = false;
            if (!stopChasing)
            {
                enemyAFBehaviour.chanceToSwitchHauntingPlayer += saltTankValue.AdditionalChance;
                setEnemyVisibility?.Invoke(true);
            }
        }

        private static Action<bool> CreateVisibilitySetter(Type enemyType, EnemyAI enemy)
        {
            if (enemyType == null) return null;

            MethodInfo setVisibilityMethod = null;
            if (enemyType.Name.Equals("HerobrineAI") || enemyType.Name.Equals("FootballAI"))
            {
                setVisibilityMethod = enemyType.GetMethod("SetEnemyVisible");
            }

            if (setVisibilityMethod != null)
            {
                return (isVisible) => setVisibilityMethod.Invoke(enemy, [isVisible]);
            }
            else
            {
                return (isVisible) => enemy.EnableEnemyMesh(isVisible, true);
            }
        }

        private static Action CreateStateSwitcher(Type enemyType, EnemyAI enemy)
        {
            if (enemyType == null) return null;

            MethodInfo switchStateMethod = null;
            MethodInfo requestNewTargetMethod = null;
            if (enemyType.Name.Equals("HerobrineAI") || enemyType.Name.Equals("FootballAI"))
            {
                switchStateMethod = enemyType.GetMethod("SwitchState", BindingFlags.NonPublic | BindingFlags.Instance);
                requestNewTargetMethod = enemyType.GetMethod("RequestNewTargetServerRpc", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            if (switchStateMethod != null)
            {
                return () =>
                {
                    switchStateMethod.Invoke(enemy, [1]);
                    if (UnityEngine.Random.Range(0, 100) < 50 && requestNewTargetMethod != null)
                    {
                        requestNewTargetMethod.Invoke(enemy, null);
                    }
                };
            }
            else if (enemy is DressGirlAI girl)
            {
                return () =>
                {
                    girl.StopChasing();
                    girl.ChooseNewHauntingPlayerClientRpc();
                };
            }
            return null;
        }
    }
}
