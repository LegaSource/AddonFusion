using System;

namespace AddonFusion
{
    internal class CustomItem
    {
        public bool IsEnabled { get; private set; }
        public Type Type { get; private set; }
        public Item Item { get; private set; }
        public bool IsSpawnable { get; private set; }
        public int MaxSpawn { get; private set; }
        public int Rarity { get; private set; }
        public bool IsPurchasable { get; private set; }
        public string Description { get; private set; }
        public int Price { get; private set; }

        public CustomItem(bool isEnabled, Type type, Item item, bool isSpawnable, int maxSpawn, int rarity, bool isPurchasable, string description, int price)
        {
            this.IsEnabled = isEnabled;
            this.Type = type;
            this.Item = item;
            this.IsSpawnable = isSpawnable;
            this.MaxSpawn = maxSpawn;
            this.Rarity = rarity;
            this.IsPurchasable = isPurchasable;
            this.Description = description;
            this.Price = price;
        }
    }
}
