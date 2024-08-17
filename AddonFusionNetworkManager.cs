using AddonFusion.Behaviours;
using AddonFusion.Patches;
using GameNetcodeStuff;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion
{
    internal class AddonFusionNetworkManager : NetworkBehaviour
    {
        public static AddonFusionNetworkManager Instance;

        public void Awake()
        {
            Instance = this;
        }

        [ServerRpc(RequireOwnership = false)]
        public void DestroyObjectServerRpc(NetworkObjectReference obj)
        {
            DestroyObjectClientRpc(obj);
        }

        [ClientRpc]
        private void DestroyObjectClientRpc(NetworkObjectReference obj)
        {
            if (obj.TryGet(out var networkObject))
            {
                DestroyObject(networkObject.gameObject.GetComponentInChildren<GrabbableObject>());
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ItemExplodeServerRpc(NetworkObjectReference obj, Vector3 position)
        {
            ItemExplodeClientRpc(obj, position);
        }

        [ClientRpc]
        private void ItemExplodeClientRpc(NetworkObjectReference obj, Vector3 position)
        {
            if (obj.TryGet(out var networkObject))
            {
                DestroyObject(networkObject.gameObject.GetComponentInChildren<GrabbableObject>());
                Landmine.SpawnExplosion(position, spawnExplosionEffect: true, 5.7f, 6f);
            }
        }

        public void DestroyObject(GrabbableObject grabbableObject)
        {
            if (grabbableObject != null)
            {
                if (grabbableObject is FlashlightItem flashlight)
                {
                    if (flashlight.isBeingUsed)
                    {
                        flashlight.isBeingUsed = false;
                        flashlight.usingPlayerHelmetLight = false;
                        flashlight.flashlightBulbGlow.enabled = false;
                        flashlight.SwitchFlashlight(on: false);
                    }
                }
                grabbableObject.DestroyObjectInHand(grabbableObject.playerHeldBy);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void BlindPlayerServerRpc(int playerId, float flashFilter)
        {
            BlindPlayerClientRpc(playerId, flashFilter);
        }

        [ClientRpc]
        public void BlindPlayerClientRpc(int playerId, float flashFilter)
        {
            if (GameNetworkManager.Instance.localPlayerController == StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>()
                && HUDManager.Instance.flashFilter < flashFilter)
            {
                HUDManager.Instance.flashFilter = flashFilter;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerParryingServerRpc(int playerId, bool enable)
        {
            SetPlayerParryingClientRpc(playerId, enable);
        }

        [ClientRpc]
        private void SetPlayerParryingClientRpc(int playerId, bool enable)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>();
            PlayerAFBehaviour playerAFBehaviour = player.GetComponent<PlayerAFBehaviour>();
            if (player != null && playerAFBehaviour != null)
            {
                playerAFBehaviour.isParrying = enable;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ParryPlayerServerRpc(int playerId, int playerWhoHitId)
        {
            ParryPlayerClientRpc(playerId, playerWhoHitId);
        }

        [ClientRpc]
        public void ParryPlayerClientRpc(int playerId, int playerWhoHitId)
        {
            PlayerControllerB playerWhoHit = StartOfRound.Instance.allPlayerObjects[playerWhoHitId].GetComponent<PlayerControllerB>();
            if (playerWhoHit != GameNetworkManager.Instance.localPlayerController)
            {
                PlayerControllerB player = StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>();
                PlayerControllerBPatch.AddParriedPlayer(player, playerWhoHit);
                PlayerControllerBPatch.ParryEntity(ref player, false);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void StunPlayerServerRpc(int playerId, float stunTime, bool isItemDropped, bool isPlayerImmobilized)
        {
            if (isItemDropped || isPlayerImmobilized) StunPlayerClientRpc(playerId, stunTime, isItemDropped, isPlayerImmobilized);
        }

        [ClientRpc]
        public void StunPlayerClientRpc(int playerId, float stunTime, bool isItemDropped, bool isPlayerImmobilized)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>();
            if (isItemDropped
                && player.currentlyHeldObjectServer != null
                && player.currentlyHeldObjectServer is Shovel shovel)
            {
                shovel.reelingUp = false;
                if (shovel.reelingUpCoroutine != null)
                {
                    StopCoroutine(shovel.reelingUpCoroutine);
                    shovel.reelingUpCoroutine = null;
                }

                if (player == GameNetworkManager.Instance.localPlayerController)
                {
                    player.DiscardHeldObject();
                }
            }
            if (isPlayerImmobilized)
            {
                StartCoroutine(StunPlayer(player, stunTime));
            }
        }

        private IEnumerator StunPlayer(PlayerControllerB player, float stunTime)
        {
            GameObject stunEffect = Instantiate(AddonFusion.stunParticle, player.gameplayCamera.transform.position + Vector3.up * 0.1f + player.gameplayCamera.transform.right * -0.3f, player.transform.rotation);
            stunEffect.transform.SetParent(player.transform);
            if (player == GameNetworkManager.Instance.localPlayerController)
            {
                AFUtilities.EnablePlayerActions(false);
                player.disableLookInput = true;
                player.inSpecialInteractAnimation = true;
                player.snapToServerPosition = true;
            }

            yield return new WaitForSeconds(stunTime);

            if (player == GameNetworkManager.Instance.localPlayerController)
            {
                AFUtilities.EnablePlayerActions(true);
                player.disableLookInput = false;
                player.inSpecialInteractAnimation = false;
                player.snapToServerPosition = false;
            }
            Destroy(stunEffect);
        }

        [ServerRpc(RequireOwnership = false)]
        public void StunEnemyServerRpc(NetworkObjectReference enemyObject, float stunTime, int playerId)
        {
            StunEnemyClientRpc(enemyObject, stunTime, playerId);
        }

        [ClientRpc]
        public void StunEnemyClientRpc(NetworkObjectReference enemyObject, float stunTime, int playerId)
        {
            if (enemyObject.TryGet(out NetworkObject networkObject))
            {
                EnemyAI enemy = networkObject.gameObject.GetComponentInChildren<EnemyAI>();
                enemy?.SetEnemyStunned(setToStunned: true, stunTime, StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>());
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemoveSprayPaintDecalServerRpc(NetworkObjectReference obj, Vector3 position)
        {
            RemoveSprayPaintDecalClientRpc(obj, position);
        }

        [ClientRpc]
        private void RemoveSprayPaintDecalClientRpc(NetworkObjectReference obj, Vector3 position)
        {
            if (obj.TryGet(out var networkObject))
            {
                SprayPaintItem sprayPaintItem = networkObject.gameObject.GetComponentInChildren<SprayPaintItem>();
                if (sprayPaintItem != null)
                {
                    foreach (GameObject sprayPaintDecal in SprayPaintItem.sprayPaintDecals)
                    {
                        if (sprayPaintDecal.transform.position == position) sprayPaintDecal.SetActive(false);
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetAddonServerRpc(NetworkObjectReference obj, string addonName)
        {
            SetAddonClientRpc(obj, addonName);
        }

        [ClientRpc]
        public void SetAddonClientRpc(NetworkObjectReference obj, string addonName)
        {
            if (obj.TryGet(out var networkObject))
            {
                SetAddon(networkObject.gameObject.GetComponentInChildren<GrabbableObject>(), addonName);
            }
        }

        public void SetAddon(GrabbableObject grabbableObject, string addonName)
        {
            if (grabbableObject != null)
            {
                Addon addon = grabbableObject.gameObject.GetComponent<Addon>() ?? grabbableObject.gameObject.AddComponent<Addon>();
                addon.hasAddon = true;
                addon.addonName = addonName;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetEphemeralServerRpc(NetworkObjectReference obj, int maxUse)
        {
            SetEphemeralClientRpc(obj, maxUse);
        }

        [ClientRpc]
        public void SetEphemeralClientRpc(NetworkObjectReference obj, int maxUse)
        {
            if (obj.TryGet(out var networkObject))
            {
                SetEphemeralItem(networkObject.gameObject.GetComponentInChildren<GrabbableObject>(), maxUse);
            }
        }

        public void SetEphemeralItem(GrabbableObject grabbableObject, int maxUse)
        {
            if (grabbableObject != null)
            {
                grabbableObject.itemProperties.isScrap = false;
                grabbableObject.scrapValue = 0;
                EphemeralItem ephemeralItem = grabbableObject.gameObject.GetComponent<EphemeralItem>() ?? grabbableObject.gameObject.AddComponent<EphemeralItem>();
                ephemeralItem.isEphemeral = true;
                if (grabbableObject is FlashlightItem flashlight)
                {
                    flashlight.insertedBattery.charge = maxUse / 100f;
                }
                else if (grabbableObject is SprayPaintItem sprayPaintItem)
                {
                    sprayPaintItem.sprayCanTank = maxUse / 100f;
                }
                else if (grabbableObject is TetraChemicalItem tetraChemicalItem)
                {
                    tetraChemicalItem.fuel = maxUse / 100f;
                }
                else
                {
                    ephemeralItem.maxUse = maxUse;
                }
                if (grabbableObject.gameObject.GetComponentInChildren<ScanNodeProperties>() == null)
                {
                    GameObject gameObject2 = new("ScanNode", typeof(ScanNodeProperties), typeof(BoxCollider));
                    gameObject2.layer = LayerMask.NameToLayer("ScanNode");
                    gameObject2.transform.localScale = Vector3.one * 1f;
                    gameObject2.transform.parent = grabbableObject.gameObject.transform;
                    GameObject scanNodeObject = gameObject2;
                    scanNodeObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    ScanNodeProperties scanNode = scanNodeObject.GetComponent<ScanNodeProperties>();
                    scanNode.scrapValue = 0;
                    scanNode.creatureScanID = -1;
                    scanNode.nodeType = 0;
                    scanNode.minRange = 1;
                    scanNode.maxRange = 13;
                    scanNode.requiresLineOfSight = true;
                    scanNode.headerText = "Ephemeral " + grabbableObject.itemProperties.itemName;
                }
                else
                {
                    ScanNodeProperties scanNode = grabbableObject.gameObject.GetComponentInChildren<ScanNodeProperties>();
                    scanNode.scrapValue = 0;
                    scanNode.creatureScanID = -1;
                    scanNode.nodeType = 0;
                    scanNode.headerText = "Ephemeral " + grabbableObject.itemProperties.itemName;
                    scanNode.subText = null;
                }
            }
        }
    }
}
