using Models.Inventory.Configuration;
using UnityEngine;

namespace Models.Inventory
{
    /**
     * Purpose: Super class of Advanced Tower Class
     *          Derives from product class.
     *          Encompasses normal tower properties and methods
     */       
    public class Tower : Product
    {
        public int TimeUntilDischarged { get; set; } // the discharging state 
        public int MaximumAreaOfEffect { get; set; } // maximum area of effect
        public Vector3 Placement { get; set; } // exact placement 
        public Tower() {}
        public Tower(Item copy) : base(copy){}
        public Tower(Model copy) : base(copy){}       
        // override the clone method inherited from item
        public override Item Clone() { return new Tower(this); }     
        /**
        * tower items are placeable
        */   
        public override bool IsPlaceable() { return true; }
    }
}