using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridManager : MonoBehaviour
{

    public static HexGridManager Instance { get; private set; } // Singleton instance

    public GameObject[] hexPrefabs; // Array of prefabs for the lands
    public int width = 20;
    public int height = 20;
    public float hexWidth = 1.18f;
    public float hexHeight = 1.18f;
    //private HexTile[,] hexGrid;
    private Tile currentlySelectedTile;
    public bool _isCastleBuild = false;

    public Data.HexGrid Grid;
    public Tile[,] hexGrid;

    // Weights for each type of land
    private int[] dynamicWeights = { 70, 10, 10, 10 }; // Corresponding to free land, mountains, forests, crops


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); // Ensures that there aren't multiple instances
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); 
    }


    public void GenerateHexGrid(Data.HexGrid grid)
    {
        hexGrid = new Tile[grid.rows, grid.columns];
        foreach(Data.HexTile tile in grid.hexTiles)
        {
            GameObject hexPrefab = hexPrefabs[tile.hexType];
            int x_pos = tile.x;
            int y_pos = tile.y;

            GameObject hex = Instantiate(hexPrefab, CalculatePosition(x_pos, y_pos), Quaternion.identity);
            hex.transform.SetParent(this.transform);

            Tile hexTile = hex.AddComponent<Tile>();
            hexTile.Initialize(tile);

            hexGrid[x_pos, y_pos] = hexTile;
        }
    }

    public void SyncHexGrid(Data.HexGrid grid)
    {
        foreach(Data.HexTile tile in grid.hexTiles)
        {
            int x_pos = tile.x;
            int y_pos = tile.y;
            if(hexGrid[x_pos, y_pos].tile.hexType != tile.hexType)
            {
                ChangeTileHexType(hexGrid[x_pos, y_pos], (Player.HexType)tile.hexType);
            }
        }
    }

    public void SelectTile(Tile tile)
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

    public Tile GetCurrentlySelectedTile()
    {
        return currentlySelectedTile;
    }

    Vector3 CalculatePosition(int x, int y)
    {
        float horizontalSpacing = hexWidth * Mathf.Sqrt(3) / 2;
        float xPos = x * horizontalSpacing + (y % 2 == 1 ? horizontalSpacing / 2 : 0);
        float yPos = y * hexHeight * 0.75f;
        return new Vector3(xPos, 0, yPos);
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

        if (newTile.tile.hexType == (int)Player.HexType.PLAYER_STONE_MINE || newTile.tile.hexType == (int)Player.HexType.PLAYER_SAWMILL || newTile.tile.hexType == (int)Player.HexType.PLAYER_FARM)
        {
            ResourceManager.instance.resourceGenerators.Add(newTile);
            ResourceManager.instance.UpdateResourceProductionRate();
        }
    }

    public void ChangeTileLandTypeToPlayer(Tile hexTile)
    {
        switch (hexTile.tile.hexType)
        {
            case (int)Player.HexType.FREE_LAND:
                ChangeTileHexType(hexTile, Player.HexType.PLAYER_LAND);
                break;
            case (int)Player.HexType.FREE_FOREST:
                ChangeTileHexType(hexTile, Player.HexType.PLAYER_FOREST);
                break;
            case (int)Player.HexType.FREE_MOUNTAIN:
                ChangeTileHexType(hexTile, Player.HexType.PLAYER_MOUNTAIN);
                break;
            case (int)Player.HexType.FREE_CROPS:
                ChangeTileHexType(hexTile, Player.HexType.PLAYER_CROPS);
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

    public void TransformCastleNeighbors(Tile castleTile)
    {
        List<Tile> castleNeighbours = Get2RingsOfNeighbours(castleTile);

        foreach (Tile neighbour in castleNeighbours)
        {
            ChangeTileLandTypeToPlayer(neighbour);
        }
    }

    public void TransformArmyCampNeighbors(Tile castleTile)
    {
        List<Tile> neighbours = GetNeighbours(castleTile);

        foreach (Tile neighbour in neighbours)
        {
            ChangeTileLandTypeToPlayer(neighbour);
        }
        FillGaps();
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
