using AddonFusion.AddonValues;
using AddonFusion.Behaviours;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class PlayerControllerBPatch
    {
        public static bool hitReturn = true;

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.ConnectClientToPlayerObject))]
        [HarmonyPostfix]
        private static void StartPlayer(ref PlayerControllerB __instance)
        {
            if (__instance.isPlayerControlled && __instance.GetComponent<PlayerAFBehaviour>() == null)
            {
                __instance.gameObject.AddComponent<PlayerAFBehaviour>();
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
        [HarmonyPostfix]
        private static void FixVehicleParent(ref GrabbableObject ___currentlyGrabbingObject)
        {
            // Limiter le fix au véhicule au cas où le parent serait nécessaire ailleur
            if (___currentlyGrabbingObject == null) return;
            if (___currentlyGrabbingObject.transform == null) return;
            if (___currentlyGrabbingObject.transform.parent == null) return;
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
                shovel.previousPlayerHeldBy = player;
                player.StartCoroutine(ParryCoroutine(shovel));
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer))]
        [HarmonyPrefix]
        private static bool DamagePlayer(ref PlayerControllerB __instance)
        {
            return !ParryEntity(ref __instance, true);
        }

        public static void AddParriedEnemy(PlayerControllerB player, EnemyAI enemyAI)
        {
            PlayerAFBehaviour playerAFBehaviour = player.GetComponent<PlayerAFBehaviour>();
            if (playerAFBehaviour != null && (string.IsNullOrEmpty(ConfigManager.cordExclusions.Value) || !ConfigManager.cordExclusions.Value.Contains(enemyAI.enemyType.enemyName)))
            {
                if (playerAFBehaviour.isParrying) playerAFBehaviour.parriedEnemy = enemyAI;
                else playerAFBehaviour.parriedEnemy = null;
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "IHittable.Hit")]
        [HarmonyPrefix]
        private static bool HitPlayer(ref PlayerControllerB __instance, ref bool __result, PlayerControllerB playerWhoHit)
        {
            AddParriedPlayer(__instance, playerWhoHit);
            if (ParryEntity(ref __instance, false))
            {
                AddonFusionNetworkManager.Instance.ParryPlayerServerRpc((int)__instance.playerClientId, (int)playerWhoHit.playerClientId);
                __result = false;
                return false;
            }
            return true;
        }

        public static void AddParriedPlayer(PlayerControllerB player, PlayerControllerB parriedPlayer)
        {
            PlayerAFBehaviour playerAFBehaviour = player.GetComponent<PlayerAFBehaviour>();
            if (playerAFBehaviour != null && (string.IsNullOrEmpty(ConfigManager.cordExclusions.Value) || !ConfigManager.cordExclusions.Value.Contains("Player")))
            {
                if (playerAFBehaviour.isParrying) playerAFBehaviour.parriedPlayer = parriedPlayer;
                else playerAFBehaviour.parriedPlayer = null;
            }
        }

        public static bool ParryEntity(ref PlayerControllerB player, bool isEnemy)
        {
            PlayerAFBehaviour playerAFBehaviour = player.GetComponent<PlayerAFBehaviour>();
            if (playerAFBehaviour != null
                && playerAFBehaviour.isParrying
                && (playerAFBehaviour.parriedEnemy != null || playerAFBehaviour.parriedPlayer != null)
                && player.currentlyHeldObjectServer != null
                && player.currentlyHeldObjectServer is Shovel)
            {
                if (player == GameNetworkManager.Instance.localPlayerController)
                {
                    ProtectiveCordValue protectiveCordValue;
                    if (isEnemy) protectiveCordValue = AddonFusion.protectiveCordValues.Where(v => v.EntityName.Equals(playerAFBehaviour.parriedEnemy.enemyType.enemyName)).FirstOrDefault();
                    else protectiveCordValue = AddonFusion.protectiveCordValues.Where(v => v.EntityName.Equals("Player")).FirstOrDefault();
                    protectiveCordValue ??= AddonFusion.protectiveCordValues.Where(v => v.EntityName.Equals("default")).FirstOrDefault();

                    player.StartCoroutine(ParryCooldownCoroutine(playerAFBehaviour, protectiveCordValue.CooldownDuration));
                    
                    if (isEnemy) AddonFusionNetworkManager.Instance.StunEnemyServerRpc(playerAFBehaviour.parriedEnemy.NetworkObject, protectiveCordValue.StunDuration, (int)player.playerClientId);
                    else AddonFusionNetworkManager.Instance.StunPlayerServerRpc((int)playerAFBehaviour.parriedPlayer.playerClientId, protectiveCordValue.StunDuration, ConfigManager.canCordStunDropItem.Value, ConfigManager.canCordStunImmobilize.Value);

                    player.StartCoroutine(SpeedBoostCoroutine(player, protectiveCordValue.SpeedBoostDuration, protectiveCordValue.SpeedMultiplier / 100 + 1));
                    player.sprintMeter += 1f * protectiveCordValue.StaminaRegen / 100;

                    GameObject audioObject = Object.Instantiate(AddonFusion.parrySound, player.transform.position, Quaternion.identity);
                    AudioSource audioSource = audioObject.GetComponent<AudioSource>();
                    Object.Destroy(audioObject, audioSource.clip.length);
                }
                playerAFBehaviour.parriedEnemy = null;
                playerAFBehaviour.parriedPlayer = null;
                return true;
            }
            else
            {
                playerAFBehaviour.parriedEnemy = null;
                playerAFBehaviour.parriedPlayer = null;
                return false;
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
            PlayerAFBehaviour playerAFBehaviour = GameNetworkManager.Instance.localPlayerController.GetComponent<PlayerAFBehaviour>();
            if (playerAFBehaviour != null )
            {
                if (playerAFBehaviour.isParryOnCooldown)
                {
                    HUDManager.Instance.DisplayTip("Information", "On cooldown!");
                }
                else if (!playerAFBehaviour.isParrying)
                {
                    AddonFusionNetworkManager.Instance.SetPlayerParryingServerRpc((int)GameNetworkManager.Instance.localPlayerController.playerClientId, true);
                    ReelingUpShovel(true, ref shovel);
                    yield return new WaitForSeconds(ConfigManager.cordWindowDuration.Value);
                    ReelingUpShovel(false, ref shovel);
                    yield return new WaitForSeconds(ConfigManager.cordSpamCooldown.Value);
                    AddonFusionNetworkManager.Instance.SetPlayerParryingServerRpc((int)GameNetworkManager.Instance.localPlayerController.playerClientId, false);
                }
            }
        }

        public static void ReelingUpShovel(bool enable, ref Shovel shovel)
        {
            shovel.reelingUp = enable;
            shovel.previousPlayerHeldBy.activatingItem = enable;
            shovel.previousPlayerHeldBy.twoHanded = enable;
            shovel.previousPlayerHeldBy.playerBodyAnimator.SetBool("reelingUp", value: enable);
            if (enable)
            {
                if (shovel.reelingUpCoroutine != null)
                {
                    shovel.StopCoroutine(shovel.reelingUpCoroutine);
                }
                shovel.previousPlayerHeldBy.playerBodyAnimator.ResetTrigger("shovelHit");
                shovel.shovelAudio.PlayOneShot(shovel.reelUp);
                shovel.ReelUpSFXServerRpc();
            }
        }

        private static IEnumerator ParryCooldownCoroutine(PlayerAFBehaviour playerAFBehaviour, float duration)
        {
            playerAFBehaviour.isParryOnCooldown = true;
            yield return new WaitForSeconds(duration);
            playerAFBehaviour.isParryOnCooldown = false;
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
            PlayerAFBehaviour playerAFBehaviour = player.GetComponent<PlayerAFBehaviour>();
            if (playerAFBehaviour != null)
            {
                playerAFBehaviour.isRevivable = true;
                float elapsedTime = 0f;
                float checkInterval = 1f;
                while (elapsedTime < ConfigManager.senzuReviveDuration.Value)
                {
                    if (!playerAFBehaviour.isRevivable) yield break;
                    yield return new WaitForSeconds(checkInterval);
                    elapsedTime += checkInterval;
                }
                playerAFBehaviour.isRevivable = false;
            }
            else
            {
                yield break;
            }
        }
    }
}
