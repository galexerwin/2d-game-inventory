using System;
using Models.Inventory.Configuration;

namespace Models.Inventory.Ingredients
{
    public class Brew : Ingredient
    {
        public Brew()
        {
            // set this ingredient as unrefined
            State = States.Unrefined;           
        }
        // copy constructor
        public Brew(Item copy) : base(copy)
        {
            // set this ingredient as unrefined
            State = States.Unrefined;              
        }
        // JSON import constructor
        public Brew(Model copy) : base(copy)
        {
            // set this ingredient as unrefined
            State = States.Unrefined;               
        }
        // override the clone method inherited from item
        public override Item Clone() { return new Brew(this); }
    }
}