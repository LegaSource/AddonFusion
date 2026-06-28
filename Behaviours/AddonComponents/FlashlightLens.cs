using AddonFusion.Behaviours.Scripts;
using AddonFusion.Managers;
using GameNetcodeStuff;
using LegaFusionCore.Managers.NetworkManagers;
using LegaFusionCore.Registries;
using LegaFusionCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static AddonFusion.Behaviours.Scripts.AddonTargetDatabase;

namespace AddonFusion.Behaviours.AddonComponents;

[AddonInfo(AddonTargetType.FLASHLIGHT)]
public class FlashlightLens : AddonComponent
{
    public override string AddonName => Constants.FLASHLIGHT_LENS;
    public override bool IsPassive => false;

    public Coroutine blindCoroutine;

    public override void ActivateAddonAbility()
    {
        if (grabbableObject is FlashlightItem && grabbableObject.TryGetComponent(out NetworkObject networkObject))
            AddonFusionNetworkManager.Instance.EnableAddonEveryoneRpc(networkObject, !isEnabled);
    }

    public void Update()
    {
        if (isEnabled && !onCooldown && grabbableObject && grabbableObject.isBeingUsed)
        {
            if (grabbableObject.insertedBattery.charge > 0f)
                grabbableObject.insertedBattery.charge -= Time.deltaTime / grabbableObject.itemProperties.batteryUsage;

            PlayerControllerB player = grabbableObject?.playerHeldBy;
            if (LFCUtilities.ShouldBeLocalPlayer(player) && blindCoroutine == null)
            {
                foreach (EnemyAI enemy in LFCSpawnRegistry.GetAllAs<EnemyAI>())
                {
                    if (CanBeBlinded(enemy))
                    {
                        blindCoroutine = StartCoroutine(BlindCoroutine(enemy));
                        return;
                    }
                }

                foreach (PlayerControllerB blindedPlayer in StartOfRound.Instance.allPlayerScripts)
                {
                    if (CanBeBlinded(blindedPlayer))
                    {
                        blindCoroutine = StartCoroutine(BlindCoroutine(blindedPlayer));
                        return;
                    }
                }
            }
        }
    }

    public bool CanBeBlinded<T>(T entity)
    {
        PlayerControllerB localPlayer = LFCUtilities.LocalPlayer;
        return localPlayer != null
            && (entity is PlayerControllerB player
                ? player.isPlayerControlled
                    && LFCUtilities.ShouldNotBeLocalPlayer(player)
                    && player.HasLineOfSightToPosition(localPlayer.playerEye.transform.position, 60f, 15)
                    && Mathf.Abs(Vector3.Angle(localPlayer.playerEye.transform.forward, player.playerEye.position - localPlayer.playerEye.transform.position)) < 15f
                : entity is EnemyAI enemy
                    && enemy.enemyType != null
                    && enemy.enemyType.canBeStunned
                    && enemy.eye != null
                    && enemy.CheckLineOfSightForPosition(localPlayer.playerEye.transform.position, 90f, 15)
                    && Mathf.Abs(Vector3.Angle(localPlayer.playerEye.transform.forward, enemy.eye.position - localPlayer.playerEye.transform.position)) < 15f);
    }

    public IEnumerator BlindCoroutine<T>(T entity)
    {
        Light[] lights = grabbableObject.GetComponentsInChildren<Light>();
        Dictionary<Light, float> defaultSpotAngles = new Dictionary<Light, float>(lights.Length);
        foreach (Light light in lights)
            defaultSpotAngles[light] = light.spotAngle;

        const float flashDuration = 3f;
        float timePassed = 0f;

        while (timePassed < flashDuration && CanBeBlinded(entity))
        {
            yield return new WaitForSeconds(0.1f);
            timePassed += 0.1f;

            float timeProrata = timePassed / flashDuration;
            foreach (Light light in lights)
                light.spotAngle = Mathf.Lerp(defaultSpotAngles[light], defaultSpotAngles[light] / 3f, timeProrata);
            if (entity is PlayerControllerB player)
                AddonFusionNetworkManager.Instance.BlindPlayerEveryoneRpc((int)player.playerClientId, timeProrata);
        }

        if (timePassed >= flashDuration)
        {
            foreach (Light light in lights)
                light.spotAngle = defaultSpotAngles[light] * 2f;
            if (entity is EnemyAI enemy)
                LFCNetworkManager.Instance.StunEnemyEveryoneRpc(enemy.NetworkObject, setToStunned: true, playerId: (int)grabbableObject.playerHeldBy.playerClientId);
            StartCooldown(ConfigManager.flashlightLensCooldown.Value);

            yield return new WaitForSeconds(0.1f);
        }

        foreach (Light light in lights)
            light.spotAngle = defaultSpotAngles[light];
        blindCoroutine = null;
    }
}
