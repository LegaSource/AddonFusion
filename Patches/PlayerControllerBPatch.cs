using AddonFusion.AddonValues;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class PlayerControllerBPatch
    {
        public static bool isParrying = false;
        public static bool isParryOnCooldown = false;
        public static EnemyAI parriedEnemy;

        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
        [HarmonyPostfix]
        private static void FixVehicleParent(ref GrabbableObject ___currentlyGrabbingObject)
        {
            // Limiter le fix au véhicule au cas où le parent serait nécessaire ailleur
            if (___currentlyGrabbingObject?.transform.parent?.GetComponent<VehicleController>() != null)
            {
                ___currentlyGrabbingObject.transform.SetParent(null);
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "ItemSecondaryUse_performed")]
        [HarmonyPostfix]
        private static void ItemSecondaryActivate(ref PlayerControllerB __instance)
        {
            if (__instance.currentlyHeldObjectServer != null && __instance.currentlyHeldObjectServer is Shovel shovel && !shovel.reelingUp)
            {
                __instance.StartCoroutine(ParryCoroutine(shovel));
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer))]
        [HarmonyPrefix]
        private static bool DamagePlayer(ref PlayerControllerB __instance)
        {
            if (isParrying && parriedEnemy != null && __instance.currentlyHeldObjectServer != null && __instance.currentlyHeldObjectServer is Shovel shovel)
            {
                ProtectiveCordValue protectiveCordValue = AddonFusion.protectiveCordValues.Where(v => v.EntityName.Equals(parriedEnemy.enemyType.enemyName)).FirstOrDefault()
                        ?? AddonFusion.protectiveCordValues.Where(v => v.EntityName.Equals("default")).FirstOrDefault();
                __instance.StartCoroutine(ParryCooldownCoroutine(protectiveCordValue.CooldownDuration));
                AddonFusionNetworkManager.Instance.StunEnemyServerRpc(parriedEnemy.NetworkObject, protectiveCordValue.StunDuration, (int)__instance.playerClientId);
                __instance.StartCoroutine(SpeedBoostCoroutine(__instance, protectiveCordValue.SpeedBoostDuration, protectiveCordValue.SpeedMultiplier / 100 + 1));
                __instance.sprintMeter += 1f * protectiveCordValue.StaminaRegen / 100;
                RoundManager.PlayRandomClip(shovel.shovelAudio, shovel.hitSFX);
                return false;
            }
            else
            {
                parriedEnemy = null;
                return true;
            }
        }

        private static IEnumerator ParryCoroutine(Shovel shovel)
        {
            if (isParryOnCooldown)
            {
                HUDManager.Instance.DisplayTip("Information", "On cooldown!");
            }
            else if (!isParrying)
            {
                isParrying = true;
                ReelingUpShovel(true, ref shovel);
                yield return new WaitForSeconds(ConfigManager.cordWindowDuration.Value);
                ReelingUpShovel(false, ref shovel);
                isParrying = false;
            }
        }

        public static void ReelingUpShovel(bool enable, ref Shovel shovel)
        {
            shovel.reelingUp = enable;
            shovel.playerHeldBy.activatingItem = enable;
            shovel.playerHeldBy.twoHanded = enable;
            shovel.playerHeldBy.playerBodyAnimator.SetBool("reelingUp", value: enable);
            if (enable)
            {
                if ((Coroutine)AccessTools.Field(typeof(Shovel), "reelingUpCoroutine").GetValue(shovel) != null)
                {
                    shovel.StopCoroutine((Coroutine)AccessTools.Field(typeof(Shovel), "reelingUpCoroutine").GetValue(shovel));
                }
                shovel.playerHeldBy.playerBodyAnimator.ResetTrigger("shovelHit");
                shovel.shovelAudio.PlayOneShot(shovel.reelUp);
                shovel.ReelUpSFXServerRpc();
            }
        }

        private static IEnumerator ParryCooldownCoroutine(float duration)
        {
            isParryOnCooldown = true;
            yield return new WaitForSeconds(duration);
            isParryOnCooldown = false;
        }

        private static IEnumerator SpeedBoostCoroutine(PlayerControllerB player, float duration, float speedMultiplier)
        {
            float sprintMultiplier = (float)AccessTools.Field(typeof(PlayerControllerB), "sprintMultiplier").GetValue(player);
            float speedBase = sprintMultiplier;
            sprintMultiplier *= speedMultiplier;
            AccessTools.Field(typeof(PlayerControllerB), "sprintMultiplier").SetValue(player, sprintMultiplier);
            yield return new WaitForSeconds(duration);
            AccessTools.Field(typeof(PlayerControllerB), "sprintMultiplier").SetValue(player, speedBase);
        }
    }
}
