using UnityEngine;

namespace AddonFusion
{
    internal class Addon : MonoBehaviour
    {
        public bool hasAddon = false;
        public string addonName;
        public string toolTip;
        // FLASHLIGHT
        public bool isFlashing;
        public float maxSpotAngle;

        public void SetSpecificFields()
        {
            GrabbableObject grabbableObject = GetComponent<GrabbableObject>();
            if (grabbableObject is FlashlightItem flashlightItem)
            {
                isFlashing = false;
                maxSpotAngle = flashlightItem.flashlightBulb.spotAngle;
            }
        }

        public void RemoveAddon()
        {
            GrabbableObject grabbableObject = GetComponentInParent<GrabbableObject>();
            if (grabbableObject != null)
            {
                ScanNodeProperties scanNode = grabbableObject.gameObject.GetComponentInChildren<ScanNodeProperties>();
                if (scanNode != null)
                {
                    string textToRemove = "\nAddon: " + addonName;
                    if (scanNode.subText.Contains(textToRemove))
                    {
                        scanNode.subText = scanNode.subText.Remove(scanNode.subText.IndexOf(textToRemove), textToRemove.Length);
                    }
                    else
                    {
                        textToRemove = "Addon: " + addonName;
                        if (scanNode.subText.Contains(textToRemove))
                        {
                            scanNode.subText = scanNode.subText.Remove(scanNode.subText.IndexOf(textToRemove), textToRemove.Length);
                        }
                    }
                }
            }
            hasAddon = false;
            addonName = null;
            toolTip = null;
        }
    }
}
