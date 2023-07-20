using Models.Inventory.Maps;
using UnityEngine;

namespace Models.Inventory
{
    public class MController : MonoBehaviour
    {
        // inventory controller
        public IController IController;  
        // inventory
        private Inventory _inventory;
        // direct reference to the map
        public Map Map;
        // guard for null references
        public bool MapHasLoaded = false;
        /**
         * Link with the inventory controller for map access
         */
        private void Update()
        {
            // call until loaded
            if (!MapHasLoaded) HasLoaded();
        }

        private void HasLoaded()
        {
            // check if we already loaded the map
            if (MapHasLoaded) return;
            // check if the inventory controller has loaded
            if (IController == null) return;
            // check if the inventory system has been loaded    
            if (!IController.InventoryHasLoaded) return;
            // reference the inventory store
            _inventory = IController.Items;
            // check if reference is set
            if (_inventory == null) return;
            // load the map for use
            if (!_inventory.LoadMap(out Map)) return;
            // if the map reference is null
            if (Map == null) return;
            // set the map has loaded
            MapHasLoaded = true;
        }
        
    }
}
