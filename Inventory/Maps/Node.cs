using UnityEngine;


namespace Models.Inventory.Maps
{
    public class Node
    {
        private bool _isOccupied;
        private Placeable _occupier;
        private PlacementType _type;

        public Node()
        {
            // node is not occupied on creation
            _isOccupied = false;
            // placement type is default
            _type = PlacementType.Map;
        }
        public bool OccupyNode(Placeable occupier)
        {
            // if occupier is null return
            if (occupier == null || occupier.ReturnGameObject() == null) return false;
            // retrieve the type from the item
            Item item = occupier.ReturnItem();
            // add the item to the node
            _occupier = occupier;
            // add the type of occupier
            _type = item?.PlaceableType ?? PlacementType.None;
            // set occupied state
            if (_type == PlacementType.Grass)
                _isOccupied = false;
            else
                _isOccupied = true;
            // return
            return true;
        }

        public Placeable GetPlaceable() => _occupier;
        public GameObject GetGameObject() => _occupier.ReturnGameObject();
        public bool IsOccupied() => _isOccupied;
        public bool IsGrass() => (_type == PlacementType.Grass);
        public bool IsObstacle() => (_type == PlacementType.Obstacle);
        public bool IsTower() => (_type == PlacementType.Tower);
        public bool IsPath() => (_type == PlacementType.Path);
    }
}