using UnityEngine;

namespace Models.Inventory.Layout
{
    /**
     * Column in the inventory table
     */
    public class Column
    {
        private bool _isGrayedOut;
        private Vector3 _position;
        private Cell _cell;
        /**
         * Constructor for building the column
         */
        public Column(bool isGrayedOut, Vector3 position, Cell cell)
        {
            _isGrayedOut = isGrayedOut;
            _position = position;
            _cell = cell;
        }        
    }
}