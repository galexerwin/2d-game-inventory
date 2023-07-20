using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
#pragma warning disable CS0414

namespace Models.Inventory
{
    // constants
    public static class InventoryConstants
    {
        // Map dimensions
        public const int CONST_MAP_WIDTH_TILES = 20;
        public const int CONST_MAP_HEIGHT_TILES = 10;
        // Map Constants
        public const float CONST_OBSTACLE_GENERATION_PERCENTAGE = 25.0f; //This constant tells us how much of the map will be obstacles.
        // The following two variables correspond to the x,y coords of the top left map tile.
        public const float CONST_X_STARTING_POS_TOP_LEFT = -9.5f;
        public const float CONST_Y_STARTING_POS_TOP_LEFT = 4.5f;
        public const float CONST_TILE_WIDTH = 1.0f; // Tiles are 1 unit x 1 unit squares.   
        // Path Information
        public const int CONST_NUM_PATH_TILES_MAP1 = 59; //53; //Number of path tiles in the map
        public const int CONST_XY_DIM_SIZE = 2; //How long the second array is, 1 for x and 1 for y    
        
    }
    // unity methods
    public delegate GameObject GetGameObject(GameObject original, Vector3 position, Quaternion rotation);
    public delegate void EndGameObject(GameObject original);
    public delegate void Print(string message);
    // prefab control
    public class PrefabControl
    {
        public GetGameObject Get; // instantiate
        public EndGameObject End; // destroy
        public Print Debug; // debug string print
        private Dictionary<string, GameObject> _keystore = new(); // key store for prefab references (other than obstacle)
        private GameObject _canvas; // canvas object using the controller
        private GameObject _towerParent;
        private GameObject _tilesParent;
        private GameObject[] _towerTiles;
        private GameObject[] _obstacleTiles;
        private readonly float _mapTopLeftX;
        private readonly float _mapTopLeftY;
        private readonly float _mapTileSize;
        private readonly int _mapHeight;
        private readonly int _mapWidth;
        public PrefabControl(GameObject canvas, GameObject towerParent, GameObject tilesParent, GameObject pathTile, 
            GameObject curvedPathTile, GameObject grassTile, GameObject[] towerTiles, GameObject[] obstacleTiles)
        {
            // add to instance
            _canvas = canvas;
            _towerParent = towerParent;
            _tilesParent = tilesParent;
            _towerTiles = towerTiles;
            _obstacleTiles = obstacleTiles;
            // add prefabs to the key store 
            _keystore.Add("GrassTile", grassTile);
            _keystore.Add("PathTile", pathTile);
            _keystore.Add("CurvedPathTile", curvedPathTile);
            // add map dimensions
            _mapHeight = InventoryConstants.CONST_MAP_HEIGHT_TILES;
            _mapWidth = InventoryConstants.CONST_MAP_WIDTH_TILES;
            // add world positions
            _mapTopLeftX = InventoryConstants.CONST_X_STARTING_POS_TOP_LEFT;
            _mapTopLeftY = InventoryConstants.CONST_Y_STARTING_POS_TOP_LEFT;
            // add tile size
            _mapTileSize = InventoryConstants.CONST_TILE_WIDTH;
        }
        // fill a placeable object with a new MAP game object
        public void InsertPrefabIntoPlaceable(Placeable placeable, int x, int y)
        {
            // retrieve the item attached
            Item item = placeable.ReturnItem();
            // retrieve the sku & placement type from the item
            PlacementType type = item.PlaceableType;
            string sku = item.SKU;
            // retrieve the prefab
            GameObject prefab = GetTile(sku);
            // make sure not null
            if (prefab != null)
            {
                // get the coordinates to place the tile
                Vector3 placeAtCoordinates = GenerateTileLocationRealCoordinates(x, y);
                // instantiate the tile
                GameObject tile = Get(prefab, placeAtCoordinates, Quaternion.identity);
                // set positioning if a path tile
                if (type == PlacementType.Path) SetPositioning(tile, y, x);
                // set the parent of the tile
                tile.transform.SetParent(_tilesParent.transform);
                // set the instanceID of the placeable
                placeable.InstanceID = sku + Convert.ToString(tile.GetInstanceID());
                // install the item in the placeable
                placeable.InstallGameObject(tile, type);
            }
        }
        // remove the prefab in placeable
        public void EndPrefabInPlaceable(Placeable placeable)
        {
            // destroy the game item
            End(placeable.ReturnGameObject());
        }
        /*
         * Set the positioning for a path tile
         * i = y, j = x
         */
        private void SetPositioning(GameObject path, int i, int j)
        {
            // angle for rotation
            float angle;
            // the caller euler angles
            Vector3 eulerAngles = _canvas.transform.eulerAngles;
            // follow pattern to generate a float
            switch (i)
            {
                // First curve (top left)
                case 1 when j == 2:
                    angle = 0f;
                    break;
                // Second curve
                case 5 when j == 2:
                    angle = 90f;
                    break;
                // Third curve
                case 5 when j == _mapWidth - 2:
                    angle = -90f;
                    break;
                
                case 8 when j == _mapWidth - 2:
                    angle = 180f;
                    break;
                default:
                {
                    if ((j == 2 && i is < 5 and > 1) || (i is > 5 and < 8 && j == _mapWidth - 2)) { angle = 90f; }
                    else { angle = 0f; }
                    break;
                }
            }
            // if the angle is not zero, do something to it
            if (angle != 0f) path.transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, angle);
        }
        // return map tile according to sku
        private GameObject GetTile(string sku)
        {
            // game object to return
            GameObject prefab;
            // retrieve appropriate prefab
            switch (sku)
            {
                case "ObstacleTile":
                    // retrieve prefab from obstacles array
                    prefab = GetRandomObstacle();
                    break;
                default:
                    // retrieve prefab from the key store
                    _keystore.TryGetValue(sku, out prefab);
                    break;
            }
            // return a prefab
            return prefab;
        }
        // return random obstacle prefab
        private GameObject GetRandomObstacle()
        {
            // get a random index based on array length
            var obstacleIndex = Mathf.RoundToInt(Mathf.Floor(Random.Range(0.0f, _obstacleTiles.Length)));
            // return randomized obstacle
            return _obstacleTiles[obstacleIndex];
        }
        /*
        * Translates the x,y coordinates of the grid to the real world position.
        * Returns Vector3
        */
        private Vector3 GenerateTileLocationRealCoordinates(int x, int y)
        {
            // calculate the real position
            float xPos = _mapTopLeftX + _mapTileSize * x;
            float yPos = _mapTopLeftY - _mapTileSize * y;
            // return
            return new Vector3(xPos, yPos, 0.0f);
        }        
    }
    public enum Elements
    {
        None,
        Life,
        Fire,
        Ice,
        Lightning,
        Poison
    }

    public enum Skills
    {
        Melee,
        ShortRange,
        LongRange,
        Advanced
    }

    public enum States
    {
        Unrefined,
        Refined,
        Final
    }

    public enum AdvancedSkills
    {
        Sonar,
        Windmill,
        Armory,
        Storage
    }

    public enum PlacementType
    {
        Tower,
        Obstacle,
        Path,
        Grass,
        Map,
        None
    }
}