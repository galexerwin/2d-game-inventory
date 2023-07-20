using Models.Inventory.Configuration;

namespace Models.Inventory.Maps
{
    public class Grass : Tile
    {
        public Grass() {}
        public Grass(Item copy) : base(copy){}
        public Grass(Model copy) : base(copy){}
        // override the clone method inherited from item
        public override Item Clone() { return new Grass(this); }         
        // this item can not be sold
        public override bool IsSellable() { return false; }
    }
}