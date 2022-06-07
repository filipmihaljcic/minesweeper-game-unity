using UnityEngine;
using UnityEngine.Tilemaps;

namespace Project.Scripts
{
    // this script will decide which tile to draw based on cell data 
    public class Board : MonoBehaviour
    {
        public Tilemap _tileMap {get; private set;}
        public Tile _tileUnknown; 
        public Tile _tileEmpty;
        public Tile _tileMine;
        public Tile _tileExplode;
        public Tile _tileFlag;
        public Tile _tileNum1;
        public Tile _tileNum2;
        public Tile _tileNum3;
        public Tile _tileNum4;
        public Tile _tileNum5;
        public Tile _tileNum6;
        public Tile _tileNum7;
        public Tile _tileNum8;
        
        private void Awake() 
        {
            _tileMap = GetComponent<Tilemap>();
        }

        // [,] indicates 2D array called _state
        public void Draw(Cell[,] _state) 
        {
            // get width and height of game board 
            int width = _state.GetLength(0);
            int height = _state.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // extract data at specific x and y coordinates
                    Cell _cell = _state[x, y];
                    // set tile at certain position and apply sprite as well
                    _tileMap.SetTile(_cell._position, GetTile(_cell));
                }
            }
        }

        private Tile GetTile(Cell _cell)
        {
            // if we clicked on cell
            if (_cell._isRevelead)
                // shuffle through all tiles that match 
                return GetReveleadTile(_cell);
    
            else if (_cell._isFlagged)
                return _tileFlag;
            
            else
                return _tileUnknown;
       }

        private Tile GetReveleadTile(Cell _cell)
        {
            // cycle through cell types and check which 
            // type they are and return adequate cell types 
            switch(_cell._type)
            {
                case Cell.Type.Empty: return _tileEmpty;
                case Cell.Type.Mine: return _cell._isExploded ? _tileExplode : _tileMine;
                case Cell.Type.Number: return GetNumberTile(_cell);
                default: return null;
            }
        }

        private Tile GetNumberTile(Cell _cell)
        {
            switch(_cell._number)
            {
                case 1: return _tileNum1;
                case 2: return _tileNum2;
                case 3: return _tileNum3;
                case 4: return _tileNum4;
                case 5: return _tileNum5;
                case 6: return _tileNum6;
                case 7: return _tileNum7;
                case 8: return _tileNum8;
                default: return null;
            }
        }
    }
}
