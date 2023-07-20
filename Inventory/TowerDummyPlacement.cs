using UnityEngine;

namespace Models.Inventory
{
    public class TowerDummyPlacement : MonoBehaviour
    {
        private float maxX; //max X coordinate
        private float maxY; //max Y coordinate
        private float minX; //min X coordinate
        private float minY; //min Y coordinate
        public MController mapMController;
        public GameObject towerToPlace;

        // Map cursor real-world xy location to the xy location in the 2D datastructures that control the tower map.
        public int currArrXPos;
        public int currArrYPos;

        // Current center of the cursor (in real world coordinates)
        public float currxCenter;
        public float curryCenter;
        
        protected string CONST_TOWER = "TOWER";
        protected string CONST_OBSTACLE = "OBSTACLE";

        private readonly float _mapHalfHeight;
        private readonly float _mapHalfWidth;
        private readonly float _mapTileSize;
        private readonly int _mapHeight;
        private readonly int _mapWidth;
        
        private TowerDummyPlacement()
        {
            // add constants
            // add map dimensions
            _mapHeight = InventoryConstants.CONST_MAP_HEIGHT_TILES;
            _mapWidth = InventoryConstants.CONST_MAP_WIDTH_TILES;
            // add tile size
            _mapTileSize = InventoryConstants.CONST_TILE_WIDTH;
            // half the map height & width
            _mapHalfHeight = (_mapHeight / 2f);
            _mapHalfWidth = (_mapWidth / 2f);
        }
        
        void Start()
        {
            //Cursor.visible = false; //TODO: Hide cursor in game view
            maxX = _mapHalfWidth * _mapTileSize;
            maxY = _mapHalfHeight * _mapTileSize;
            minX = -1.0f * maxX;
            minY = -1.0f * maxY;
            
        }

        // // Update is called once per frame
        // void Update()
        // {
        //     Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     if (Mathf.Abs(mousePos.x) < _mapHalfWidth && Mathf.Abs(mousePos.y) < _mapHalfHeight)
        //     {
        //         Cursor.visible = false;
        //     }
        //     else
        //     {
        //         Cursor.visible = true;
        //     }
        //     // float mouseXPos = Mathf.Round(mousePos.x * 2) / 2;
        //     // float mouseYPos = Mathf.Round(mousePos.y * 2) / 2;
        //     float mouseXPos = Mathf.Round(mousePos.x);
        //     float mouseYPos = Mathf.Round(mousePos.y);
        //     //X pos
        //     if (mouseXPos > maxX) {
        //         mouseXPos = maxX;
        //     }
        //     else if (mouseXPos < minX) {
        //         mouseXPos = minX;
        //     }
        //     //Y pos
        //     if (mouseYPos > maxY) {
        //         mouseYPos = maxY;
        //     }
        //     else if (mouseYPos < minY) {
        //         mouseYPos = minY;
        //     }
        //     //print(transform.position);
        //     //Now find the center of the tile to go to.
        //     float xCenter = mouseXPos;
        //     float yCenter = mouseYPos;
        //     // float xCenter = Mathf.Floor(mouseXPos) + 0.5f;
        //     // float yCenter = Mathf.Floor(mouseYPos) + 0.5f;
        //     if (mouseXPos > 0) 
        //     {
        //         xCenter += 0.5f;
        //     }
        //     else
        //     {
        //         xCenter += 0.5f;
        //     }
        //     if (mouseYPos > 0) 
        //     {
        //         yCenter -= 0.5f;
        //     }
        //     else
        //     {
        //         yCenter -= 0.5f;
        //     }
        //     
        //
        //     //Two edge cases
        //     if (yCenter <= -_mapHalfHeight)
        //     {
        //         yCenter = -_mapHalfHeight + 0.5f;
        //     }
        //     if (xCenter >= _mapHalfWidth)
        //     {
        //         xCenter = _mapHalfWidth - 0.5f;
        //     }
        //     //if (!isPlaced)
        //     //{
        //     transform.position = new Vector3(xCenter, yCenter, 0.0f); //mousePos.z);
        //     //}
        //
        //     currxCenter = xCenter;
        //     curryCenter = yCenter;
        //
        //     /*
        //     *
        //     *   HANDLING TOWER INTERACTIONS
        //     *       In order to handle tower interactions, we first need to get the array position.
        //     *       This is done by translating world coordinates into coordinates for our 2D data structures.
        //     *
        //     */
        //     int arrXPos = (int)(mouseXPos + _mapHalfWidth);
        //     int arrYPos = Mathf.Abs(-1 * (int)(mouseYPos + _mapHalfHeight));
        //     //Handle edge cases
        //     if (arrXPos == _mapWidth)
        //     {
        //         arrXPos--;
        //     }
        //     if (arrYPos == _mapHeight)
        //     {
        //         arrYPos--;
        //     }
        //     currArrXPos = arrXPos;
        //     currArrYPos = arrYPos;
        //
        //
        //     //Place towers
        //     if (Input.GetMouseButtonDown(0)) 
        //     {
        //         if (!mapController.IsSpotFull(currArrXPos, currArrYPos))
        //         {
        //             bool rc = PlaceTower(Instantiate(towerToPlace, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity));
        //             if (rc)
        //             {
        //                 print("Placed tower!");
        //             }
        //             else 
        //             {
        //                 print("Couldn't place tower!");
        //             }
        //         }
        //     }
        //     //Sell towers
        //     if (Input.GetMouseButtonDown(1))
        //     {
        //         bool rc = SellTower();
        //         if (rc)
        //         {
        //             print("sold tower successfully");
        //         }
        //         else
        //         {
        //             print("error selling tower");
        //         }
        //     }
        //
        //     //Sell obstacles
        //     if (Input.GetKeyDown((KeyCode.Space)))
        //     {
        //         bool rc = SellObstacle();
        //         if (rc)
        //         {
        //             print("sold obstacle successfully");
        //         }
        //         else
        //         {
        //             print("error selling obstacle");
        //         }
        //     }
        // }
        // /*
        // *   This function sells the tower at the current selected tile.
        // *   Returns true or false if it was successful.
        // */
        // public bool SellTower()
        // {
        //     // return result
        //     return mapController.SellTileObject(currArrXPos, currArrYPos);
        // }
        // /*
        // * This function sells the obstacle at the current selected tile.
        // * Returns true or false if it was successful.
        // */
        // public bool SellObstacle()
        // {
        //     // return result
        //     return mapController.SellTileObject(currArrXPos, currArrYPos);
        // }
        //
        // public bool PlaceTower(GameObject towerToPlace)
        // {
        //     /*
        //     *   This function places a tower at the current selected tile.
        //     *   Returns true or false if it was successful.
        //     *   Takes in the tower to place.
        //     */
        //     if (!mapController.IsSpotFull(currArrXPos, currArrYPos))
        //     {
        //         Vector3 tilePosition = new Vector3(currxCenter, curryCenter, 0.0f);
        //         towerToPlace.transform.position = tilePosition;
        //         mapController.PlaceTower(currArrXPos, currArrYPos, towerToPlace);
        //         return true;
        //     }
        //     else {
        //         return false;
        //     }
        // }        
    }
}