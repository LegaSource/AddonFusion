using AddonFusion.AddonValues;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class FlashlightItemPatch
    {
        internal static List<EnemyAI> blindableEnemies = new List<EnemyAI>();
        private static bool isFlashing = false;
        private static float maxSpotAngle;

        [HarmonyPatch(typeof(FlashlightItem), nameof(FlashlightItem.Start))]
        [HarmonyPostfix]
        private static void StartPatch(ref FlashlightItem __instance)
        {
            maxSpotAngle = __instance.flashlightBulb.spotAngle;
        }

        [HarmonyPatch(typeof(FlashlightItem), nameof(FlashlightItem.Update))]
        [HarmonyPostfix]
        private static void StunFlash(ref FlashlightItem __instance)
        {
            Addon addon = __instance.GetComponent<Addon>();
            if ((addon = AFUtilities.GetAddonInstalled(__instance, "Lens")) != null
                && !__instance.itemProperties.itemName.Equals("Laser pointer")
                && __instance.playerHeldBy != null
                && __instance.playerHeldBy == GameNetworkManager.Instance.localPlayerController
                && __instance.isBeingUsed
                && !isFlashing)
            {
                FlashlightItem flashlightItem = __instance;
                foreach (EnemyAI enemy in blindableEnemies.Where(e => e.enemyType != null && e.enemyType.canBeStunned && e.eye != null))
                {
                    LensValue lensValue = AddonFusion.lensValues.Where(v => v.EntityName.Equals(enemy.enemyType.enemyName)).FirstOrDefault()
                        ?? AddonFusion.lensValues.Where(v => v.EntityName.Equals("default")).FirstOrDefault();
                    if (lensValue != null)
                    {
                        float flashDuration = lensValue.FlashDuration;
                        float stunDuration = lensValue.StunDuration;
                        float lightAngle = lensValue.LightAngle;
                        float angle = lensValue.EntityAngle;
                        int distance = lensValue.EntityDistance;
                        float batteryConsumption = lensValue.BatteryConsumption;

                        if (MeetConditions(enemy, ref flashlightItem, lightAngle, angle, distance))
                        {
                            __instance.StartCoroutine(StunCoroutine(flashlightItem, enemy, flashDuration, lightAngle, angle, distance, batteryConsumption, stunDuration));
                            break;
                        }
                    }
                    else
                    {
                        AddonFusion.mls.LogInfo($"No default value for the 'Flashlight Lens Values' field in the config for {enemy.enemyType.enemyName}.");
                    }
                }

                LensValue lensValuePlayer = AddonFusion.lensValues.Where(v => v.EntityName.Equals("Player")).FirstOrDefault();
                if (lensValuePlayer != null && !isFlashing)
                {
                    foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts.Where(p => p.isPlayerControlled && p != flashlightItem.playerHeldBy))
                    {
                        float flashDuration = lensValuePlayer.FlashDuration;
                        float lightAngle = lensValuePlayer.LightAngle;
                        float angle = lensValuePlayer.EntityAngle;
                        int distance = lensValuePlayer.EntityDistance;
                        float batteryConsumption = lensValuePlayer.BatteryConsumption;

                        if (MeetConditions(player, ref flashlightItem, lightAngle, angle, distance))
                        {
                            __instance.StartCoroutine(StunCoroutine(flashlightItem, player, flashDuration, lightAngle, angle, distance, batteryConsumption));
                            break;
                        }
                    }
                }

                if (__instance.flashlightBulb.spotAngle != maxSpotAngle)
                {
                    __instance.flashlightBulb.spotAngle = maxSpotAngle;
                }
            }
        }

        private static IEnumerator StunCoroutine<T>(FlashlightItem flashlightItem, T entity, float flashDuration, float lightAngle, float angle, int distance, float batteryConsumption, float stunDuration = 0f)
        {
            float timePassed = 0f;
            float minSpotAngle = maxSpotAngle / 3f;

            while (MeetConditions(entity, ref flashlightItem, lightAngle, angle, distance))
            {
                yield return new WaitForSeconds(0.1f);
                timePassed += 0.1f;
                flashlightItem.flashlightBulb.spotAngle = Mathf.Lerp(maxSpotAngle, minSpotAngle, timePassed / flashDuration);
                if (entity is PlayerControllerB player) AddonFusionNetworkManager.Instance.BlindPlayerServerRpc((int)player.playerClientId, timePassed / flashDuration);
                if (timePassed >= flashDuration) break;
            }

            if (isFlashing)
            {
                flashlightItem.flashlightBulb.spotAngle = maxSpotAngle * 2;
                if (entity is EnemyAI enemy) AddonFusionNetworkManager.Instance.StunEnemyServerRpc(enemy.NetworkObject, stunDuration, (int)flashlightItem.playerHeldBy.playerClientId);
                if (batteryConsumption > 0f) flashlightItem.insertedBattery.charge = flashlightItem.insertedBattery.charge * batteryConsumption / 100;
                yield return new WaitForSeconds(0.1f);
                isFlashing = false;
            }
            flashlightItem.flashlightBulb.spotAngle = maxSpotAngle;
        }

        private static bool MeetConditions<T>(T entity, ref FlashlightItem flashlightItem, float lightAngle, float angle, int distance)
        {
            if (flashlightItem.isBeingUsed
                && ((entity is PlayerControllerB player
                    && player.HasLineOfSightToPosition(flashlightItem.flashlightBulb.transform.position + Vector3.up * 0.4f, angle, distance)
                    && Mathf.Abs(Vector3.Angle(flashlightItem.flashlightBulb.transform.forward, player.playerEye.position - flashlightItem.flashlightBulb.transform.position)) < lightAngle
                    && Mathf.Abs(Vector3.Angle(player.gameplayCamera.transform.forward, -1 * (player.playerEye.position - flashlightItem.flashlightBulb.transform.position + Vector3.up * 0.4f))) <= 40f)
                    || (entity is EnemyAI enemy
                        && enemy.CheckLineOfSightForPosition(flashlightItem.flashlightBulb.transform.position, angle, distance)
                        && Mathf.Abs(Vector3.Angle(flashlightItem.flashlightBulb.transform.forward, enemy.eye.position - flashlightItem.flashlightBulb.transform.position)) < lightAngle)))
            {
                isFlashing = true;
                return true;
            }
            flashlightItem.flashlightBulb.spotAngle = maxSpotAngle;
            isFlashing = false;
            return false;
        }
    }
}
