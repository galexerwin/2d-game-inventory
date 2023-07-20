using UnityEngine;

namespace Models.Inventory.Layout
{
    /**
     * Class to add a tile prefab
     */    
    public class Cell : MonoBehaviour
    {
        // the prefab for the body border (tile)
        [SerializeField] private Tile _tile;
        // on initializing this object, initialize a tile
        public void Initialize(Sprite sprite, string name)
        {
            // reference to our the inventory item panel
            var newObj = Instantiate(_tile, transform);
            // initialize the panel with the requested sprite and name
            newObj.Initialize(sprite, name);
        }        
    }
}