using UnityEngine;

namespace Project.Scripts
{
    public class Game : MonoBehaviour
    {
        [Header("Game Board Settings")]
        public int _width = 16;
        public int _height = 16;
        public int _mineAmmount = 32;
        private Board _board;
        private Cell [,] _state;
        private bool _gameover;

        private void OnValidate() 
        {
            _mineAmmount = Mathf.Clamp(_mineAmmount, 0, _width * _height);    
        }

        private void Awake() 
        {
            _board = GetComponentInChildren<Board>();
        }

        private void Start() 
        {
            NewGame();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                NewGame();

            else if (!_gameover)
            {
               if (Input.GetMouseButtonDown(1))
                  Flag();
            
               if (Input.GetMouseButtonDown(0))
                  Reveal();
           }
        }  

        private void NewGame() 
        {
            _state = new Cell[_width, _height];
            _gameover = false;

            GenerateCells();
            GenerateMines();
            GenerateNumbers();

            // set our camera position to be able to see board properly without clipping 
            Camera.main.transform.position = new Vector3(_width / 2f, _height / 2f, -10f);

            // update our board 
            _board.Draw(_state);
        }

        private void GenerateCells() 
        {
            for (int _x = 0; _x < _width; _x++)
            {
                for (int _y = 0; _y < _height; _y++)
                {
                    // create new cell 
                    Cell _cell = new Cell();
                    // assign position to new cell
                    _cell._position = new Vector3Int(_x, _y, 0);
                    // in first pass set cells to be empty 
                    _cell._type = Cell.Type.Empty;
                    // assign data to 2D array 
                    _state[_x, _y] = _cell;
                }
            }
        }
        private void GenerateMines() 
        {
            // in second pass we set mines 
            for (int _i = 0; _i < _mineAmmount; _i++)
            {
                int _x = Random.Range(0, _width);
                int _y = Random.Range(0, _height);

                // check if coordinates are the same as before 
                // to avoid setting the mine on the same cell 
                while (_state[_x, _y]._type == Cell.Type.Mine)
                {
                    _x++;

                    if (_x >= _width)
                    {
                        _x = 0;
                        _y++; 

                        if (_y >= _height)
                            _y = 0;   
                    }
                }
                _state[_x, _y]._type = Cell.Type.Mine;    
            } 
        }
        private void GenerateNumbers() 
        {
            for (int _x = 0; _x < _width; _x++)
            {
                for (int _y = 0; _y < _height; _y++)
                {
                    Cell _cell = _state[_x, _y];

                    if (_cell._type == Cell.Type.Mine)
                        continue;
                    
                    _cell._number = CountMines(_x, _y);

                    // set cell number to be 
                    // exactly what count of mine is 
                    if (_cell._number > 0)
                        _cell._type = Cell.Type.Number;

                    
                    _state[_x, _y] = _cell;
                }
            }
        }
        private int CountMines(int _cellX, int _cellY)
        {
            int _mineCount = 0;

            for (int _adjecentX = -1; _adjecentX <= 1; _adjecentX++)
            {
                for (int _adjecentY = -1; _adjecentY <= 1; _adjecentY++)
                {
                    // if we are on current cell it's not a mine
                    if (_adjecentX == 0 && _adjecentY == 0)
                        continue;

                    // this is like offset(moving left, right, above, bellow)
                    int _x = _cellX + _adjecentX;
                    int _y = _cellY + _adjecentY;

                    // check if indexes are out of bounds 
                    if (_x < 0 || _x >= _width || _y < 0 || _y >= _height)
                        continue;

                    // if mine is encountered on given coordinates incease mine counter
                    if (GetCell(_x, _y)._type == Cell.Type.Mine)
                        _mineCount++;
                }
            }
            return _mineCount;
        }
        private void Flag()
        {
            // convert screen position into world position 
            Vector3 _worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // convert world positon to cell position 
            Vector3Int _cellPosition = _board._tileMap.WorldToCell(_worldPosition);
            // set cell position 
            Cell _cell = GetCell(_cellPosition.x, _cellPosition.y);

            if (_cell._type == Cell.Type.Invalid || _cell._isRevelead)
                return;
            
            _cell._isFlagged = !_cell._isFlagged;
            _state[_cellPosition.x, _cellPosition.y] = _cell;
            _board.Draw(_state);
        }
        private void Reveal()
        {
            Vector3 _worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int _cellPosition = _board._tileMap.WorldToCell(_worldPosition);
            Cell _cell = GetCell(_cellPosition.x, _cellPosition.y);

            if (_cell._type == Cell.Type.Invalid || _cell._isRevelead || _cell._isFlagged)
                return;

            switch(_cell._type)
            {
                case Cell.Type.Mine:
                    Explode(_cell);
                    break;
                
                case Cell.Type.Empty:
                    Flood(_cell);
                    CheckWinCondition();
                    break;
                
                default:
                   _cell._isRevelead = true;
                   _state[_cellPosition.x, _cellPosition.y] = _cell;
                   CheckWinCondition();
                   break;
             }
            _board.Draw(_state);
        }

        private void Flood(Cell _cell)
        {
            // check if our cell is already revealed 
            if (_cell._isRevelead) return;

            // check if flooding came to mine cell or went out of bounds
            if (_cell._type == Cell.Type.Mine || _cell._type == Cell.Type.Invalid) return;

            // set our cell to be revaled
            _cell._isRevelead = true;
            // update our 2d array position 
            _state[_cell._position.x, _cell._position.y] = _cell;

            // check if that cell is empty and flood in all directions 
            if (_cell._type == Cell.Type.Empty)
            {
                Flood(GetCell(_cell._position.x - 1, _cell._position.y));
                Flood(GetCell(_cell._position.x + 1, _cell._position.y));
                Flood(GetCell(_cell._position.x, _cell._position.y - 1));
                Flood(GetCell(_cell._position.x, _cell._position.y + 1));
            }
        }

        private void Explode(Cell _cell)
        {
            Debug.Log("GameOver!");
            _gameover = true;

            _cell._isRevelead = true;
            _cell._isExploded = true;
            _state[_cell._position.x, _cell._position.y] = _cell;


            // we loop through our 2D array 
            // and reveal other mines if we 
            // clicked on mine before that 
            for (int _x = 0; _x < _width; _x++)
            {
                for (int _y = 0; _y < _height; _y++)
                {
                    _cell = _state[_x, _y];
                    
                    if (_cell._type == Cell.Type.Mine)
                    {
                        _cell._isRevelead = true;
                        _state[_x, _y] = _cell;
                    }
                }
            }
        }

        private void CheckWinCondition()
        {
            for (int _x = 0; _x < _width; _x++)
            {
                for (int _y = 0; _y < _height; _y++)
                {
                    Cell _cell = _state[_x, _y];

                    if (_cell._type != Cell.Type.Mine && !_cell._isRevelead)
                        return;
                }
            }

            Debug.Log("Winner.");
            _gameover = true;
        }
        private Cell GetCell(int _x, int _y) 
        {
            // if coordinates are ok 
            if (IsValid(_x, _y))
                // return a cell at given position 
                return _state[_x, _y];
            else
                return new Cell();
        }
        // checks for validity of x and y coordinates
        private bool IsValid(int _x, int _y)
        {
            return _x >= 0 && _x < _width && _y >= 0 && _y < _height;
        }
    }
}
