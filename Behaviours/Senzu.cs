using GameNetcodeStuff;
using System;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Behaviours
{
    internal class Senzu : AddonProp
    {
        public override Type AddonType => typeof(TetraChemicalItem);

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null)
            {
                if (Physics.Raycast(new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward), out RaycastHit hit, 3f, 832))
                {
                    DeadBodyInfo deadBodyInfo = hit.transform.GetComponentInParent<DeadBodyInfo>();
                    if (deadBodyInfo?.playerScript != null)
                    {
                        PlayerAFBehaviour playerAFBehaviour = deadBodyInfo.playerScript.GetComponent<PlayerAFBehaviour>();
                        if (playerAFBehaviour != null && playerAFBehaviour.isRevivable)
                        {
                            Vector3 position = deadBodyInfo.transform.position;
                            RagdollGrabbableObject ragdollGrabbableObject = deadBodyInfo.GetComponentInChildren<RagdollGrabbableObject>();
                            if (ragdollGrabbableObject != null)
                            {
                                position = ragdollGrabbableObject.transform.position;
                            }
                            RevivePlayerServerRpc((int)deadBodyInfo?.playerScript.playerClientId, position + Vector3.up * 0.5f);
                        }
                        else
                        {
                            HUDManager.Instance.DisplayTip("Impossible action", "This player can no longer be revived.");
                        }
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void RevivePlayerServerRpc(int playerId, Vector3 position)
        {
            RevivePlayerClientRpc(playerId, position);
        }

        [ClientRpc]
        private void RevivePlayerClientRpc(int playerId, Vector3 position)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>();
            if (player != null)
            {
                RevivePlayer(ref player, position);
                DestroyObjectInHand(playerHeldBy);
            }
        }

        public void RevivePlayer(ref PlayerControllerB player, Vector3 position)
        {
            int index = Array.IndexOf(StartOfRound.Instance.allPlayerScripts, player);
            PlayerAFBehaviour playerAFBehaviour = player.GetComponent<PlayerAFBehaviour>();
            if (playerAFBehaviour != null) playerAFBehaviour.isRevivable = false;
            player.ResetPlayerBloodObjects(player.isPlayerDead);
            player.isClimbingLadder = false;
            player.clampLooking = false;
            player.inVehicleAnimation = false;
            player.disableMoveInput = false;
            player.ResetZAndXRotation();
            player.thisController.enabled = true;
            player.health = 100;
            player.disableLookInput = false;
            player.disableInteract = false;
            player.isPlayerDead = false;
            player.isPlayerControlled = true;
            player.isInElevator = playerHeldBy.isInElevator;
            player.isInHangarShipRoom = playerHeldBy.isInHangarShipRoom;
            player.isInsideFactory = playerHeldBy.isInsideFactory;
            player.parentedToElevatorLastFrame = playerHeldBy.parentedToElevatorLastFrame;
            player.overrideGameOverSpectatePivot = null;
            if (player == GameNetworkManager.Instance.localPlayerController)
            {
                StartOfRound.Instance.SetPlayerObjectExtrapolate(enable: false);
            }
            player.TeleportPlayer(position);
            player.setPositionOfDeadPlayer = false;
            player.DisablePlayerModel(StartOfRound.Instance.allPlayerObjects[index], enable: true, disableLocalArms: true);
            player.helmetLight.enabled = false;
            player.Crouch(crouch: false);
            player.criticallyInjured = false;
            player.playerBodyAnimator?.SetBool("Limp", value: false);
            player.bleedingHeavily = false;
            player.activatingItem = false;
            player.twoHanded = false;
            player.inShockingMinigame = false;
            player.inSpecialInteractAnimation = false;
            player.freeRotationInInteractAnimation = false;
            player.disableSyncInAnimation = false;
            player.inAnimationWithEnemy = null;
            player.holdingWalkieTalkie = false;
            player.speakingToWalkieTalkie = false;
            player.isSinking = false;
            player.isUnderwater = false;
            player.sinkingValue = 0f;
            player.statusEffectAudio.Stop();
            player.DisableJetpackControlsLocally();
            player.mapRadarDotAnimator.SetBool("dead", value: false);
            player.externalForceAutoFade = Vector3.zero;
            if (player == GameNetworkManager.Instance.localPlayerController)
            {
                HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", value: false);
                player.hasBegunSpectating = false;
                HUDManager.Instance.RemoveSpectateUI();
                HUDManager.Instance.gameOverAnimator.SetTrigger("revive");
                player.hinderedMultiplier = 1f;
                player.isMovementHindered = 0;
                player.sourcesCausingSinking = 0;
                player.reverbPreset = StartOfRound.Instance.shipReverb;
                SoundManager.Instance.earsRingingTimer = 0f;
                HUDManager.Instance.UpdateHealthUI(100, hurtPlayer: false);
                HUDManager.Instance.HideHUD(hide: false);
                HUDManager.Instance.audioListenerLowPass.enabled = false;
                player.spectatedPlayerScript = null;
            }
            player.voiceMuffledByEnemy = false;
            SoundManager.Instance.playerVoicePitchTargets[index] = 1f;
            SoundManager.Instance.SetPlayerPitch(1f, index);
            if (player.currentVoiceChatIngameSettings == null)
            {
                StartOfRound.Instance.RefreshPlayerVoicePlaybackObjects();
            }
            if (player.currentVoiceChatIngameSettings != null)
            {
                if (player.currentVoiceChatIngameSettings.voiceAudio == null)
                {
                    player.currentVoiceChatIngameSettings.InitializeComponents();
                }
                if (player.currentVoiceChatIngameSettings.voiceAudio == null)
                {
                    return;
                }
                player.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
            }

            RagdollGrabbableObject[] ragdollGrabbableObjects = FindObjectsOfType<RagdollGrabbableObject>();
            for (int j = 0; j < ragdollGrabbableObjects.Length; j++)
            {
                if (!ragdollGrabbableObjects[j].isHeld)
                {
                    if (IsServer)
                    {
                        if (ragdollGrabbableObjects[j].NetworkObject.IsSpawned)
                        {
                            ragdollGrabbableObjects[j].NetworkObject.Despawn();
                        }
                        else
                        {
                            Destroy(ragdollGrabbableObjects[j].gameObject);
                        }
                    }
                }
                else if (ragdollGrabbableObjects[j].isHeld && ragdollGrabbableObjects[j].playerHeldBy != null)
                {
                    ragdollGrabbableObjects[j].playerHeldBy.DropAllHeldItems();
                }
            }
            Destroy(player.deadBody.gameObject);
            StartOfRound.Instance.livingPlayers = StartOfRound.Instance.connectedPlayersAmount + 1;
            StartOfRound.Instance.allPlayersDead = false;
            StartOfRound.Instance.UpdatePlayerVoiceEffects();
        }
    }
}
