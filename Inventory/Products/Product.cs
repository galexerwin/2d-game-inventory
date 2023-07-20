using Models.Inventory.Configuration;

namespace Models.Inventory
{
    /**
     * Purpose: Super class of tower and chassis classes
     *          Derives from item class.
     */    
    public class Product : Item
    {
        public Skills BaseSkill { get; set; } // base physical skill
        public bool OnTheMap { get; set; } // if this item is on the map currently
        public bool CanTakeDamage { get; set; } // can the object take damage
        public int HealthMax { get; set; } // the maximum damage this object can take
        public int HealthNow { get; set; } // the current health of this object
        public int BlockSizeForSelf { get; set; } // map blocks this object takes up for itself
        public int BlockSizeForBorder { get; set; } // map blocks this object requires for a border
        // needed to allow for below
        protected Product():base(){}
        // copy data upwards when cloning or importing
        protected Product(Item copy) :base(copy){}
        protected Product(Model copy) : base(copy){}
        // this item can be sold
        public override bool IsSellable() { return true; }   
        // this item can be removed from inventory if no more exist
        public override bool IsRemovable() { return true; }           
    }
}