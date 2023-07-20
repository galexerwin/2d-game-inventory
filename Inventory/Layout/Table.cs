using System;
using System.IO;
using System.Collections.Generic;
using Models.Inventory.Configuration;
using UnityEngine;
//using Unity.UI;

namespace Models.Inventory.Layout
{
    /**
     * Inventory output table
     */
    public class Table : MonoBehaviour
    {
        private const int MaxColumnsPerRow = 4;
        private const string SpritesFolder = "Assets/Sprites";
        public Cell CellPrefab;
        private Column[,] _columns;

        public void CreateTable(List<Item> items)
        {
            // row count as int
            int maxRow = items.Count / MaxColumnsPerRow;
            // construct the height from the item counts & max columns per row
            int height = ((items.Count % MaxColumnsPerRow) == 0) ? maxRow : maxRow + 1;
            // copy width for clarity
            int width = MaxColumnsPerRow;
            // initialize an array of columns
            _columns = new Column[width, height];
            // column index
            int index = 0;
            // iterate over the rows
            for (int i = 0; i < height; i++)
            {
                // iterate over the columns
                for (int j = 0; j < width; j++)
                {
                    // there must be an item to retrieve
                    if (index >= items.Count)
                        // return
                        return;
                    // get a handle to the item
                    Item myItem = items[index];
                    print(myItem.Name);
                    // place the object in the world space?
                    Vector3 worldPosition = new Vector3(i, 0, j);
                    // clone a cell prefab object for inclusion
                    Cell newObj = Instantiate(CellPrefab, transform);
                    // create a sprite reference from the configuration
                    Sprite mySprite = RetrieveSprite(myItem.Image);
                    // initialize the cell object
                    newObj.Initialize(mySprite, myItem.Name);
                    // set the cell name
                    newObj.name = items[index].Name;
                    // create a node at the proper place
                    _columns[i, j] = new Column(false, worldPosition, newObj);
                    // increment
                    index++; 
                }
            }                 
        }
        public Sprite RetrieveSprite(SpritePath path)
        {
            // create a new empty sprite
            Sprite mySprite = null;
            // string path to sprite directory
            string fullPathToSprite = Directory.GetCurrentDirectory().Replace(@"\", "/") + "/" + SpritesFolder + "/" ;
            /*
             * Checks to see how to get the sprite
             * Sheet collects sprite at offset (preferred)
             * Path collects sprite from path directly
             * No else statement is specified because there isn't a file path configured
             *  if neither Sheet nor Path has a string length
             *  need to provide fixup in case of incorrect path specification
             */
            if (path.Sheet.Length > 0)
            {
                // build up full path to sprite sheet
                fullPathToSprite += path.Sheet;
                // load the sprite sheet
                Sprite[] slices = Resources.LoadAll<Sprite>(fullPathToSprite);
                // check the length to make sure offset is within bounds
                if (slices.Length > path.Offset)
                    // get the sprite at the offset
                    mySprite = slices[path.Offset];
            }
            else if (path.Path.Length > 0)
            {
                // build up full path to sprite
                fullPathToSprite += path.Path;
                // load the sprite path
                mySprite = Resources.Load<Sprite>(fullPathToSprite);
            }
            // return sprite
            return mySprite;
        }
    }
}