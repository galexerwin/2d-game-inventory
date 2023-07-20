using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Models.Inventory.Maps
{
    /**
     * Purpose: Create a map that the inventory can work with
     */
    public class Map
    {
        private int mapNumber = 1; //If we had multiple maps, this would be how we generate the paths.
        private Inventory _inventory; // a self-reference is passed in from the inventory
        private Node[,] _tiles = new Node[InventoryConstants.CONST_MAP_HEIGHT_TILES, InventoryConstants.CONST_MAP_WIDTH_TILES]; // create a tile space
        private int[,] pathTileLocations; //The x,y location of each path object
        private readonly float _mapPathTileCount;
        private readonly float _mapTopLeftX;
        private readonly float _mapTopLeftY;
        private readonly float _mapTileSize;
        private readonly int _mapHeight;
        private readonly int _mapWidth;
        private readonly float _obstacleRatio;
        public bool MapIsReady = false;
        /*
         * Constructor for the map
         */
        public Map(Inventory inventory)
        {
            // store the back-reference to inventory to use it's methods for assigning obstacles
            _inventory = inventory;
            // add map dimensions
            _mapHeight = InventoryConstants.CONST_MAP_HEIGHT_TILES;
            _mapWidth = InventoryConstants.CONST_MAP_WIDTH_TILES;
            // add obstacle ratio
            _obstacleRatio = InventoryConstants.CONST_OBSTACLE_GENERATION_PERCENTAGE;
            // add the path tile count for the map
            _mapPathTileCount = InventoryConstants.CONST_NUM_PATH_TILES_MAP1;
            // initialize the path tile locations
            pathTileLocations = new int[InventoryConstants.CONST_NUM_PATH_TILES_MAP1, InventoryConstants.CONST_XY_DIM_SIZE];
            // generate path locations
            PopulatePathLocations();            
            // initialize the map as empty
            EmptyMap();
            // populate the map tiles
            GenerateNewMap();
            // make map ready
            MapIsReady = true;
        }
        // place a tower
        public bool PlaceTower(int x, int y, GameObject towerToPlace, string sku)
        {
            // get the on hand quantity and placed quantities
            var onhand = _inventory.GetItemQuantity(sku);
            var placed = _inventory.GetItemPlacedCount(sku);
            // get the tower name
            var towerName = _inventory.GetItemName(sku);
            // make sure not default name
            /*if (towerName.Equals("Invalid"))
            {
                // error out
                _inventory.PrintMessage("Something went wrong! That sku does not match a tower.");
                // return false
                return false;                
            }
            // check if the user has sufficient quantity
            if (onhand < 1)
            {
                // error out
                _inventory.PrintMessage("You don't enough " + towerName + "s to place!");
                // return false
                return false;
            }
            // check if the user has placed all of their towers already
            if (onhand < placed + 1)
            {
                // error out
                _inventory.PrintMessage("All " + towerName + "s you own have been placed!");
                // return false
                return false;                
            }
            // check if the spot is full
            if (IsSpotFull(x, y))
            {
                // error out
                _inventory.PrintMessage("Can't place a tower here because something else is already there!");
                // return false
                return false;                
            }
            // attempt to fill the tile with that tower
            if (!FillTile(x, y, towerToPlace, sku))
            {
                // error out
                _inventory.PrintMessage("Could not place tower!");
                // return false
                return false;           
            }*/
            var toPlace = new Placeable();
            toPlace.InstallGameObject(towerToPlace, PlacementType.Tower);
            // create a node
            _tiles[y, x] = new Node();                
            // fill the node with a placeable
            _tiles[y, x].OccupyNode(toPlace);           
            // positive message
            _inventory.PrintMessage("A " + towerName + " was successfully placed at " + x + ", " + y + "!");
            // true
            return true;
        } 	
        /*
        * Checks if a spot is full on the map. Intakes the XY coordinates of the spot. Returns a boolean.
        */
        public void ok(string sku) => _inventory.ItemInsert(sku);
        /*
        * Checks if a spot is full on the map. Intakes the XY coordinates of the spot. Returns a boolean.
        */
        public bool IsSpotFull(int x, int y)
        {
            // if (MapIsReady)
            // {
            // _inventory.PrintMessage(_tiles[y, x].IsOccupied().ToString());
            // }
            return (_tiles[y, x] != null) && _tiles[y, x].IsOccupied();
        }
        /*
        * Checks if a spot is an obstacle on the map. Intakes the XY coordinates of the spot. Returns a boolean.
        */
        public bool IsSpotObstacle(int x, int y)
        {
            return _tiles[y, x].IsObstacle();
        }
        /*
        * Checks if a spot is a tower on the map. Intakes the XY coordinates of the spot. Returns a boolean.
        */
        public bool IsSpotTower(int x, int y)
        {
            return _tiles[y, x].IsTower();
        }
        /*
        * Returns the tile game object occupying a given x,y coordinate. Returns a game object.
        */
        public GameObject GetTileObjectInTile(int x, int y)
        {
            return _tiles[y, x].GetGameObject();
        }
        /*
        * Allows selling of an item in the tile if it is sellable
        */
        public bool SellTileObject(int x, int y)
        {
            // check if there is an item there and it's not a tile
            if (!IsSpotFull(x, y) || CheckIfTileIsPath(y, x)) return false;
            // reference to the node
            Node node = _tiles[y, x];
            // retrieve a placeable
            var placeable = _inventory.SellObstacle(node.GetPlaceable(), "GrassTile", x, y);
            // check if we retrieve a placeable and sold the item
            if (placeable == null) return false;
            // replace the contents with grass
            node.OccupyNode(placeable);
            // return true
            return true;
        }
        /*
        * Checks if a given tile is a path based on the x/y location 
        * Returns true or false
        */
        private bool CheckIfTileIsPath(int x, int y)
        {
            // iterate over the path array
            for (int i = 0; i < _mapPathTileCount; i++) {
                // if the spot is correct, return true to place a path tile
                if (pathTileLocations[i, 0] == x && pathTileLocations[i, 1] == y) {
                    // found a path tile
                    return true;
                }
            }
            // default
            return false;
        }        
        /*
        * Clears a tile 
        * Takes in the x,y location to empty.
        * Returns a boolean as to whether or not it was successful or not.
        */
        public bool EmptyTile(int x, int y)
        {
            // check if there is an item there and it's not a tile
            if (!IsSpotFull(x, y) || CheckIfTileIsPath(y, x) != false) return false;
            // reference to the node
            Node node = _tiles[y, x];
            // replace the contents with grass
            node.OccupyNode(_inventory.SwapPlaceableGameItem(node.GetPlaceable(), "GrassTile", x, y));
            // return true
            return true;
        }
        /*
        * Set all tiles to unoccupied.
        * Destroys references to any placed items
        */
        private void EmptyMap()
        {
            // iterate over the map
            for (int i = 0; i < _mapHeight; i++)
                for (int j = 0; j < _mapWidth; j++)
                    // clear the map (needs to unset on all the inventory if every used to clear the board)
                    _tiles[i, j] = null;
        }
        /*
        * Marks a tile as occupied. 
        * Takes in the x,y location and the object to fill the tile with.
        * Returns a boolean as to whether or not it was successful or not.
        */
        private bool FillTile(int x, int y, GameObject obj, string sku)
        {
            // retrieve a placeable
            var toPlace = _inventory.GetPlaceableTower(sku, obj);
            // check if null
            if (toPlace != null)
            {
                // create a node
                _tiles[y, x] = new Node();                
                // fill the node with a placeable
                _tiles[y, x].OccupyNode(toPlace); 
                // return
                return true;
            }
            // default
            return false;
        }       
        /*
        * Fills a tile with a map tile
        * Takes in the x,y location, and its type.
        */
        private void FillMapTile(int x, int y, PlacementType type)
        {
            // check the state before creating a node there
            if (IsSpotFull(x, y)) return;
            // sku generation
            string sku = string.Empty;
            // generate the sku from placement type
            switch (type)
            {
                case PlacementType.Grass:
                    sku = "GrassTile";
                    break;
                case PlacementType.Obstacle:
                    sku = "ObstacleTile";
                    break;
                case PlacementType.Path:
                    sku = GetPathType(y, x);
                    break;
                default:
                    sku = "GrassTile";
                    break;     
            }
            // retrieve a placeable
            var toPlace = _inventory.GetPlaceableGameItem(sku, x, y);
            // check if null
            if (toPlace != null)
            {
                // create a node
                _tiles[y, x] = new Node();                
                // fill the node with a placeable
                _tiles[y, x].OccupyNode(toPlace);                 
            }
        }
        /*
        * This method picks a path tile based on the tile position
        */    
        private string GetPathType(int i, int j)
        {
            // sku
            string sku;
            // follow pattern to generate a float
            switch (i)
            {
                // first curve (top left)
                case 1 when j == 2:
                    sku = "CurvedPathTile";
                    break;
                // second curve
                case 5 when j == 2:
                    sku = "CurvedPathTile";
                    break;
                // third curve
                case 5 when j == _mapWidth - 2:
                    sku = "CurvedPathTile";
                    break;
                // fourth curve (bottom right)
                case 8 when j == _mapWidth - 2:
                    sku = "CurvedPathTile";
                    break;
                // default
                default:
                {
                    // straight files that face down
                    if ((j == 2 && i is < 5 and > 1) || (i is > 5 and < 8 && j == _mapWidth - 2)) { sku = "PathTile"; }
                    // straight tiles that face right and left
                    else { sku = "PathTile"; }
                    break;
                }
            }
            // return the sku
            return sku;
        }
        /*
        * This method fills path locations
        */        
        private void PopulatePathLocations()
        {
            if (mapNumber == 1) {
                int counter = 0;
                //Top section
                for (int i = _mapWidth - 1; i > 2; i--)
                {
                    pathTileLocations[counter,0] = 1; // y location
                    pathTileLocations[counter,1] = i; //X location
                    counter++;
                }
                //Down facing section
                for (int i = 1; i < 6 ; i++)
                {
                    pathTileLocations[counter,0] = i;
                    pathTileLocations[counter,1] = 2;
                    counter++;
                }
                //middle section
                for (int i = _mapWidth - 3; i > 2; i--)
                {
                    pathTileLocations[counter,0] = 5; // y location
                    pathTileLocations[counter,1] = i; //X location
                    counter++;
                }
                //2nd downward section
                for (int i = 5; i < 8 ; i++)
                {
                    pathTileLocations[counter,0] = i;
                    pathTileLocations[counter,1] = _mapWidth - 2;
                    counter++;
                }
                //section towards the end
                for (int i = _mapWidth - 2; i >= 0; i--)
                {
                    pathTileLocations[counter,0] = 8; // y location
                    pathTileLocations[counter,1] = i; //X location
                    counter++;
                }
            }
        }        
        /*
        * This function Generates a new map.
        * Obstacles are procedurally added and the fixed path is generated.
        */
        private void GenerateNewMap()
        {
            for (int i = 0; i < _mapHeight; i++)
            {
                for (int j = 0; j < _mapWidth; j++)
                {
                    // check if we are working on the path
                    if (CheckIfTileIsPath(i, j))
                    {
                        // fill with path
                        FillMapTile(j, i, PlacementType.Path);
                        // loop again
                        continue;
                    }
                    // create a random float
                    float isObstacleTile = Random.Range(0.0f, 100.0f);
                    // fill with grass
                    if (isObstacleTile > _obstacleRatio)
                    {
                        // Create a grass tile
                        FillMapTile(j, i, PlacementType.Grass);
                        // loop again
                        continue;
                    }
                    // Create a removable obstacle (like a rock)
                    FillMapTile(j, i, PlacementType.Obstacle);
                }
            }
        }
    }
}