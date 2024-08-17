namespace AddonFusion
{
    internal class CutomEphemeralItem
    {
        public Item Item { get; private set; }
        public int Rarity { get; private set; }
        public int AddonRarity { get; private set; }
        public int MinUse { get; private set; }
        public int MaxUse { get; private set; }

        public CutomEphemeralItem(Item item, int rarity, int addonRarity, int minUse, int maxUse)
        {
            this.Item = item;
            this.Rarity = rarity;
            this.AddonRarity = addonRarity;
            this.MinUse = minUse;
            this.MaxUse = maxUse;
        }
    }
}
