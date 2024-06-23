using AddonFusion.AddonValues;
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
                __instance.StartCoroutine(ImmuneCoroutine(__instance, lensValue.ImmunityTime));
            }
        }

        private static IEnumerator ImmuneCoroutine(EnemyAI enemy, float immunityTime)
        {
            FlashlightItemPatch.blindableEnemies.Remove(enemy);
            yield return new WaitForSeconds(immunityTime);
            FlashlightItemPatch.blindableEnemies.Add(enemy);
        }
    }
}
