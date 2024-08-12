namespace AddonFusion.AddonValues
{
    internal class PyrethrinTankValue
    {
        public string EntityName { get; private set; }
        public float FleeDuration { get; private set; }

        public PyrethrinTankValue(string entityName, float fleeDuration)
        {
            this.EntityName = entityName;
            this.FleeDuration = fleeDuration;
        }
    }
}
