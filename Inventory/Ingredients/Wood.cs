using System;
using Models.Inventory.Configuration;

namespace Models.Inventory.Ingredients
{
    public class Wood : Ingredient
    {
        public Wood()
        {
            // set this ingredient as refined
            State = States.Refined;           
        }
        // copy constructor
        public Wood(Item copy) : base(copy)
        {
            // set this ingredient as refined
            State = States.Refined;              
        }
        // JSON import constructor
        public Wood(Model copy) : base(copy)
        {
            // set this ingredient as refined
            State = States.Refined;               
        }
        // override the clone method inherited from item
        public override Item Clone() { return new Wood(this); }
        // make a consumable item
        public new bool IsConsumable() { return true; }        
    }
}