using Unity.VisualScripting;
using UnityEngine;

namespace Models.Inventory
{
    public class Placeable
    {
        public string InstanceID;
        private PlacementType _instanceType;
        private Item _instanceItem;
        private GameObject _instanceResource;
        // return the type of placeable
        public PlacementType ReturnType() => _instanceType;
        // return the definition of the item
        public Item ReturnItem() => _instanceItem;
        // return the prefab representing the item
        public GameObject ReturnGameObject() => _instanceResource;
        // import a item definition
        public void InstallItem(Item item) => _instanceItem = item;
        // import a game object
        public void InstallGameObject(GameObject resource, PlacementType type)
        {
            // set the game object
            _instanceResource = resource;
            // set the type
            _instanceType = type;
        }
    }
}