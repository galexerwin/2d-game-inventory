using Models.Inventory.Ingredients;
using Models.Inventory.Configuration;
using Models.Inventory.Maps;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Models.Inventory
{
    /**
     * Purpose: Inventory of all items
     *          Inventory for user
     *          Inventory for game
     */
    public class Inventory
    {
        public Print PrintMessage; // debug functionality        
        private PrefabControl _prefabControl;
        private Map Map;
        private List<Item> _gameItems; // a list of all members of the Item class the game makes use of
        private List<Item> _userItems; // a list of all members of the Item class the user has.
        private StringBuilder _debugMessage;
        private int _wealth; // currency the user has
        private readonly bool _inventoryIsReady; // inventory readiness
        private bool _isInTestMode;
        // constructor
        public Inventory(string configuration, PrefabControl prefabControl, Print printMessage)
        {
            // store a reference to prefab controller
            _prefabControl = prefabControl;
            // begin a new list for all items
            _gameItems = new List<Item>();
            // begin a new list for user items
            _userItems = new List<Item>();
            // load the catalog
            LoadCatalogItems(configuration);
            // debug
            PrintMessage = printMessage;            
            // create a new map
            Map = new Map(this);
            // enter test mode
            _isInTestMode = true;           
            // debugging output
            //DebugInventory(4);
            // exit test mode
            _isInTestMode = false;
            // set inventory ready
            _inventoryIsReady = true;
        }
        /**
         * Construct a map and returns the reference
         */
        public bool LoadMap(out Map backreference)
        {
            // check if the map is ready
            if (!_inventoryIsReady || !Map.MapIsReady)
            {
                // return a null reference
                backreference = null;
                // return false
                return false;
            }
            // pass back the reference
            backreference = Map;
            // return true
            return true;
        }
        /**
         * Return if inventory is ready
         */
        public bool IsInventoryReady() => _inventoryIsReady;
        /**
         * Returns the user's wealth
         */
        public int GetWealth() => _wealth;

        /**
         * Credit the user's wealth, maybe a currency drop?
         */
        public void SetWealth(int changeByAmount)
        {
            // check if inventory is ready
            if (!_inventoryIsReady && !_isInTestMode) return;
            // call debit or credit based on the changeAmount sign
            if (changeByAmount < 0)
            {
                // debit account
                DebitWealth(changeByAmount);
                // return
                return;
            }
            // credit account
            CreditWealth(changeByAmount);
        }
        /**
         * Returns the count of items in the inventory
         */
        public int GetInventoryCount() => _userItems.Count;

        /**
         * Returns the sku item's name
         */
        public string GetItemName(string sku) => GetItemBySKU(sku, _gameItems)?.Name ?? "Invalid";
        /**
         * Returns the quantity that has been placed
         * for a single SKU. Always returns zero if
         * not placeable
         */
        public int GetItemPlacedCount(string sku) => GetItemBySKU(sku, _userItems)?.Placed ?? 0;
        /**
         * Returns the quantity for a single SKU.
         */        
        public int GetItemQuantity(string sku) => GetItemBySKU(sku, _userItems)?.Quantity ?? 0;
        /**
         * Returns the price of an item
         */
        public int GetItemPurchasePrice(string sku)        
        {
            // check if inventory is ready
            if (_inventoryIsReady || _isInTestMode) return GetItemBySKU(sku, _gameItems)?.PurchaseValue ?? 0;
            // error
            PrintMessage("Inventory is not ready!");
            // return
            return 0;
        }

        /**
         * Returns the price of an item
         */
        public int GetItemSellingValue(string sku)
        {
            // check if inventory is ready
            if (_inventoryIsReady || _isInTestMode) return GetItemBySKU(sku, _gameItems)?.SellableValue ?? 0;
            // error
            PrintMessage("Inventory is not ready!");
            // return
            return 0;
        }
        /**
         * Adds an item from an enemy drop 
         */
        public void ItemInsert(string sku, int quantity = 1)
        {
            // check if inventory is ready
            if (!_inventoryIsReady && !_isInTestMode)
            {
                // error
                PrintMessage("Inventory is not ready!");
                // return false
                return;
            }             
            // insert item
            IncreaseOrAddItem(sku, quantity);
        }
        /**
         * Removes an item from a delete action
         */        
        public void ItemRemove(string sku, int quantity = 1)
        {
            // check if inventory is ready
            if (!_inventoryIsReady && !_isInTestMode)
            {
                // error
                PrintMessage("Inventory is not ready!");
                // return false
                return;
            }         
            // remove item
            DecreaseOrRemoveItem(sku, quantity);
        } 
        /**
         * Adds an item to inventory due to purchasing it
         */
        public bool Buy(string sku, int quantity = 1)
        {
            // check if inventory is ready
            if (!_inventoryIsReady && !_isInTestMode)
            {
                // error
                PrintMessage("Inventory is not ready!");
                // return false
                return false;
            }         
            // get the catalog version
            var obtainable = GetItemBySKU(sku, _gameItems);
            // make sure it exists
            if (obtainable == null) return false;
            // final value
            var finalValue = obtainable.PurchaseValue * quantity;
            // check if the user has enough for the purchase
            if (finalValue > _wealth) return false;
            // debit the user's wealth for purchasable amount 
            DebitWealth(finalValue);
            // add item or increase quantity (using this version to make sure to clone if necessary)
            IncreaseOrAddItem(obtainable.SKU, quantity);   
            // return
            return true;
        }
        /**
         * Sells an item by crediting the wealth and decrementing the quantity
         */        
        // default
        public bool Sell(string sku, int quantity = 1)
        {
            // check if inventory is ready
            if (!_inventoryIsReady && !_isInTestMode)
            {
                // error
                PrintMessage("Inventory is not ready!");
                // return false
                return false;
            }                 
            // get the inventory item
            var obtainable = GetItemBySKU(sku, _userItems);
            // make sure it exists
            if (obtainable == null || !obtainable.IsSellable()) return false;
            // final value
            var finalValue = obtainable.SellableValue * quantity;
            // remove the item quantity
            DecreaseOrRemoveItem(sku, quantity);
            // credit the user's wealth for sellable amount
            CreditWealth(finalValue);
            // return
            return true;
        }
        /**
         * Sells an obstacle
         * swaps with the sku
         */
        public Placeable SellObstacle(Placeable placeable, string sku, int x, int y)
        {
            // check if inventory is ready
            if (!_inventoryIsReady && !_isInTestMode)
            {
                // error
                PrintMessage("Inventory is not ready!");
                // return
                return null;
            }              
            // Item Clone
            var clone = placeable.ReturnItem();
            // if not sellable return null
            if (!(clone?.IsSellable() ?? false)) return null;
            // retrieve the top level item to perform operations on it (for clarity)
            var topLevelItem = GetItemBySKU(clone.SKU, _userItems);
            // sell the item
            DebitWealth(topLevelItem.PurchaseValue);
            // perform a swap and return new Placeable
            return SwapPlaceableGameItem(placeable, sku, x, y);
        }
        /**
         * Refine combines the required items for the sku
         * Returns a bool if successful
         */
        public bool Refine(string sku)
        {
            // check if inventory is ready
            if (!_inventoryIsReady && !_isInTestMode)
            {
                // error
                PrintMessage("Inventory is not ready!");
                // return false
                return false;
            }                 
            // retrieve the final item
            var finalProduct = GetItemBySKU(sku, _gameItems);
            // if it doesn't exists, return
            if (finalProduct == null)
            {
                // print debug message
                PrintMessage("Item SKU is incorrect!");
                // return false
                return false;
            }
            // check if the user has enough wealth
            if (_wealth < finalProduct.PurchaseValue)
            {
                // print debug message
                PrintMessage("Not enough wealth for transaction!");
                // return false
                return false;                
            }
            // set aside the needed cash
            DebitWealth(finalProduct.PurchaseValue);
            // get the requirements for the final product
            CraftingResource[] resources = finalProduct.Requires;
            // check if there are resources
            if (resources.Length == 0)
            {
                // print debug message
                PrintMessage("Item either can't be crafted or the recipe is unavailable!");
                // credit the user
                CreditWealth(finalProduct.PurchaseValue);
                // return false
                return false;                
            }
            // iterate through the resources to test if enough of each exists
            foreach(CraftingResource resource in resources)
            {
                var neededItem = GetItemBySKU(resource.SKU, _userItems);
                // check if the item exists
                if (neededItem == null || neededItem.Quantity < resource.Quantity)
                {
                    // print debug message
                    PrintMessage("Not enough " + resource.SKU + " is available!");
                    // credit the user
                    CreditWealth(finalProduct.PurchaseValue);                    
                    // return false
                    return false;                      
                }
            }
            // create a transaction buffer
            CraftingResource[] transactionBuffer = new CraftingResource[resources.Length];
            // item number
            int itemNumber = 0;
            // perform subtractions within a transaction
            foreach (CraftingResource resource in resources)
            {
                if (!DecreaseOrRemoveItem(resource.SKU, resource.Quantity))
                    break;
                // add to the transaction buffer
                transactionBuffer[itemNumber] = resource;
                // increment itemNumber
                itemNumber++;
            }
            // check the transaction buffer length is equal
            if (transactionBuffer.Length != resources.Length)
            {
                // undo the removing of items by iterating over what was accepted
                foreach(CraftingResource resource in transactionBuffer)
                    IncreaseOrAddItem(resource.SKU, resource.Quantity);
                // credit the user
                CreditWealth(finalProduct.PurchaseValue);
                // print debug message
                PrintMessage("An error occurred during crafting! Transaction rolled back.");
                // return false
                return false;
            }
            // add final product to the user's inventory
            IncreaseOrAddItem(finalProduct.SKU);
            // return true
            return true;
        }
        /**
         * Purpose: Return a placeable object for tile items
         * Tiles are not shown in the inventory list, they
         * however are inventory because some are sellable
         */
        public Placeable GetPlaceableGameItem(string sku, int x, int y)
        {
            // check if sku exists and is placeable
            if (!(GetItemBySKU(sku, _gameItems)?.IsPlaceable() ?? false)) return null;
            // increase or add to the user's inventory
            IncreaseOrAddItem(sku);                   
            // retrieve the item from the inventory
            var userItem = GetItemBySKU(sku, _userItems);
            // create a placeable object
            var placeable = userItem.Place();
            // check if placeable is null
            if (placeable == null) return null;
            // retrieve a game object into the placeable
            _prefabControl.InsertPrefabIntoPlaceable(placeable, x, y);
            // return a placeable
            return placeable;
        }
        /**
         * Purpose: Return a placeable object for towers
         * The item manages it's placeable quantity through checks
         */
        public Placeable GetPlaceableTower(string sku, GameObject tower)
        {
            // check if inventory is ready
            if (!_inventoryIsReady && !_isInTestMode)
            {
                // error
                PrintMessage("Inventory is not ready!");
                // return false
                return null;
            }                
            // uncomment the following line to override the lack of quantity
            // IncreaseOrAddItem(sku);        
            // fetch the tower sku
            var baseItem = GetItemBySKU(sku, _userItems);
            // retrieve the placeable
            var placeable = baseItem?.Place();
            // check if placeable is null
            if (placeable == null) return null;
            // install the game object 
            placeable.InstallGameObject(tower, PlacementType.Tower);
            // return the placeable object
            return placeable;              
        }
        /**
         * Purpose: Swap the tile to another type
         */
        public Placeable SwapPlaceableGameItem(Placeable placeable, string sku, int x, int y)
        {
            // get a reference to the current placeable
            var original = placeable;
            // guard null reference errors
            if (original == null || original.ReturnGameObject() == null) return null;
            // destroy the game object there
            _prefabControl.EndPrefabInPlaceable(placeable);
            // store a new placeable at the location
            Placeable replacement = GetPlaceableGameItem(sku, x, y);
            // make sure replacement isn't null
            if (replacement == null) return null;
            // retrieve the prior sku attached to the other item
            string oldsku = original.ReturnItem().SKU;
            // retrieve the base item
            Item itemBase = GetItemBySKU(oldsku, _userItems);
            // tell the item base to remove the clone
            itemBase.Remove(original.InstanceID);
            // remove or adjust the quantity
            DecreaseOrRemoveItem(oldsku);
            // return the replacement
            return replacement;
        }
        /**
         * Purpose: Return a collection item by sku
         */
        private Item GetItemBySKU(string sku, List<Item> collection)
        {
            // iterate over the collection of items
            foreach (var item in collection)
                // need an exact match
                if (item.SKU.Equals(sku))
                    // return the item
                    return item;
            // return the default
            return null;           
        }
        /**
         * Procedure to add item or add to the item quantity
         */
        // default (makes sure that there is a user item or passes the catalog version with newItem flag)
        private void IncreaseOrAddItem(string sku, int quantity = 1)
        {
            // retrieve the item
            var obtainable = GetItemBySKU(sku, _userItems);
            // if item doesn't exist in the user's inventory
            if (obtainable == null)
            {
                // obtain a catalog copy
                obtainable = GetItemBySKU(sku, _gameItems);
                // make sure an item was returned
                if (obtainable == null)
                {
                    // print a helpful message
                    PrintMessage("SKU is invalid!");
                    // exit
                    return;
                }
                // increase the number with the newItem flag
                IncreaseOrAddItem(obtainable, quantity, true);
                // return
                return;
            }
            // call overloaded with default of newItem == false because item exists in user's inventory
            IncreaseOrAddItem(obtainable, quantity);
        }
        // actual: allows for a catalog item to be added to user's inventory in specified amount
        private void IncreaseOrAddItem(Item obtainable, int quantity = 1, bool newItem = false)
        {
            // determine if a unique item unsellable or removable
            var mustBeKept = !obtainable.IsSellable() && !obtainable.IsRemovable();
            // override for placeable items
            mustBeKept = !obtainable.IsPlaceable() && mustBeKept;
            // check if new item
            if (newItem)
            {
                // obtainable is from the catalog (so clone it)
                _userItems.Add(obtainable.Clone());
                // set obtainable to the new instance
                obtainable = GetItemBySKU(obtainable.SKU, _userItems);
            }
            // add the quantity to the retrieved item
            obtainable.Quantity += (mustBeKept) ? 0 : quantity;
        }
        // overloaded with quantity
        private bool DecreaseOrRemoveItem(string sku, int quantity = 1)
        {
            // retrieve the item
            var obtainable = GetItemBySKU(sku, _userItems);
            // if item doesn't exist
            if (obtainable == null) return false;
            // return result
            return DecreaseOrRemoveItem(obtainable, quantity);
        }
        // actual
        private bool DecreaseOrRemoveItem(Item obtainable, int quantity)
        {
            // new quantity after decreasing
            var newQuantity = obtainable.Quantity - quantity;
            //PrintMessage("Sku: " + obtainable.SKU + " current on-hand: " + obtainable.Quantity + " decrease by: " + quantity + " new balance is: " + newQuantity);
            // check if the method would create a negative quantity
            if (newQuantity < 0) return false;
            // if the item isn't removable, don't try
            if (!obtainable.IsRemovable()) return false;
            // if the balance would be zero, remove and return
            if (newQuantity == 0)
            {
                // remove the item from the list
                _userItems.Remove(obtainable);
                // return
                return true;
            }
            // subtract quantity from the total
            obtainable.Quantity -= quantity;
            // return
            return true;
        }
        // increase the user's wealth by amount
        private void CreditWealth(int amount)
        {
            // increase wealth
            _wealth += Math.Abs(amount);
        }
        // decrease the user's wealth by amount
        private void DebitWealth(int amount)
        {
            // decrease wealth
            _wealth -= Math.Abs(amount);
        }
        // check if the item exists within the user's inventory
        private bool Exists(int itemID)
        {
            // returns true if the item exists
            return _userItems.Count > itemID && _userItems[itemID] != null;
        }
        /**
         * Purpose: Load item data from file
         */        
        private void LoadCatalogItems(string json)
        {
            // null reference may occur
            try
            {
                // deserialize the json
                List<Model> source = JsonConvert.DeserializeObject<List<Model>>(json);
                // iterate over the data in the source
                foreach (Model input in source)
                {
                    switch (input.Type)
                    {
                        case "Bottle":
                            _gameItems.Add(new Bottle(input));
                            break;
                        case "Brew":
                            _gameItems.Add(new Brew(input));
                            break;
                        case "Log":
                            _gameItems.Add(new Log(input));
                            break;
                        case "Metal":
                            _gameItems.Add(new Metal(input));
                            break;
                        case "Ore":
                            _gameItems.Add(new Ore(input));
                            break;
                        case "Potion":
                            _gameItems.Add(new Potion(input));
                            break;
                        case "Wood":
                            _gameItems.Add(new Wood(input));
                            break;
                        case "GrassTile":
                            _gameItems.Add(new Grass(input));
                            break;
                        case "PathTile":
                            _gameItems.Add(new StraightPath(input));
                            break;
                        case "CurvedPathTile":
                            _gameItems.Add(new CurvedPath(input));
                            break;
                        case "ObstacleTile":
                            _gameItems.Add(new Obstacle(input));
                            break;
                        case "Chassis":
                            _gameItems.Add(new Chassis(input));
                            break;
                        case "Tower":
                            _gameItems.Add(new Tower(input));
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                PrintMessage("Could not load the configuration file! Error:  " + e.Message);
            }
        }
        /**
         * Inventory Class Debugging Script
         * Takes int level  1 = Catalog,
         *                  2 = Catalog & User Inventory,
         *                  3 = 1 with Recipe Information,
         *                  4 = Inventory Management Tests
         *                  5 = All output
         */
        private void DebugInventory(int level = 5)
        {
            // before spacing
            BufferDebugLog(" ");
            BufferDebugLog("Inventory Debug Output");
            switch (level)
            {
                case 1:
                    PrintCatalogList();
                    break;
                case 2:
                    PrintCatalogList();
                    PrintInventoryList();
                    break;
                case 3:
                    PrintCatalogList(true);
                    PrintInventoryList(true);
                    break;
                case 4:
                    DebugInventoryManagement();
                    break;
                default:
                    PrintCatalogList(true);
                    DebugInventoryManagement();
                    PrintInventoryList(true);
                    break;
            }
            BufferDebugLog(" ");
            // print to console
            PrintMessage(_debugMessage.ToString());
        }
        /**
         * Simply removes mapping objects
         */       
        private int GetDebugInventoryCount() => GetInventoryCount() - 4;
        /**
         * Inventory Management Debugging Script
         */
        private void DebugInventoryManagement()
        {
            // testing
            BufferDebugLog("Inventory Tests***********");
            BufferDebugLog("Inventory Count: " + GetDebugInventoryCount());
            BufferDebugLog("Current Wealth: " + GetWealth());
            BufferDebugLog("Fire Brew Price: " + GetItemPurchasePrice("FireBrew"));
            BufferDebugLog("User Tries To Buy: Success? " + Buy("FireBrew"));
            BufferDebugLog("Inventory Count: " + GetDebugInventoryCount());       
            BufferDebugLog("Adding 50 additional wealth.");
            SetWealth(50);
            BufferDebugLog("Current Wealth: " + GetWealth());
            BufferDebugLog("User Tries To Buy: Success? " + Buy("FireBrew"));
            BufferDebugLog("Inventory Count: " + GetDebugInventoryCount());           
            BufferDebugLog("Adding 50 additional wealth.");
            SetWealth(50);
            BufferDebugLog("Current Wealth: " + GetWealth());
            BufferDebugLog("User Tries To Buy: Success? " + Buy("FireBrew"));
            BufferDebugLog("Inventory Count: " + GetDebugInventoryCount());
            BufferDebugLog("Current Wealth: " + GetWealth());
            BufferDebugLog("Fire Brew Count (should be 1): " + GetItemQuantity("FireBrew"));
            PrintInventoryList();
            BufferDebugLog("Direct Adding of a Glass Bottle");
            ItemInsert("Glass");
            BufferDebugLog("Inventory Count (should be 2): " + GetDebugInventoryCount());
            BufferDebugLog("Glass Count (should be 1): " + GetItemQuantity("Glass"));
            PrintInventoryList();
            BufferDebugLog("Direct Adding of another Glass Bottle");
            ItemInsert("Glass");
            BufferDebugLog("Inventory Count (should be 2): " + GetDebugInventoryCount());
            BufferDebugLog("Glass Count (should be 2): " + GetItemQuantity("Glass"));
            BufferDebugLog("Direct Adding of three more Glass Bottles");
            ItemInsert("Glass", 3);
            BufferDebugLog("Inventory Count (should be 2): " + GetDebugInventoryCount());
            BufferDebugLog("Glass Count (should be 5): " + GetItemQuantity("Glass"));
            BufferDebugLog("Direct Removal of a Glass Bottle");
            ItemRemove("Glass");
            BufferDebugLog("Inventory Count (should be 2): " + GetDebugInventoryCount());
            BufferDebugLog("Glass Count (should be 4): " + GetItemQuantity("Glass"));
            BufferDebugLog("Sell Removal of a Glass Bottle");
            BufferDebugLog("Current Wealth (should be 0): " + GetWealth());
            BufferDebugLog("Glass Bottle Sell Price: " + GetItemSellingValue("Glass"));
            Sell("Glass");
            BufferDebugLog("Inventory Count (should be 2): " + GetDebugInventoryCount());
            BufferDebugLog("Glass Count (should be 3): " + GetItemQuantity("Glass"));  
            BufferDebugLog("Current Wealth (should be 1): " + GetWealth()); 
            BufferDebugLog("Refining to Lightning (Bolt) Potion");
            BufferDebugLog("Bolt Potion Price: " + GetItemPurchasePrice("BoltPotion"));
            BufferDebugLog("Bolt Potion Recipe: ");
            PrintCraftingRecipe("BoltPotion");
            BufferDebugLog("Bolt Brew Quantity On Hand: " + GetItemQuantity("BoltBrew"));
            BufferDebugLog("Glass Bottle Quantity On Hand: " + GetItemQuantity("Glass"));
            BufferDebugLog("Bolt Potion Quantity On Hand: " + GetItemQuantity("BoltPotion"));
            BufferDebugLog("Attempting to refine was successful? " + Refine("BoltPotion"));
            BufferDebugLog("Direct Add of Bolt Brew");
            ItemInsert("BoltBrew");
            BufferDebugLog("Refining to Lightning (Bolt) Potion: Attempt 2");
            BufferDebugLog("Bolt Potion Price: " + GetItemPurchasePrice("BoltPotion"));
            PrintCraftingRecipe("BoltPotion");
            BufferDebugLog("Bolt Brew Quantity On Hand: " + GetItemQuantity("BoltBrew"));
            BufferDebugLog("Glass Bottle Quantity On Hand: " + GetItemQuantity("Glass"));
            BufferDebugLog("Bolt Potion Quantity On Hand: " + GetItemQuantity("BoltPotion"));
            BufferDebugLog("Attempting to refine was successful? " + Refine("BoltPotion"));
            BufferDebugLog("Current Wealth (should be 1): " + GetWealth()); 
            BufferDebugLog("Adding 124 additional wealth.");
            SetWealth(124);
            BufferDebugLog("Current Wealth (should be 125): " + GetWealth());
            BufferDebugLog("Refining to Lightning (Bolt) Potion: Attempt 3");
            BufferDebugLog("Bolt Potion Price: " + GetItemPurchasePrice("BoltPotion"));
            PrintCraftingRecipe("BoltPotion");
            BufferDebugLog("Bolt Brew Quantity On Hand: " + GetItemQuantity("BoltBrew"));
            BufferDebugLog("Glass Bottle Quantity On Hand: " + GetItemQuantity("Glass"));
            BufferDebugLog("Bolt Potion Quantity On Hand: " + GetItemQuantity("BoltPotion"));
            BufferDebugLog("Attempting to refine was successful? " + Refine("BoltPotion"));
            BufferDebugLog("Bolt Brew Quantity On Hand: " + GetItemQuantity("BoltBrew"));
            BufferDebugLog("Glass Bottle Quantity On Hand: " + GetItemQuantity("Glass"));
            BufferDebugLog("Bolt Potion Quantity On Hand: " + GetItemQuantity("BoltPotion")); 
            BufferDebugLog("Refining to Wood");
            PrintCraftingRecipe("Wood");
            BufferDebugLog("Log Quantity On Hand: " + GetItemQuantity("Log"));
            BufferDebugLog("Wood Quantity On Hand: " + GetItemQuantity("Wood"));
            BufferDebugLog("Current Wealth (should be 0): " + GetWealth()); 
            BufferDebugLog("Adding 5 additional wealth.");
            SetWealth(5);
            BufferDebugLog("Current Wealth (should be 5): " + GetWealth());
            BufferDebugLog("Inserting a Log");
            ItemInsert("Log");
            BufferDebugLog("Log Quantity On Hand: " + GetItemQuantity("Log"));
            BufferDebugLog("Wood Quantity On Hand: " + GetItemQuantity("Wood"));
            BufferDebugLog("Attempting to refine was successful? " + Refine("Wood"));
            BufferDebugLog("Log Quantity On Hand: " + GetItemQuantity("Log"));
            BufferDebugLog("Wood Quantity On Hand: " + GetItemQuantity("Wood"));
            ItemRemove("Wood");
            BufferDebugLog("Crafting a Cannon Chassis");
            BufferDebugLog("Inserting two Logs");
            ItemInsert("Log", 2);            
            BufferDebugLog("Inserting one Ore");
            ItemInsert("Ore");              
            BufferDebugLog("Log Quantity On Hand: " + GetItemQuantity("Log"));
            BufferDebugLog("Ore Quantity On Hand: " + GetItemQuantity("Ore"));
            BufferDebugLog("Refining Logs");
            BufferDebugLog("Attempting to refine was successful? " + Refine("Wood"));
            BufferDebugLog("Attempting to refine was successful? " + Refine("Wood"));
            BufferDebugLog("Refining Ore");
            BufferDebugLog("Attempting to refine was successful? " + Refine("Metal"));
            BufferDebugLog("Log Quantity On Hand: " + GetItemQuantity("Log"));
            BufferDebugLog("Ore Quantity On Hand: " + GetItemQuantity("Ore"));
            BufferDebugLog("Wood Quantity On Hand: " + GetItemQuantity("Wood"));
            BufferDebugLog("Metal Quantity On Hand: " + GetItemQuantity("Metal"));
            BufferDebugLog("Attempting to craft a Cannon Chassis was successful? " + Refine("CannonChassis"));
            BufferDebugLog("Crafting a Fire Potion");
            BufferDebugLog("Inserting General Brew 1 & 2");
            ItemInsert("GeneralBrew1", 3);
            ItemInsert("GeneralBrew2",3);
            BufferDebugLog("Inserting Glass Bottle");
            ItemInsert("Glass");
            BufferDebugLog("General Brew 1 Quantity On Hand: " + GetItemQuantity("GeneralBrew1"));
            BufferDebugLog("General Brew 2 Quantity On Hand: " + GetItemQuantity("GeneralBrew2"));
            BufferDebugLog("Glass Bottle Quantity On Hand: " + GetItemQuantity("Glass"));
            BufferDebugLog("Attempting to craft a Fire Potion was successful? " + Refine("FirePotion"));
            BufferDebugLog("Crafting a Fire Cannon Tower");
            BufferDebugLog("Fire Potion Quantity On Hand: " + GetItemQuantity("FirePotion"));
            BufferDebugLog("Cannon Chassis Quantity On Hand: " + GetItemQuantity("CannonChassis"));
            BufferDebugLog("Fire Cannon Tower Quantity On Hand: " + GetItemQuantity("FireCannonTower"));
            BufferDebugLog("Attempting to craft a Fire Potion was successful? " + Refine("FireCannonTower"));
            BufferDebugLog("Fire Potion Quantity On Hand: " + GetItemQuantity("FirePotion"));
            BufferDebugLog("Cannon Chassis Quantity On Hand: " + GetItemQuantity("CannonChassis"));
            BufferDebugLog("Fire Cannon Tower Quantity On Hand: " + GetItemQuantity("FireCannonTower"));            
        }
        /**
         * Debug Catalog
         */
        private void PrintCatalogList(bool withCraftingRequirements = false)
        {
            // before spacing
            BufferDebugLog(" "); 
            // title
            BufferDebugLog("All Items In The Catalog:");
            // iterate over all items in user inventory
            foreach (var item in _gameItems)
            {
                // send to buffered output
                BufferDebugLog("* Item: " + item.Name + ", Qty: " + item.Quantity + ", SKU: " + item.SKU + ", Can Buy For: " + item.PurchaseValue + " Can Sell For: " + item.SellableValue + ", Placeable on map? " + item.IsPlaceable());
                // if crafting requirements
                if (withCraftingRequirements)
                    PrintCraftingRecipe(item.SKU);
            }
            // after spacing
            BufferDebugLog(" ");            
        }
        /**
         * Debug Inventory
         */
        private void PrintInventoryList(bool withCraftingRequirements = false)
        {
            // before spacing
            BufferDebugLog(" "); 
            // title
            BufferDebugLog("All Items In User's Inventory:");
            // iterate over all items in user inventory
            foreach (var item in _userItems)
            {
                // send to buffered output
                BufferDebugLog("* Item: " + item.Name + ", Qty: " + item.Quantity + ", SKU: " + item.SKU + ", Can Buy For: " + item.PurchaseValue + " Can Sell For: " + item.SellableValue + ", Placeable on map? " + item.IsPlaceable());
                // if crafting requirements
                if (withCraftingRequirements)
                    PrintCraftingRecipe(item.SKU);
            }
            // after spacing
            BufferDebugLog(" ");
        }
        /**
         * Debug Recipe
         */        
        private void PrintCraftingRecipe(string sku)
        {
            // get the catalog obtainable from the sku
            var obtainable = GetItemBySKU(sku, _gameItems);
            // error out if no recipe exists
            if (obtainable.Requires.Length == 0)
            {
                // debug message
                BufferDebugLog("** Item does not have a recipe or isn't craftable!");
            }
            else
            {
                // debug message
                BufferDebugLog("** The recipe for " + obtainable.Name + " requires:");
                // iterate over the recipe
                foreach (var resource in obtainable.Requires)
                    // buffer output
                    BufferDebugLog("  --" + resource.Quantity + " " + resource.SKU);
            }
        }        
        /**
         * Buffer Message
         */
        public void BufferDebugLog(string message)
        {
            // create if not exists
            _debugMessage ??= new StringBuilder();
            // add the string
            _debugMessage.AppendLine(message);
        }        
    }
}