using System;
using System.Collections.Generic;
using System.IO;
using Models.Inventory.Configuration;
using Newtonsoft.Json;
using Models.Inventory.Ingredients;

namespace Models.Inventory
{
    /**
     * Purpose: All obtainable/creatable items that could end up in the user's inventory
     */    
    public class Catalog
    {
        // project path to configuration file
        private const string CatalogConfigFile = @"\Assets\Scripts\Models\Inventory\Configuration\Configure.json";
        // list of available obtainable items
        private List<Item> _gameItems;

        public Catalog()
        {
            // initialize a new list of items
            _gameItems = new List<Item>();
            // load the items
            LoadItems();            
            
            Console.WriteLine("Item in List: " + _gameItems.Count);
        }

        public List<Item> GetList()
        {
            return _gameItems;
        }
        
        /**
         * Purpose: Return an obtainable item for inventory
         */
        public Item GetObtainable(string SKU)
        {
            // iterate over the collection of items in the Catalog
            foreach (var item in _gameItems)
                // need an exact match
                if (item.SKU == SKU)
                    // return the item
                    return item;
            // return the default
            return null;
        }
        /**
         * Purpose: Load item data from file
         */
        private void LoadItems()
        {
            // null reference may occur
            try
            {
                // get the full path
                string ConfigFilePath = Directory.GetCurrentDirectory() + @"\" + CatalogConfigFile;
                // store a list of items from source
                List<Model> source;
                // read the configuration file
                using (StreamReader reader = new StreamReader(ConfigFilePath))
                {
                    // retrieve the json as a string
                    string json = reader.ReadToEnd();
                    // deserialize the json
                    source = JsonConvert.DeserializeObject<List<Model>>(json);
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
                        }
                    }                
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not load the configuration file! Error: " + e.Message);
            }
        }
    }
}