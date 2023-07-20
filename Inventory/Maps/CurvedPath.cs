using Models.Inventory.Configuration;

namespace Models.Inventory.Maps
{
    public class CurvedPath : Tile
    {
        public CurvedPath() {}
        public CurvedPath(Item copy) : base(copy){}
        public CurvedPath(Model copy) : base(copy){}
        // override the clone method inherited from item
        public override Item Clone() { return new CurvedPath(this); }         
        // this item can not be sold
        public override bool IsSellable() { return false; }            
    }
}