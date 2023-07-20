using System;
using System.Collections.Generic;
using Models.Inventory.Configuration;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Models.Inventory
{
    public class IController : MonoBehaviour
    {
        // game objects
        public TextMeshProUGUI coinUI; // Added by Aleks
        public GameObject GrassTile;
        public GameObject PathTile;
        public GameObject CurvedPathTile;
        public GameObject[] TowerTiles;
        public GameObject[] ObstacleTiles;
        // Parent object for the generated tiles
        public GameObject ParentObj;
        // Parent object for towers
        public GameObject TowerParentObj;
        // inventory
        public Inventory Items;
        // prefab controller
        private PrefabControl _prefabControl;
        // guard for null references
        public bool InventoryHasLoaded = false;
        /**
         * Create the inventory and get it ready for access
         */        
        void Start()
        {
            // create prefab controller
            _prefabControl = new PrefabControl(transform.gameObject, TowerParentObj, ParentObj, PathTile, CurvedPathTile, GrassTile, TowerTiles, ObstacleTiles)
            {
                // add methods
                Get = CloneUnityObject,
                End = RemoveUnityObject,
                Debug = print
            };
            // configuration data
            var configure = GetConfigureAsset();
            // check string
            if (configure == string.Empty)
            {
                // write error to console
                print("Unable to load the Inventory from the configuration file!!!");
                // exist
                return;
            }
            // create the inventory with the key store attached
            Items = new Inventory(configure, _prefabControl, print);
        }
        /**
         * Check inventory to make sure it has loaded and is available
         */
        private void Update()
        {
            // if inventory has already loaded, do nothing
            if (!InventoryHasLoaded)
            {
                // if inventory has not loaded yet, exit
                if (!Items.IsInventoryReady()) return;
                // toggles the guard
                InventoryHasLoaded = true;
                // calls user setup when ready
                UserSetup();
            }
        }
        /**
         * Use this for user setup!
         */
        private void UserSetup()
        {
            // add 200 gold default
            Items.SetWealth(200);
            // change the wealth UI container
            coinUI.text = Items.GetWealth().ToString(); // Added by Aleks
        }
        /**
         * Fetch the configuration file asset
         */   
        private string GetConfigureAsset()
        {
            // fetch the configure file
            var asset = Resources.Load<TextAsset>("Configure");
            // return the data as a string
            return !asset.IsUnityNull() ? asset.text : string.Empty;
        }
        // unity methods
        private static void RemoveUnityObject(GameObject original)
        {
            // destroy a prefab game object
            Destroy(original);
        }
        private static GameObject CloneUnityObject(GameObject original, Vector3 position, Quaternion rotation)
        {
            // return a game object from a prefab
            return Instantiate(original, position, rotation);
        }
    }
}
