using System;
using Models.Inventory.Configuration;

namespace Models.Inventory.Ingredients
{
    public class Ore : Ingredient
    {
        public Ore()
        {
            // set this ingredient as unrefined
            State = States.Unrefined;           
        }
        // copy constructor
        public Ore(Item copy) : base(copy)
        {
            // set this ingredient as unrefined
            State = States.Unrefined;              
        }
        // JSON import constructor
        public Ore(Model copy) : base(copy)
        {
            // set this ingredient as unrefined
            State = States.Unrefined;               
        } 
        // override the clone method inherited from item
        public override Item Clone() { return new Ore(this); }
        // make a transmogrifiable item
        public override bool IsTransmogrifiable() { return true; }        
    }
}