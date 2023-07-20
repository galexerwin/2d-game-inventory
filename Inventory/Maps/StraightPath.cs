using Models.Inventory.Configuration;

namespace Models.Inventory.Maps
{
    public class StraightPath : Tile
    {
        public StraightPath() {}
        public StraightPath(Item copy) : base(copy){}
        public StraightPath(Model copy) : base(copy){}
        // override the clone method inherited from item
        public override Item Clone() { return new StraightPath(this); }         
        // this item can not be sold
        public override bool IsSellable() { return false; }        
    }
}