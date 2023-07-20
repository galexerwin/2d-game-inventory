using UnityEngine;

namespace Models.Inventory.Layout
{
    /**
     * Class to add a panel prefab to the layout
     */    
    public class Tile : MonoBehaviour
    {
        // the prefab for our individual panel containing a sprite and its name
        [SerializeField] private Body _body;
        // on initializing this object, initialize a body object
        public void Initialize(Sprite sprite, string name)
        {
            // reference to our the inventory item panel
            var newObj = Instantiate(_body, transform);
            // initialize the panel with the requested sprite and name
            newObj.Initialize(sprite, name);
        }        
    }
}