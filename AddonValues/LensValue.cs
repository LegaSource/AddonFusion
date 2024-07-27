namespace AddonFusion.AddonValues
{
    internal class LensValue
    {
        public string EntityName { get; private set; }
        public float FlashDuration { get; private set; }
        public float StunDuration { get; private set; }
        public float LightAngle { get; private set; }
        public float EntityAngle { get; private set; }
        public int EntityDistance { get; private set; }
        public float ImmunityDuration { get; private set; }
        public float BatteryConsumption { get; private set; }

        public LensValue(string entityName, float flashDuration, float stunDuration, float lightAngle, float entityAngle, int entityDistance, float immunityDuration, float batteryConsumption)
        {
            this.EntityName = entityName;
            this.FlashDuration = flashDuration;
            this.StunDuration = stunDuration;
            this.LightAngle = lightAngle;
            this.EntityAngle = entityAngle;
            this.EntityDistance = entityDistance;
            this.ImmunityDuration = immunityDuration;
            this.BatteryConsumption = batteryConsumption;
        }
    }
}
