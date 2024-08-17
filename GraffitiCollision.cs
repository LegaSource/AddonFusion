using AddonFusion.Patches;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion
{
    internal class GraffitiCollision : MonoBehaviour
    {
        private SprayPaintItem sprayPaintItem;
        private GameObject parentObject;

        public void Initialize(SprayPaintItem sprayPaintItem, GameObject gameObject)
        {
            this.sprayPaintItem = sprayPaintItem;
            parentObject = gameObject;
        }

        private void OnTriggerEnter(Collider other)
        {
            EnemyAICollisionDetect collisionDetect = other.GetComponent<EnemyAICollisionDetect>();
            if (SprayPaintItemPatch.IsDressGirlChasing(ref collisionDetect) || SprayPaintItemPatch.IsKittenjiEnemyChasing(ref collisionDetect))
            {
                BoxCollider collider = GetComponent<BoxCollider>();
                if (collider != null && collider.name.Equals("GraffitiCollision"))
                {
                    SprayPaintItemPatch.ManageCollision(collisionDetect.mainScript);
                }
                AddonFusionNetworkManager.Instance.RemoveSprayPaintDecalServerRpc(sprayPaintItem.GetComponent<NetworkObject>(), parentObject.transform.position);
            }
        }
    }
}
