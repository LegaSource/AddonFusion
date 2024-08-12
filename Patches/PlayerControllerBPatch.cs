using AddonFusion.AddonValues;
using AddonFusion.Behaviours;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class PlayerControllerBPatch
    {
        public static bool isParrying = false;
        public static bool isParryOnCooldown = false;
        public static EnemyAI parriedEnemy;

        public static List<PlayerControllerB> revivablePlayers = new List<PlayerControllerB>();

        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
        [HarmonyPostfix]
        private static void FixVehicleParent(ref GrabbableObject ___currentlyGrabbingObject)
        {
            // Limiter le fix au véhicule au cas où le parent serait nécessaire ailleur
            if (___currentlyGrabbingObject == null)
            {
                return;
            }
            if (___currentlyGrabbingObject.transform == null)
            {
                return;
            }
            if (___currentlyGrabbingObject.transform.parent == null)
            {
                return;
            }
            if (___currentlyGrabbingObject.transform.parent.GetComponent<VehicleController>() != null)
            {
                ___currentlyGrabbingObject.transform.SetParent(null);
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "ItemSecondaryUse_performed")]
        [HarmonyPostfix]
        private static void ItemSecondaryActivate(ref PlayerControllerB __instance)
        {
            ShovelSecondaryActivate(ref __instance);
            SenzuSecondaryActivate(ref __instance);
        }

        private static void ShovelSecondaryActivate(ref PlayerControllerB player)
        {
            if (player == GameNetworkManager.Instance.localPlayerController
                && player.currentlyHeldObjectServer != null
                && player.currentlyHeldObjectServer is Shovel shovel
                && !shovel.reelingUp
                && AFUtilities.GetAddonInstalled(shovel, "Protective Cord") != null)
            {
                player.StartCoroutine(ParryCoroutine(shovel));
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer))]
        [HarmonyPrefix]
        private static bool DamagePlayer(ref PlayerControllerB __instance)
        {
            if (isParrying && parriedEnemy != null && __instance.currentlyHeldObjectServer != null && __instance.currentlyHeldObjectServer is Shovel)
            {
                ProtectiveCordValue protectiveCordValue = AddonFusion.protectiveCordValues.Where(v => v.EntityName.Equals(parriedEnemy.enemyType.enemyName)).FirstOrDefault()
                        ?? AddonFusion.protectiveCordValues.Where(v => v.EntityName.Equals("default")).FirstOrDefault();
                __instance.StartCoroutine(ParryCooldownCoroutine(protectiveCordValue.CooldownDuration));
                AddonFusionNetworkManager.Instance.StunEnemyServerRpc(parriedEnemy.NetworkObject, protectiveCordValue.StunDuration, (int)__instance.playerClientId);
                __instance.StartCoroutine(SpeedBoostCoroutine(__instance, protectiveCordValue.SpeedBoostDuration, protectiveCordValue.SpeedMultiplier / 100 + 1));
                __instance.sprintMeter += 1f * protectiveCordValue.StaminaRegen / 100;
                GameObject audioObject = Object.Instantiate(AddonFusion.parrySound, __instance.transform.position, Quaternion.identity);
                AudioSource audioSource = audioObject.GetComponent<AudioSource>();
                Object.Destroy(audioObject, audioSource.clip.length);
                return false;
            }
            else
            {
                parriedEnemy = null;
                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc")]
        [HarmonyPostfix]
        private static void PostKillPlayerClientRpc(ref PlayerControllerB __instance)
        {
            if (__instance.isPlayerDead && __instance.deadBody != null)
            {
                __instance.StartCoroutine(ReviveCoroutine(__instance));
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
                yield return new WaitForSeconds(ConfigManager.cordSpamCooldown.Value);
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
                if (shovel.reelingUpCoroutine != null)
                {
                    shovel.StopCoroutine(shovel.reelingUpCoroutine);
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
            float speedBase = player.sprintMultiplier;
            player.sprintMultiplier *= speedMultiplier;
            yield return new WaitForSeconds(duration);
            player.sprintMultiplier = speedBase;
        }

        private static void SenzuSecondaryActivate(ref PlayerControllerB player)
        {
            if (player == GameNetworkManager.Instance.localPlayerController
                && player.currentlyHeldObjectServer != null
                && player.currentlyHeldObjectServer is Senzu)
            {
                int healthBefore = player.health;
                player.health = 100;
                HUDManager.Instance.UpdateHealthUI(player.health, hurtPlayer: false);
                player.DamagePlayerClientRpc((player.health - healthBefore) * -1, player.health);
                if (player.criticallyInjured)
                {
                    player.MakeCriticallyInjured(enable: false);
                }
                player.sprintMeter = 1f;
                AddonFusionNetworkManager.Instance.DestroyObjectServerRpc(player.currentlyHeldObjectServer.GetComponent<NetworkObject>());
            }
        }

        private static IEnumerator ReviveCoroutine(PlayerControllerB player)
        {
            if (!revivablePlayers.Contains(player)) revivablePlayers.Add(player);
            float elapsedTime = 0f;
            float checkInterval = 1f;
            while (elapsedTime < ConfigManager.senzuReviveDuration.Value)
            {
                if (!revivablePlayers.Contains(player)) yield break;
                yield return new WaitForSeconds(checkInterval);
                elapsedTime += checkInterval;
            }
            revivablePlayers.Remove(player);
        }
    }
}
