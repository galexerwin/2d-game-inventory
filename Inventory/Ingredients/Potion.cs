using System;
using Models.Inventory.Configuration;

namespace Models.Inventory.Ingredients
{
    public class Potion : Ingredient
    {
        public Potion()
        {
            // set this ingredient as refined
            State = States.Refined;           
        }
        // copy constructor
        public Potion(Item copy) : base(copy)
        {
            // set this ingredient as refined
            State = States.Refined;              
        }
        // JSON import constructor
        public Potion(Model copy) : base(copy)
        {
            // set this ingredient as refined
            State = States.Refined;               
        }
        // override the clone method inherited from item
        public override Item Clone() { return new Potion(this); }        
        /*public Potion()
        {
            // set this ingredient as refined
            State = States.Refined;       
        }
        // JSON to class constructor
        public Potion(Model import)
        {
            // set this ingredient as unrefined
            State = States.Unrefined;   
            // copy elements
            Name = import.Name;
            SKU = import.SKU;
            Tooltip = import.Tooltip;
            PurchaseValue = import.PurchaseValue;
            SellableValue = import.SellableValue;
            Requires = import.Requires;
            Image = import.Image;
            // fix up the element
            switch (import.Element)
            {
                case "Life":
                    Element = Elements.Life;
                    break;
                case "Fire":
                    Element = Elements.Fire;
                    break;
                case "Ice":
                    Element = Elements.Ice;
                    break;
                case "Lightning":
                    Element = Elements.Lightning;
                    break;
                case "Poison":
                    Element = Elements.Poison;
                    break;
            }
        }   */     
        // make a consumable item
        public new bool IsConsumable() { return true; }
    }
}