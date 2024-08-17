using AddonFusion.AddonValues;
using AddonFusion.Behaviours;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class EnemyAIPatch
    {
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Start))]
        [HarmonyPostfix]
        private static void StartEnemy(ref EnemyAI __instance)
        {
            if (__instance.enemyType != null
                && __instance.enemyType.canBeStunned
                && __instance.eye != null
                && (string.IsNullOrEmpty(ConfigManager.lensExclusions.Value) || !ConfigManager.lensExclusions.Value.Contains(__instance.enemyType.enemyName)))
            {
                FlashlightItemPatch.blindableEnemies.Add(__instance);
            }
        }

        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Update))]
        [HarmonyPostfix]
        private static void UpdatePatch(ref EnemyAI __instance)
        {
            EnemyAFBehaviour enemyAFBehaviour = __instance.gameObject.GetComponent<EnemyAFBehaviour>();
            if (enemyAFBehaviour != null
                && enemyAFBehaviour.pyrethrinTankBehaviourIndex != -1
                && __instance.currentBehaviourStateIndex == enemyAFBehaviour.pyrethrinTankBehaviourIndex
                && enemyAFBehaviour.playerHitBy != null)
            {
                if (enemyAFBehaviour.isPyrethrinTankActive)
                {
                    __instance.ChangeOwnershipOfEnemy(enemyAFBehaviour.playerHitBy.actualClientId);
                    enemyAFBehaviour.StopAggressiveAI(__instance);
                }
                else
                {
                    enemyAFBehaviour.PyrethrinTankBehaviour(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.SwitchToBehaviourStateOnLocalClient))]
        [HarmonyPrefix]
        private static bool FixButlerBeesBehaviourState(ref EnemyAI __instance, ref int stateIndex)
        {
            if (__instance is ButlerBeesEnemyAI && stateIndex == -1)
            {
                __instance.currentBehaviourStateIndex = stateIndex;
                __instance.currentBehaviourState = null;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.OnCollideWithPlayer))]
        [HarmonyPostfix]
        private static void EnemyCollideWithPlayer(ref EnemyAI __instance, Collider other)
        {
            PlayerControllerB player = __instance.MeetsStandardPlayerCollisionConditions(other);
            if (player != null)
            {
                PlayerControllerBPatch.AddParriedEnemy(player, __instance);
            }
        }

        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.HitEnemyOnLocalClient))]
        [HarmonyPrefix]
        private static bool CriticalHit(ref EnemyAI __instance, ref int force, ref PlayerControllerB playerWhoHit)
        {
            Addon addon;
            if (playerWhoHit != null
                && playerWhoHit.currentlyHeldObjectServer != null
                && playerWhoHit.currentlyHeldObjectServer is KnifeItem knife
                && (addon = AFUtilities.GetAddonInstalled(knife, "Blade Sharpener")) != null)
            {
                EnemyAI enemy = __instance;
                BladeSharpenerValue bladeSharpenerValue = AddonFusion.bladeSharpenerValues.Where(v => v.EntityName.Equals(enemy.enemyType.enemyName)).FirstOrDefault()
                        ?? AddonFusion.bladeSharpenerValues.Where(v => v.EntityName.Equals("default")).FirstOrDefault();
                int chance = Random.Range(0, 100);
                if (chance < bladeSharpenerValue.CriticalSuccessChance)
                {
                    force = __instance.enemyHP;
                }
                else if (chance < bladeSharpenerValue.CriticalSuccessChance + bladeSharpenerValue.CriticalFailChance)
                {
                    playerWhoHit.DiscardHeldObject();
                    return false;
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.HitEnemy))]
        [HarmonyPostfix]
        private static void HitEphemeralItem(ref PlayerControllerB playerWhoHit)
        {
            EphemeralItem ephemeralItem;
            if (playerWhoHit != null
                && playerWhoHit.currentlyHeldObjectServer != null
                && (ephemeralItem = AFUtilities.GetEphemeralItem(playerWhoHit.currentlyHeldObjectServer)) != null)
            {
                ephemeralItem.use++;
            }
        }

        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.KillEnemy))]
        [HarmonyPostfix]
        private static void EndEnemy(ref EnemyAI __instance)
        {
            FlashlightItemPatch.blindableEnemies.Remove(__instance);
        }

        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.SetEnemyStunned))]
        [HarmonyPostfix]
        private static void PostSetEnemyStunned(ref EnemyAI __instance)
        {
            if (!__instance.isEnemyDead && __instance.enemyType.canBeStunned)
            {
                EnemyAI enemy = __instance;
                LensValue lensValue = AddonFusion.lensValues.Where(v => v.EntityName.Equals(enemy.enemyType.enemyName)).FirstOrDefault()
                    ?? AddonFusion.lensValues.Where(v => v.EntityName.Equals("default")).FirstOrDefault();
                __instance.StartCoroutine(ImmuneCoroutine(__instance, lensValue.ImmunityDuration));
            }
        }

        private static IEnumerator ImmuneCoroutine(EnemyAI enemy, float immunityDuration)
        {
            FlashlightItemPatch.blindableEnemies.Remove(enemy);
            yield return new WaitForSeconds(immunityDuration);
            FlashlightItemPatch.blindableEnemies.Add(enemy);
        }
    }
}
