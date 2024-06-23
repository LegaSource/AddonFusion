namespace AddonFusion.AddonValues
{
    internal class LensValue
    {
        public string EntityName { get; private set; }
        public float FlashTime { get; private set; }
        public float StunTime { get; private set; }
        public float LightAngle { get; private set; }
        public float EntityAngle { get; private set; }
        public int EntityDistance { get; private set; }
        public float ImmunityTime { get; private set; }
        public float BatteryConsumption { get; private set; }

        public LensValue(string entityName, float flashTime, float stunTime, float lightAngle, float entityAngle, int entityDistance, float immunityTime, float batteryConsumption)
        {
            this.EntityName = entityName;
            this.FlashTime = flashTime;
            this.StunTime = stunTime;
            this.LightAngle = lightAngle;
            this.EntityAngle = entityAngle;
            this.EntityDistance = entityDistance;
            this.ImmunityTime = immunityTime;
            this.BatteryConsumption = batteryConsumption;
        }
    }
}
