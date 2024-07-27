using AddonFusion.AddonValues;
using AddonFusion.Patches;
using HarmonyLib;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Behaviours
{
    internal class CapsuleHoiPoi : PhysicsProp
    {
        public Component component;
        public Animator[] componentAnimators;
        public Renderer[] componentRenderers;
        public Collider[] componentColliders;
        public ParticleSystem SmokeParticle;
        public AudioSource capsulePoof;

        public override void Start()
        {
            base.Start();
            if (SmokeParticle == null)
            {
                SmokeParticle = GetComponentInChildren<ParticleSystem>();
            }
            if (SmokeParticle == null)
            {
                AddonFusion.mls.LogError("SmokeParticle is not assigned and could not be found in children.");
            }

            if (capsulePoof == null)
            {
                capsulePoof = GetComponent<AudioSource>();
            }
            if (capsulePoof == null)
            {
                AddonFusion.mls.LogError("capsulePoof is not assigned and could not be found.");
            }
        }

        public override void Update()
        {
            base.Update();
            if (ConfigManager.isCapsuleCharge.Value && component != null && component is GrabbableObject grabbableObject)
            {
                CapsuleHoiPoiValue capsuleHoiPoiValue = AddonFusion.capsuleHoiPoiValues.Where(c => c.ItemName.Equals(grabbableObject.itemProperties?.itemName)).FirstOrDefault()
                        ?? AddonFusion.capsuleHoiPoiValues.Where(v => v.ItemName.Equals("default")).FirstOrDefault();
                if (grabbableObject.itemProperties.requiresBattery)
                {
                    grabbableObject.insertedBattery.charge = Mathf.Min(grabbableObject.insertedBattery.charge + Time.deltaTime / capsuleHoiPoiValue.ChargeTime, 1f);
                    grabbableObject.insertedBattery.empty = false;
                }
                else if (grabbableObject is SprayPaintItem sprayPaintItem)
                {
                    UpdateChargeValue(sprayPaintItem, "sprayCanTank", capsuleHoiPoiValue.ChargeTime);
                }
                else if (grabbableObject is TetraChemicalItem tetraChemicalItem)
                {
                    UpdateChargeValue(tetraChemicalItem, "fuel", capsuleHoiPoiValue.ChargeTime);
                }
            }
        }

        private void UpdateChargeValue<T>(T obj, string fieldName, float chargeTime)
        {
            float value = (float)AccessTools.Field(typeof(T), fieldName).GetValue(obj);
            value = Mathf.Min(value + Time.deltaTime / chargeTime, 1f);
            AccessTools.Field(typeof(T), fieldName).SetValue(obj, value);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null)
            {
                Component hitComponent;
                if (Physics.Raycast(new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward), out RaycastHit hit, 3f, 832)
                    && (hitComponent = (Component)hit.transform.GetComponentInParent<VehicleController>() ?? hit.transform.GetComponent<GrabbableObject>()) != null
                    && !hitComponent.name.Contains("CapsuleHoiPoi"))
                {
                    if (component != null) HUDManager.Instance.DisplayTip("Information", "The capsule is not empty.");
                    else SetComponentServerRpc(hitComponent.GetComponent<NetworkObject>());
                }
                else if (component != null)
                {
                    ThrowCapsuleServerRpc();
                }
                else
                {
                    HUDManager.Instance.DisplayTip("Information", "The capsule is empty.");
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetComponentServerRpc(NetworkObjectReference itemObject)
        {
            SetComponentClientRpc(itemObject);
        }

        [ClientRpc]
        private void SetComponentClientRpc(NetworkObjectReference itemObject)
        {
            if (itemObject.TryGet(out NetworkObject networkObject))
            {
                Component hitComponent = (Component)networkObject.gameObject.GetComponentInChildren<VehicleController>() ?? networkObject.gameObject.GetComponentInChildren<GrabbableObject>();
                if (hitComponent is VehicleController vehicle)
                {
                    if (vehicle.physicsRegion.physicsTransform.GetComponentsInChildren<GrabbableObject>().Any(g => !(g is ClipboardItem))
                        || vehicle.currentDriver != null
                        || vehicle.currentPassenger != null)
                    {
                        if (IsOwner) HUDManager.Instance.DisplayTip("Information", "The vehicle needs to be empty.");
                        return;
                    }
                    vehicle.SetIgnition(started: false);
                }
                if (hitComponent is GrabbableObject grabbableObject)
                {
                    if (!StormyWeatherPatch.conductiveObjectsToRemove.Contains(grabbableObject)) StormyWeatherPatch.conductiveObjectsToRemove.Add(grabbableObject);
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
                }
                componentAnimators = networkObject.GetComponentsInChildren<Animator>().Where(a => a.enabled).ToArray();
                componentRenderers = networkObject.GetComponentsInChildren<Renderer>().Where(r => r.enabled).ToArray();
                componentColliders = networkObject.GetComponentsInChildren<Collider>().Where(c => c.enabled).ToArray();
                foreach (AudioSource audio in networkObject.gameObject.GetComponentsInChildren<AudioSource>())
                {
                    audio.Stop();
                }
                foreach (ParticleSystem particle in networkObject.gameObject.GetComponentsInChildren<ParticleSystem>())
                {
                    particle.Stop();
                }
                foreach (Light light in networkObject.gameObject.GetComponentsInChildren<Light>())
                {
                    light.enabled = false;
                }
                component = hitComponent;
                StoreComponentInCapsule(true);
            }
        }

        public void StoreComponentInCapsule(bool enable)
        {
            // Désactivation des composants d'animations, de visuels et des colliders
            foreach (Animator animator in componentAnimators)
            {
                animator.enabled = !enable;
            }
            foreach (Renderer renderer in componentRenderers)
            {
                renderer.enabled = !enable;
            }
            foreach (Collider collider in componentColliders)
            {
                collider.enabled = !enable;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ThrowCapsuleServerRpc()
        {
            ThrowCapsuleClientRpc();
        }

        [ClientRpc]
        private void ThrowCapsuleClientRpc()
        {
            Vector3 initialPosition = CalculateThrowPosition(new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward));
            Vector3 finalPosition = AdjustPositionWithGroundCheck(initialPosition);
            playerHeldBy.DiscardHeldObject(placeObject: true, null, finalPosition);
            StartCoroutine(RespawnCoroutine(component));
        }

        private Vector3 CalculateThrowPosition(Ray throwRay)
        {
            // Effectuer le premier raycast pour déterminer la position initiale de l'objet lancé
            if (Physics.Raycast(throwRay, out RaycastHit hit, 15f, 832, QueryTriggerInteraction.Ignore))
            {
                return throwRay.GetPoint(hit.distance - 0.05f);
            }
            else
            {
                return throwRay.GetPoint(10f);
            }
        }

        private Vector3 AdjustPositionWithGroundCheck(Vector3 position)
        {
            // Effectuer un second raycast vers le bas pour ajuster la position par rapport au sol
            Ray downRay = new Ray(position, Vector3.down);
            if (Physics.Raycast(downRay, out RaycastHit hit, 30f, 832, QueryTriggerInteraction.Ignore))
            {
                return hit.point + Vector3.up * 0.05f;
            }
            else
            {
                return downRay.GetPoint(30f);
            }
        }

        private IEnumerator RespawnCoroutine(Component component)
        {
            yield return new WaitForSeconds(1.5f);

            SmokeParticle.Play();
            capsulePoof.Play();
            if (component is VehicleController vehicle)
            {
                VehicleControllerPatch.isImmune = true;
                vehicle.transform.position = transform.position + Vector3.up * 2.5f;
                vehicle.transform.rotation = Quaternion.identity;
                StartCoroutine(ImmuneCoroutine());
            }
            else if (component is GrabbableObject grabbableObject)
            {
                StormyWeatherPatch.conductiveObjectsToAdd.Add(grabbableObject);
                grabbableObject.transform.position = transform.position + Vector3.up;
                grabbableObject.startFallingPosition = grabbableObject.transform.position;
                if (grabbableObject.transform.parent != null)
                {
                    grabbableObject.startFallingPosition = grabbableObject.transform.parent.InverseTransformPoint(grabbableObject.startFallingPosition);
                }
                grabbableObject.FallToGround();
            }
            StoreComponentInCapsule(false);
            this.component = null;
            componentAnimators = null;
            componentRenderers = null;
            componentColliders = null;
        }

        private IEnumerator ImmuneCoroutine()
        {
            yield return new WaitForSeconds(0.2f);
            VehicleControllerPatch.isImmune = false;
        }
    }
}
