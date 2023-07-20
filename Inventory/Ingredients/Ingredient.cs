using System;
using Models.Inventory.Configuration;

namespace Models.Inventory.Ingredients
{
    /**
     * Purpose: Super class of all ingredients
     *          Derives from item class.
     */     
    public class Ingredient : Item
    {
        public States State { get; set; } // state of the ingredient (un)refined/final
        // needed to allow for below
        protected Ingredient():base(){}
        // copy data upwards when cloning or importing
        protected Ingredient(Item copy) : base(copy){}
        protected Ingredient(Model copy) : base(copy){}
        // set as consumable
        public override bool IsConsumable() { return true; }
        // this item can be sold
        public override bool IsSellable() { return true; }   
        // this item can be removed from inventory if no more exist
        public override bool IsRemovable() { return true; }           
    }
}