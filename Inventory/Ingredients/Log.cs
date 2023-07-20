using System;
using Models.Inventory.Configuration;

namespace Models.Inventory.Ingredients
{
    public class Log : Ingredient
    {
        public Log()
        {
            // set this ingredient as unrefined
            State = States.Unrefined;           
        }
        // copy constructor
        public Log(Item copy) : base(copy)
        {
            // set this ingredient as unrefined
            State = States.Unrefined;              
        }
        // JSON import constructor
        public Log(Model copy) : base(copy)
        {
            // set this ingredient as unrefined
            State = States.Unrefined;               
        }  
        // override the clone method inherited from item
        public override Item Clone() { return new Log(this); }
        // make a transmogrifiable item
        public override bool IsTransmogrifiable() { return true; }
    }
}