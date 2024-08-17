namespace AddonFusion.AddonValues
{
    internal class CapsuleHoiPoiValue
    {
        public string ItemName { get; private set; }
        public int ChargeTime { get; private set; }

        public CapsuleHoiPoiValue(string itemName, int chargeTime)
        {
            this.ItemName = itemName;
            this.ChargeTime = chargeTime;
        }
    }
}
