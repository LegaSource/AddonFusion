namespace AddonFusion.AddonValues
{
    internal class ProtectiveCordValue
    {
        public string EntityName { get; private set; }
        public float CooldownDuration { get; private set; }
        public float StunDuration { get; private set; }
        public float SpeedBoostDuration { get; private set; }
        public int SpeedMultiplier { get; private set; }
        public int StaminaRegen { get; private set; }

        public ProtectiveCordValue(string entityName, float cooldownDuration, float stunDuration, float speedBoostDuration, int speedMultiplier, int staminaRegen)
        {
            this.EntityName = entityName;
            this.CooldownDuration = cooldownDuration;
            this.StunDuration = stunDuration;
            this.SpeedBoostDuration = speedBoostDuration;
            this.SpeedMultiplier = speedMultiplier;
            this.StaminaRegen = staminaRegen;
        }
    }
}
