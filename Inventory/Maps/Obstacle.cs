using Models.Inventory.Configuration;

namespace Models.Inventory.Maps
{
    public class Obstacle : Tile
    {
        public Obstacle() {}
        public Obstacle(Item copy) : base(copy){}
        public Obstacle(Model copy) : base(copy){}
        // override the clone method inherited from item
        public override Item Clone() { return new Obstacle(this); } 
        // this item can be sold
        public override bool IsSellable() { return true; }   
        // this item can be removed from inventory if no more exist
        public override bool IsRemovable() { return true; }  
    }
}