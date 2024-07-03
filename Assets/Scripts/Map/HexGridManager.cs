using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexGridManager : MonoBehaviour
{

    public static HexGridManager Instance { get; private set; }

    public GameObject[] hexPrefabs;
    public int width = 20;
    public int height = 20;
    public float hexWidth = 1.18f;
    public float hexHeight = 1.18f;
    private Tile currentlySelectedTile;
    public bool canSelectAnyTile = true;
    public Player.HexType SelectedTileTypeAllowed;    
    public bool _isCastleBuild = false;
    public bool gridGenerated = false;
    public Tile player1CastleTile;
    public Tile player2CastleTile;
    private bool player1CastleBuild = false;
    private bool player2CastleBuild = false;

    public int player1ArmyCampCount = 0;
    public int player2ArmyCampCount = 0;
    bool player1CastleFound = false;
    bool player2CastleFound = false;

    public Tile[,] hexGrid;
    public PathNode[,] pathGrid;    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
       // DontDestroyOnLoad(this.gameObject);
    }


    public void GenerateHexGrid(Data.HexGrid grid)
    {
        gridGenerated = false;

        hexGrid = new Tile[grid.rows, grid.columns];
        pathGrid = new PathNode[grid.rows, grid.columns];

        height = grid.rows;
        width = grid.columns;       

        foreach (Data.HexTile tile in grid.hexTiles)
        {
            GameObject hexPrefab = hexPrefabs[tile.hexType];
            int x_pos = tile.x;
            int y_pos = tile.y;


            GameObject hex = Instantiate(hexPrefab, CalculatePosition(x_pos, y_pos), Quaternion.identity);
            hex.transform.SetParent(this.transform);

            Tile hexTile = hex.AddComponent<Tile>();
            hexTile.Initialize(tile);

            hexGrid[x_pos, y_pos] = hexTile;          

            pathGrid[tile.x, tile.y] = new PathNode(hexTile);
        }
        gridGenerated = true;
    }

    public void ResetGrid()
    {
        if (hexGrid != null)
        {
            for (int x = 0; x < hexGrid.GetLength(0); x++)
            {
                for (int y = 0; y < hexGrid.GetLength(1); y++)
                {                    
                    if (hexGrid[x, y] != null)
                    {
                        Destroy(hexGrid[x, y].gameObject);
                        hexGrid[x, y] = null;
                    }
                    
                    pathGrid[x, y] = null;
                }
            }
        }

        hexGrid = new Tile[width, height];
        pathGrid = new PathNode[width, height];

        gridGenerated = false; 
    }


    public void SyncHexGrid(Data.HexGrid grid)
    {
        player1ArmyCampCount = 0;
        player2ArmyCampCount = 0;

        player1CastleFound = false;
        player2CastleFound = false;

        if(gridGenerated == true)
        {
            foreach (Data.HexTile tile in grid.hexTiles)
            {
                int x_pos = tile.x;
                int y_pos = tile.y;

                hexGrid[x_pos, y_pos].tile.stonePerSecond = tile.stonePerSecond;
                hexGrid[x_pos, y_pos].tile.woodPerSecond = tile.woodPerSecond;
                hexGrid[x_pos, y_pos].tile.foodPerSecond = tile.foodPerSecond;
                hexGrid[x_pos, y_pos].tile.health = tile.health;
                hexGrid[x_pos, y_pos].tile.capacity = tile.capacity;
                hexGrid[x_pos, y_pos].tile.attack = tile.attack;
                hexGrid[x_pos, y_pos].tile.defense = tile.defense;
                hexGrid[x_pos, y_pos].tile.isAttacking = tile.isAttacking;
                hexGrid[x_pos, y_pos].tile.isDefending = tile.isDefending;
                hexGrid[x_pos, y_pos].tile.isUnderAttack = tile.isUnderAttack;

                hexGrid[x_pos, y_pos].UpdateBuildingTexts();

                if (tile.hexType == (int)Player.HexType.PLAYER1_ARMY_CAMP)
                {
                    player1ArmyCampCount += 1;
                }

                if (tile.hexType == (int)Player.HexType.PLAYER2_ARMY_CAMP)
                {
                    player2ArmyCampCount += 1;
                }

                if (hexGrid[x_pos, y_pos].tile.hexType != tile.hexType)
                {
                    ChangeTileHexType(hexGrid[x_pos, y_pos], (Player.HexType)tile.hexType);
                    if (tile.hexType == (int)Player.HexType.PLAYER1_CASTLE)
                    {
                        if (Player.instance.data.isPlayer1 == 1)
                        {
                            player1CastleTile = hexGrid[x_pos, y_pos];
                            player1CastleBuild = true;
                        }

                    }
                    if (tile.hexType == (int)Player.HexType.PLAYER2_CASTLE)
                    {
                        if (Player.instance.data.isPlayer1 == 0)
                        {
                            player2CastleTile = hexGrid[x_pos, y_pos];
                            player2CastleBuild = true;
                        }

                    }
                }

                if (tile.hexType == (int)Player.HexType.PLAYER1_CASTLE)
                {
                    player1CastleBuild = true;
                    player1CastleFound = true;
                }
                if (tile.hexType == (int)Player.HexType.PLAYER2_CASTLE)
                {
                    player2CastleBuild = true;
                    player2CastleFound = true;
                }
            }        
        }                
    }

     

    public void SelectTile(Tile tile)
    {        
        if (canSelectAnyTile)
        {
            if (currentlySelectedTile != null)
                currentlySelectedTile.Deselect();

            if (currentlySelectedTile != tile)
            {
                currentlySelectedTile = tile;
                currentlySelectedTile.Select();
            }
            else
            {
                currentlySelectedTile = null;
            }
        }
        else
        {
            if(tile.tile.hexType == (int)SelectedTileTypeAllowed)
            {
                if (currentlySelectedTile != null)
                    currentlySelectedTile.Deselect();

                if (currentlySelectedTile != tile)
                {
                    currentlySelectedTile = tile;
                    currentlySelectedTile.Select();
                }
                else
                {
                    currentlySelectedTile = null;
                }
            }
        }
    }

    public Tile GetCurrentlySelectedTile()
    {
        return currentlySelectedTile;
    }


    public Vector3 CalculatePosition(int x, int y)
    {
        float horizontalSpacing = hexWidth * Mathf.Sqrt(3) / 2;
        float xPos = x * horizontalSpacing + (y % 2 == 1 ? horizontalSpacing / 2 : 0);
        float yPos = y * hexHeight * 0.75f;
        return new Vector3(xPos, 0, yPos);
    }

    public Vector3 CalculateUnitPosition(int x, int y)
    {
        float horizontalSpacing = hexWidth * Mathf.Sqrt(3) / 2;
        float xPos = x * horizontalSpacing + (y % 2 == 1 ? horizontalSpacing / 2 : 0);
        float yPos = y * hexHeight * 0.75f;
        return new Vector3(xPos, 0.2f, yPos);
    }


    public void ChangeTileHexType(Tile hexTile, Player.HexType hexType)
    {
        int x_pos = hexTile.tile.x;
        int y_pos = hexTile.tile.y;

        Destroy(hexTile.gameObject);

        GameObject landPrefab = hexPrefabs[(int)hexType];
        GameObject newHex = Instantiate(landPrefab, CalculatePosition(x_pos, y_pos), Quaternion.identity);
        newHex.transform.SetParent(this.transform);

        Tile newTile = newHex.AddComponent<Tile>();
        newTile.Initialize(hexTile.tile);
        newTile.tile.hexType = (int)hexType;

        hexGrid[x_pos, y_pos] = newTile;        
    }

    public void ChangeTileLandTypeToPlayer(Tile hexTile)
    {
        switch (Player.instance.data.isPlayer1)
        {
            case 1:
                switch (hexTile.tile.hexType)
                {
                    case (int)Player.HexType.FREE_LAND:
                        ChangeTileHexType(hexTile, Player.HexType.PLAYER1_LAND);
                        break;
                    case (int)Player.HexType.FREE_FOREST:
                        ChangeTileHexType(hexTile, Player.HexType.PLAYER1_FOREST);
                        break;
                    case (int)Player.HexType.FREE_MOUNTAIN:
                        ChangeTileHexType(hexTile, Player.HexType.PLAYER1_MOUNTAIN);
                        break;
                    case (int)Player.HexType.FREE_CROPS:
                        ChangeTileHexType(hexTile, Player.HexType.PLAYER1_CROPS);
                        break;
                }
                break;

            case 2:
                switch (hexTile.tile.hexType)
                {
                    case (int)Player.HexType.FREE_LAND:
                        ChangeTileHexType(hexTile, Player.HexType.PLAYER2_LAND);
                        break;
                    case (int)Player.HexType.FREE_FOREST:
                        ChangeTileHexType(hexTile, Player.HexType.PLAYER2_FOREST);
                        break;
                    case (int)Player.HexType.FREE_MOUNTAIN:
                        ChangeTileHexType(hexTile, Player.HexType.PLAYER2_MOUNTAIN);
                        break;
                    case (int)Player.HexType.FREE_CROPS:
                        ChangeTileHexType(hexTile, Player.HexType.PLAYER2_CROPS);
                        break;
                }
                break;
        }
       
    }


    public List<Tile> Get2RingsOfNeighbours(Tile centerHexTile)
    {
        List<Tile> neighbors = new List<Tile>();
        Vector2Int currentPosition;

        Vector2Int[] evenDirections =
        {
            new Vector2Int(0, +1), new Vector2Int(+1, +1), new Vector2Int(+1, 0),
            new Vector2Int(+1, -1), new Vector2Int(0, -1), new Vector2Int(-1, 0),
            new Vector2Int(0, +2), new Vector2Int(+1, +2), new Vector2Int(+2, +1),
            new Vector2Int(+2,0), new Vector2Int(+2, -1), new Vector2Int(+1, -2),
            new Vector2Int(0, -2), new Vector2Int(-1, -2), new Vector2Int(-1, -1),
            new Vector2Int(-2,0), new Vector2Int(-1, +1), new Vector2Int(-1, +2),
        };
        Vector2Int[] oddDirections =
        {
            new Vector2Int(-1, +1), new Vector2Int(0, +1), new Vector2Int(+1, 0),
            new Vector2Int(0,-1), new Vector2Int(-1, -1), new Vector2Int(-1, 0),
            new Vector2Int(0, +2), new Vector2Int(+1, +2), new Vector2Int(+1, +1),
            new Vector2Int(2,0), new Vector2Int(+1, -1), new Vector2Int(+1, -2),
            new Vector2Int(0, -2), new Vector2Int(-1, -2), new Vector2Int(-2, -1),
            new Vector2Int(-2,0), new Vector2Int(-2, +1), new Vector2Int(-1, +2),
        };

        if (centerHexTile.tile.y % 2 != 0)
        {
            foreach (Vector2Int direction in evenDirections)
            {
                //currentPosition = centerHexTile._gridPosition;
                currentPosition = new Vector2Int(centerHexTile.tile.x, centerHexTile.tile.y);
                currentPosition += direction;
                if (currentPosition.x >= 0 && currentPosition.x < width && currentPosition.y >= 0 && currentPosition.y < height)
                {
                    Tile neighbor = hexGrid[currentPosition.x, currentPosition.y];
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }
        else
        {
            foreach (Vector2Int direction in oddDirections)
            {
                //currentPosition = centerHexTile._gridPosition;
                currentPosition = new Vector2Int(centerHexTile.tile.x, centerHexTile.tile.y);
                currentPosition += direction;
                if (currentPosition.x >= 0 && currentPosition.x < width && currentPosition.y >= 0 && currentPosition.y < height)
                {
                    Tile neighbor = hexGrid[currentPosition.x, currentPosition.y];
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }

    public List<Tile> GetNeighbours(Tile centerHexTile)
    {
        List<Tile> neighbors = new List<Tile>();
        Vector2Int currentPosition = new Vector2Int();

        Vector2Int[] evenDirections =
        {
            new Vector2Int(0, +1), new Vector2Int(+1, +1), new Vector2Int(+1, 0),
            new Vector2Int(+1, -1), new Vector2Int(0, -1), new Vector2Int(-1, 0)
        };
        Vector2Int[] oddDirections =
        {
            new Vector2Int(-1, +1), new Vector2Int(0, +1), new Vector2Int(+1, 0),
            new Vector2Int(0,-1), new Vector2Int(-1, -1), new Vector2Int(-1, 0),
        };

        if (centerHexTile.tile.y % 2 != 0)
        {
            foreach (Vector2Int direction in evenDirections)
            {             
                currentPosition = new Vector2Int(centerHexTile.tile.x, centerHexTile.tile.y);
                currentPosition += direction;
                if (currentPosition.x >= 0 && currentPosition.x < width && currentPosition.y >= 0 && currentPosition.y < height)
                {
                    Tile neighbor = hexGrid[currentPosition.x, currentPosition.y];
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }
        else
        {
            foreach (Vector2Int direction in oddDirections)
            {                
                currentPosition = new Vector2Int(centerHexTile.tile.x, centerHexTile.tile.y);
                currentPosition += direction;
                if (currentPosition.x >= 0 && currentPosition.x < width && currentPosition.y >= 0 && currentPosition.y < height)
                {
                    Tile neighbor = hexGrid[currentPosition.x, currentPosition.y];
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }


    public void FillGaps()
    {
        foreach(Tile hex in hexGrid)
        {
            if(hex.tile.hexType == (int)Player.HexType.FREE_LAND || hex.tile.hexType == (int)Player.HexType.FREE_MOUNTAIN || hex.tile.hexType == (int)Player.HexType.FREE_FOREST || hex.tile.hexType == (int)Player.HexType.FREE_CROPS)
            {
                bool isGap = true;
                List<Tile> neighbours = GetNeighbours(hex);
                foreach(Tile neighbour in neighbours)
                {
                    if (neighbour.tile.hexType == (int)Player.HexType.FREE_LAND || neighbour.tile.hexType == (int)Player.HexType.FREE_MOUNTAIN || neighbour.tile.hexType == (int)Player.HexType.FREE_FOREST || neighbour.tile.hexType == (int)Player.HexType.FREE_CROPS)
                    {
                        isGap = false;
                        break;
                    }
                }
                if (isGap)
                {
                    ChangeTileLandTypeToPlayer(hex);
                }
            }
        }
    }   
}
