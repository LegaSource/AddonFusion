namespace AddonFusion.AddonValues
{
    internal class BladeSharpenerValue
    {
        public string EntityName { get; private set; }
        public int CriticalSuccessChance { get; private set; }
        public int CriticalFailChance { get; private set; }

        public BladeSharpenerValue(string entityName, int criticalSuccessChance, int criticalFailChance)
        {
            this.EntityName = entityName;
            this.CriticalSuccessChance = criticalSuccessChance;
            this.CriticalFailChance = criticalFailChance;
        }
    }
}
