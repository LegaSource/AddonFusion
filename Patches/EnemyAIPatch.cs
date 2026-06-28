using AddonFusion.Behaviours.AddonComponents;
using GameNetcodeStuff;
using HarmonyLib;
using LegaFusionCore.Managers.NetworkManagers;
using LegaFusionCore.Utilities;
using UnityEngine;

namespace AddonFusion.Patches;

public class EnemyAIPatch
{
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.MeetsStandardPlayerCollisionConditions))]
    [HarmonyPostfix]
    public static void ProtectiveCordParry(EnemyAI __instance, ref PlayerControllerB __result)
    {
        if (__result != null
            && __result.currentlyHeldObjectServer != null
            && AFUtilities.TryGetAddonComponent(__result.currentlyHeldObjectServer, out ProtectiveCord protectiveCord)
            && protectiveCord.isParrying)
        {
            LFCNetworkManager.Instance.StunEnemyEveryoneRpc(__instance.NetworkObject, setToStunned: true, playerId: (int)__result.playerClientId);
            LFCNetworkManager.Instance.PlayAudioEveryoneRpc($"{AddonFusion.modName}{AddonFusion.parryAudio.name}", __result.currentlyHeldObjectServer.transform.position);
            __result = null;
        }
    }

    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.HitEnemyOnLocalClient))]
    [HarmonyPrefix]
    public static bool BladeSharpenerHit(EnemyAI __instance, ref int force, PlayerControllerB playerWhoHit)
    {
        if (playerWhoHit != null
            && playerWhoHit.currentlyHeldObjectServer != null
            && AFUtilities.TryGetAddonComponent(playerWhoHit.currentlyHeldObjectServer, out BladeSharpener _)
            && Random.Range(1, 101) < 5)
        {
            force = __instance.enemyHP;
        }
        return true;
    }

    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.HitEnemy))]
    [HarmonyPrefix]
    public static bool SaltTankHit(EnemyAI __instance, PlayerControllerB playerWhoHit)
    {
        if (playerWhoHit != null
            && playerWhoHit.currentlyHeldObjectServer != null
            && AFUtilities.TryGetAddonComponent(playerWhoHit.currentlyHeldObjectServer, out SaltTank _))
        {
            if (__instance is DressGirlAI dressGirlAI)
            {
                dressGirlAI.StopChasing();
                if (LFCUtilities.IsServer)
                    dressGirlAI.ChooseNewHauntingPlayerClientRpc();
            }
            // TODO: Autres enemis
        }
        return true;
    }
}
