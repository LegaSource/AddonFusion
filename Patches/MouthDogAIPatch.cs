using GameNetcodeStuff;
using HarmonyLib;

namespace AddonFusion.Patches
{
    internal class MouthDogAIPatch
    {
        [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.KillPlayerClientRpc))]
        [HarmonyPrefix]
        private static bool MouthDogKillPlayer(ref MouthDogAI __instance, int playerId)
        {
            PlayerControllerB killPlayer = StartOfRound.Instance.allPlayerScripts[playerId];
            PlayerControllerBPatch.AddParriedEnemy(killPlayer, __instance);
            if (PlayerControllerBPatch.ParryEntity(ref killPlayer, true))
            {
                killPlayer.inAnimationWithEnemy = null;
                if (GameNetworkManager.Instance.localPlayerController.IsServer) __instance.inKillAnimation = false;
                return false;
            }
            return true;
        }
    }
}
