using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class TetraChemicalItemPatch
    {
        private static float healthRegen = 0f;

        [HarmonyPatch(typeof(TetraChemicalItem), nameof(TetraChemicalItem.Update))]
        [HarmonyPrefix]
        private static void DestroyEphemeralTZP(ref TetraChemicalItem __instance)
        {
            if (__instance.IsOwner
                && AFUtilities.GetEphemeralItem(__instance) != null
                && __instance.fuel <= 0f)
            {
                AddonFusionNetworkManager.Instance.DestroyObjectServerRpc(__instance.GetComponent<NetworkObject>());
            }
        }

        [HarmonyPatch(typeof(TetraChemicalItem), nameof(TetraChemicalItem.Update))]
        [HarmonyPostfix]
        private static void TetraChemicalActivate(ref TetraChemicalItem __instance, ref bool ___emittingGas)
        {
            if (__instance.isHeld
                && __instance.playerHeldBy == GameNetworkManager.Instance.localPlayerController
                && AFUtilities.GetAddonInstalled(__instance, "Senzu") != null)
            {
                if (___emittingGas)
                {
                    healthRegen += Time.deltaTime / ConfigManager.senzuHealthRegenDuration.Value * 100;
                    if (healthRegen >= 1f)
                    {
                        int healthRegenRounded = (int)healthRegen;
                        healthRegen -= healthRegenRounded;
                        __instance.playerHeldBy.health = Mathf.Min(__instance.playerHeldBy.health + healthRegenRounded, 100);
                        HUDManager.Instance.UpdateHealthUI(__instance.playerHeldBy.health, hurtPlayer: false);
                        __instance.playerHeldBy.DamagePlayerClientRpc(-healthRegenRounded, __instance.playerHeldBy.health);
                        if (__instance.playerHeldBy.criticallyInjured && __instance.playerHeldBy.health >= 10)
                        {
                            __instance.playerHeldBy.MakeCriticallyInjured(enable: false);
                        }
                    }
                    __instance.playerHeldBy.sprintMeter = Mathf.Min(__instance.playerHeldBy.sprintMeter + Time.deltaTime / ConfigManager.senzuStaminaRegenDuration.Value, 1f);
                }
                else
                {
                    healthRegen = 0f;
                }
            }
        }
    }
}
