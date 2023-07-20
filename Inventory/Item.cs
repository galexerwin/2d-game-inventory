using System;
using System.Collections;
using System.Collections.Generic;
using Models.Inventory.Configuration;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Models.Inventory
{
    /**
     * Purpose: Base Obtainable Object Class
     */      
    public class Item
    {
        public string Name { get; protected set; } // name
        public string Tooltip { get; protected set; } // roll over description
        public int PurchaseValue { get; protected set; } // value if purchased
        public int SellableValue { get; protected set; } // value if sold
        public int Quantity { get; set; } // quantity on hand
        
        public int Placed { get; set; } // quantity placed
        public String SKU { get; protected set; } // uniquely distinguishing different item types
        public PlacementType PlaceableType { get; protected set; } // get the placement type
        public Elements Element { get; set; } // element assigned to object
        public CraftingResource[] Requires { get; protected set; }
        public SpritePath Image { get; protected set; } // path to the sprite (mainly for ingredients and obstacles)

        protected List<Placeable> Placeables; // items that can be placed on a map
        // base constructor with default assignments
        public Item() : this("NoNameYet!", "None", 999, 0) {}
        /**
         * Overloaded constructor
         * Call me when no tool tip available
         */
        public Item(String name, String sku, int purchaseValue, int sellableValue): this(name, sku, "No hover text assigned yet!", purchaseValue, sellableValue) {}
        /**
         * Overloaded constructor
         * Call me when specifics are available
         */
        public Item(String name, String sku, String tooltip, int purchaseValue, int sellableValue)
        {
            // assign a default name
            Name = name;
            // assign a sku (so we can modify a single instance of the item)
            SKU = sku;
            // assign a default tooltip
            Tooltip = tooltip;
            // initially an item has no element unless specifically placed
            Element = Elements.None;
            // purchase value is super high (from base constructor) 
            PurchaseValue = purchaseValue;
            // sellable value is super low (from base constructor)
            SellableValue = sellableValue;
            // quantity is initially 1
            Quantity = 0;
            // placed is initially 0
            Placed = 0;
            // default placeable type is none
            PlaceableType = PlacementType.None;
        }
        /**
         * Copy constructor for copying a new instance from another object
         */
        protected Item(Item item)
        {
            // copy over the data from object
            Name = item.Name;
            SKU = item.SKU;
            Tooltip = item.Tooltip;
            Element = item.Element;
            PurchaseValue = item.PurchaseValue;
            SellableValue = item.SellableValue;
            Quantity = 0;
            Placed = 0;
            Requires = item.Requires;
            Image = item.Image;
            PlaceableType = item.PlaceableType;
        }
        /**
         * Copy constructor for copying a new instance from config import
         */
        protected Item(Model item)
        {
            // copy over the data from object
            Name = item.Name;
            SKU = item.SKU;
            Tooltip = item.Tooltip;
            PurchaseValue = item.PurchaseValue;
            SellableValue = item.SellableValue;
            Quantity = 0;
            Placed = 0;
            Requires = item.Requires;
            Image = item.Image;
            // fix up the placeable type
            switch (item.PlacementType)
            {
                case "Tower":
                    PlaceableType = PlacementType.Tower;
                    break;
                case "Path":
                    PlaceableType = PlacementType.Path;
                    break;
                case "Obstacle":
                    PlaceableType = PlacementType.Obstacle;
                    break;
                case "Grass":
                    PlaceableType = PlacementType.Grass;
                    break;
                default:
                    PlaceableType = PlacementType.None;
                    break;
            }
            // fix up the element
            switch (item.Element)
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
        }        
        // return a cloned instance
        public virtual Item Clone()
        {
            return new Item(this);
        }
        /**
         * Purpose: Return a placeable object
         */
        public Placeable Place()
        { // should add a recursion guard ? like _AmAPlaceable = true | false
            // guard statement
            if (!IsPlaceable()) return null;
            // do not place if there are none
            if (Quantity < 1 || Quantity < Placed + 1) return null;
            // create the list if it doesn't exist yet
            if (Placeables == null) Placeables = new List<Placeable>();
            // create a new placeable
            Placeable toPlace = new Placeable();
            // add the reference to a cloned object
            toPlace.InstallItem(Clone());
            // add a new member
            Placeables.Add(toPlace);
            // increase the placed number
            Placed += 1;
            // return the same instance
            return toPlace;
        }
        /**
         * Purpose: Remove a placeable object from the placeables array
         */
        public bool Remove(string instanceID)
        {
            // guard statement
            if (!IsPlaceable() || Placeables.Count == 0) return false;
            // iterate over the placeable array
            foreach (var placeable in Placeables)
            {
                // check if this is the droid we're looking for
                if (placeable.InstanceID == instanceID)
                {
                    // remove the placeable
                    Placeables.Remove(placeable);
                    // decrease placed number
                    Placed -= 1;
                    // return 
                    return true;
                }
            }
            // return default
            return false;
        }
        // effects array?
        
        // can you transform this item
        public virtual bool IsTransmogrifiable() { return false; }
        // can you sell this item for currency?
        public virtual bool IsSellable() { return true; }
        // can you consume this item to make another item?
        
        public virtual bool IsConsumable() { return false; }
        // can you delete this item from inventory?
        public virtual bool IsRemovable() { return false; }
        // can you place this item on the map?
        public virtual bool IsPlaceable() { return false; } 
        // Is this item hidden from inventory
        public virtual bool IsHiddenFromInventory() { return false; } 
    }
}