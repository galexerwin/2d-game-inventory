using System.Collections.Generic;
using Models.Inventory.Configuration;

namespace Models.Inventory.Maps
{
    public class Tile : Item
    {
        // needed to allow for below
        protected Tile():base(){}
        // copy data upwards when cloning or importing
        protected Tile(Item copy) : base(copy){}
        protected Tile(Model copy) : base(copy){}
        // tile items are not shown in the inventory
        public override bool IsHiddenFromInventory() { return true; } 
        // tile items are placeable
        public override bool IsPlaceable() { return true; }        
    }
}