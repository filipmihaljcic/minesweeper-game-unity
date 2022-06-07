using UnityEngine;

namespace Project.Scripts
{
    /*we are defining properties that make our cells
    this can be viewed as data structure so struct 
    is used(basically cell data is 2D array)*/
    public struct Cell 
    {
        /* using enum we determine what 
        types of cell we are using so we 
        enumerate three types of cell state*/ 
        public enum Type
        {
            Invalid,
            Empty,
            Mine,
            Number,
        }
        
        public Type _type;  // after creating enum we create property of Type enum
        public Vector3Int _position; // position of the cell within the board 
        public int _number; // what cell number is it(1, 2, 5 etc.)
        public bool _isRevelead; // check if cell is revelead 
        public bool _isFlagged;  // check if user put flag on cell
        public bool _isExploded; // check if mine exploded when user clicked on cell
    }
}
