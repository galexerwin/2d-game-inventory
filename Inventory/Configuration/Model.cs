namespace Models.Inventory.Configuration
{
    public class Model
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public string Type { get; set; }
        public string Tooltip { get; set; }
        public int PurchaseValue { get; set; }
        public int SellableValue { get; set; }
        public string Element { get; set; }
        public string PlacementType { get; set; }
        public CraftingResource[] Requires { get; set; }
        public SpritePath Image { get; set; }
    }
}