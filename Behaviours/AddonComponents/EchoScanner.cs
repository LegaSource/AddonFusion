using AddonFusion.Behaviours.Scripts;
using AddonFusion.Managers;
using GameNetcodeStuff;
using LegaFusionCore.Managers;
using LegaFusionCore.Utilities;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static AddonFusion.Behaviours.Scripts.AddonTargetDatabase;

namespace AddonFusion.Behaviours.AddonComponents;

[AddonInfo(AddonTargetType.BOOMBOX)]
public class EchoScanner : AddonComponent
{
    public override string AddonName => Constants.ECHO_SCANNER;
    public override bool IsPassive => false;

    private readonly Collider[] overlapBuffer = new Collider[64];
    public readonly float AoERadius = 15f;
    public readonly int AoEMask = 832 | 1084754248;
    public readonly float AuraCooldown = 2f;

    public float auraTimer = 0f;
    public bool canReveal = true;

    public override void ActivateAddonAbility()
    {
        if (grabbableObject is BoomboxItem && grabbableObject.TryGetComponent(out NetworkObject networkObject))
            AddonFusionNetworkManager.Instance.EnableAddonEveryoneRpc(networkObject, !isEnabled);
    }

    public void Update()
    {
        LFCUtilities.UpdateTimer(ref auraTimer, AuraCooldown, !canReveal, () =>
        {
            canReveal = true;
            LFCCustomPassManager.RemoveAuraByTag($"{AddonFusion.modName}{gameObject}");
        });

        if (isEnabled && grabbableObject is BoomboxItem boombox && boombox.isBeingUsed)
        {
            if (boombox.insertedBattery.charge > 0f)
                boombox.insertedBattery.charge -= Time.deltaTime / boombox.itemProperties.batteryUsage;

            if (canReveal)
            {
                canReveal = false;
                HashSet<GameObject> targets = [];
                int count = Physics.OverlapSphereNonAlloc(boombox.transform.position, AoERadius, overlapBuffer, AoEMask, QueryTriggerInteraction.Collide);

                for (int i = 0; i < count; i++)
                {
                    GameObject gameObject = GetValidTarget(overlapBuffer[i], boombox.isHeld);
                    if (gameObject != null)
                        _ = targets.Add(gameObject);
                }

                if (targets.Count > 0)
                {
                    Color color = boombox.isHeld ? Color.yellow : Color.red;
                    LFCCustomPassManager.SetupAuraForObjects(targets.ToArray(), LegaFusionCore.LegaFusionCore.wallhackShader, $"{AddonFusion.modName}{gameObject}", color);
                }
            }
        }
    }

    public GameObject GetValidTarget(Collider collider, bool isHeld)
    {
        if (collider != null)
        {
            if (isHeld && collider.gameObject.TryGetComponent(out GrabbableObject grabbableObject))
                return grabbableObject.gameObject;
            if (isHeld && collider.gameObject.TryGetComponent(out PlayerControllerB player) && !player.isPlayerDead && LFCUtilities.ShouldNotBeLocalPlayer(player))
                return player.gameObject;
            if (!isHeld && collider.gameObject.TryGetComponent(out EnemyAICollisionDetect collision) && collision.mainScript != null)
                return collision.mainScript.gameObject;
        }
        return null;
    }
}
