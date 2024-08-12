using AddonFusion.AddonValues;
using AddonFusion.Patches;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Behaviours
{
    internal class CapsuleHoiPoi : PhysicsProp
    {
        public Component component;
        public float repairModuleDuration = 0f;
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
            if (component != null && component is GrabbableObject grabbableObject)
            {
                if (ConfigManager.isCapsuleCharge.Value
                    && AFUtilities.GetEphemeralItem(grabbableObject) == null)
                {
                    CapsuleHoiPoiValue capsuleHoiPoiValue = AddonFusion.capsuleHoiPoiValues.Where(c => c.ItemName.Equals(grabbableObject.itemProperties?.itemName)).FirstOrDefault()
                        ?? AddonFusion.capsuleHoiPoiValues.Where(v => v.ItemName.Equals("default")).FirstOrDefault();
                    if (grabbableObject.itemProperties.requiresBattery)
                    {
                        grabbableObject.insertedBattery.charge = UpdateChargeValue(grabbableObject.insertedBattery.charge, capsuleHoiPoiValue.ChargeTime);
                        grabbableObject.insertedBattery.empty = false;
                    }
                    else if (grabbableObject is SprayPaintItem sprayPaintItem)
                    {
                        sprayPaintItem.sprayCanTank = UpdateChargeValue(sprayPaintItem.sprayCanTank, capsuleHoiPoiValue.ChargeTime);
                    }
                    else if (grabbableObject is TetraChemicalItem tetraChemicalItem)
                    {
                        tetraChemicalItem.fuel = UpdateChargeValue(tetraChemicalItem.fuel, capsuleHoiPoiValue.ChargeTime);
                    }
                }
                Addon addon;
                if ((addon = AFUtilities.GetAddonInstalled(this, "Repair Module")) != null
                    && grabbableObject.itemProperties.isScrap
                    && grabbableObject.scrapValue > 0)
                {
                    repairModuleDuration += Time.deltaTime;
                    if (repairModuleDuration >= ConfigManager.repairModuleDuration.Value)
                    {
                        repairModuleDuration = ConfigManager.repairModuleDuration.Value;
                        addon.RemoveAddon();
                    }
                }
            }
        }

        private float UpdateChargeValue(float charge, float chargeTime)
        {
            return Mathf.Min(charge + Time.deltaTime / chargeTime, 1f);
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
                    if (vehicle.physicsRegion.physicsTransform.GetComponentsInChildren<GrabbableObject>().Any(g => g is not ClipboardItem)
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
            Ray downRay = new(position, Vector3.down);
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
                if (repairModuleDuration > 0f)
                {
                    SetProfitScrapValue(ref grabbableObject);
                    Addon addon;
                    if ((addon = AFUtilities.GetAddonInstalled(this, "Repair Module")) != null)
                    {
                        addon.RemoveAddon();
                    }
                }
            }
            StoreComponentInCapsule(false);
            this.component = null;
            componentAnimators = null;
            componentRenderers = null;
            componentColliders = null;
        }

        public void SetProfitScrapValue(ref GrabbableObject grabbableObject)
        {
            float rateProfit = ConfigManager.repairModuleProfit.Value * repairModuleDuration / ConfigManager.repairModuleDuration.Value;
            repairModuleDuration = 0f;
            grabbableObject.scrapValue = (int)(grabbableObject.scrapValue * (1f + rateProfit / 100f));
            ScanNodeProperties scanNode = grabbableObject.gameObject.GetComponentInChildren<ScanNodeProperties>();
            if (scanNode == null)
            {
                AddonFusion.mls.LogError("Scan node is missing for item!: " + grabbableObject.gameObject.name);
                return;
            }
            scanNode.subText = $"Value: ${grabbableObject.scrapValue}";
            scanNode.scrapValue = grabbableObject.scrapValue;
        }

        private IEnumerator ImmuneCoroutine()
        {
            yield return new WaitForSeconds(0.2f);
            VehicleControllerPatch.isImmune = false;
        }
    }
}
