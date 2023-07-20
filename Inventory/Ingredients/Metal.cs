using System;
using Models.Inventory.Configuration;

namespace Models.Inventory.Ingredients
{
    public class Metal : Ingredient
    {
        public Metal()
        {
            // set this ingredient as refined
            State = States.Refined;           
        }
        // copy constructor
        public Metal(Item copy) : base(copy)
        {
            // set this ingredient as refined
            State = States.Refined;              
        }
        // JSON import constructor
        public Metal(Model copy) : base(copy)
        {
            // set this ingredient as refined
            State = States.Refined;               
        }
        // override the clone method inherited from item
        public override Item Clone() { return new Metal(this); }
    }
}