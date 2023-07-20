using System;
using Models.Inventory.Configuration;

namespace Models.Inventory.Ingredients
{
    public class Bottle : Ingredient
    {
        public Bottle()
        {
            // set this ingredient as unrefined
            State = States.Unrefined;           
        }
        // copy constructor
        public Bottle(Item copy) : base(copy)
        {
            // set this ingredient as unrefined
            State = States.Unrefined;              
        }
        // JSON import constructor
        public Bottle(Model copy) : base(copy)
        {
            // set this ingredient as unrefined
            State = States.Unrefined;               
        }
        // override the clone method inherited from item
        public override Item Clone() { return new Bottle(this); }
    }
}