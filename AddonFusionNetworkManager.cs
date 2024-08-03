using GameNetcodeStuff;
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
        public void StunPlayerServerRpc(int playerId)
        {
            StunPlayerClientRpc(playerId);
        }

        [ClientRpc]
        public void StunPlayerClientRpc(int playerId)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>();
            Instantiate(AddonFusion.stunParticle, player.gameplayCamera.transform.position + Vector3.up * 0.1f + Vector3.forward * 0.25f, player.transform.rotation);
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
    }
}
