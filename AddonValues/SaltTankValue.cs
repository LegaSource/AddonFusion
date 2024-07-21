namespace AddonFusion.AddonValues
{
    internal class SaltTankValue
    {
        public string EntityName { get; private set; }
        public int BaseChance { get; private set; }
        public int AdditionalChance { get; private set; }

        public SaltTankValue(string entityName, int baseChance, int additionalChance)
        {
            this.EntityName = entityName;
            this.BaseChance = baseChance;
            this.AdditionalChance = additionalChance;
        }
    }
}
