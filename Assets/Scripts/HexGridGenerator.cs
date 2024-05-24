using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{

    public static HexGridGenerator Instance { get; private set; } // Singleton instance

    public GameObject[] landPrefabs; // Array of prefabs for the lands
    public int width = 20;
    public int height = 20;
    public float hexWidth = 1.18f;
    public float hexHeight = 1.18f;
    private HexTile[,] hexGrid;
    private HexTile currentlySelectedTile;
    public bool _isCastleBuild = false;

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
        DontDestroyOnLoad(this.gameObject); // Optional: Makes it persist across scenes
    }


    public void GenerateGrid()
    {
        hexGrid = new HexTile[width, height];
        dynamicWeights = new int[] { 70, 10, 10, 10 };
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Get a random Land Type for the tile
                LandType landType = GetRandomLandType();
                GameObject landPrefab = GetLandTypePrefab(landType);

                // Instantiate the hex prefab at the calculated position
                GameObject hex = Instantiate(landPrefab, CalculatePosition(x, y), Quaternion.identity);
                hex.transform.SetParent(this.transform);

                //Add a HexTile component to each hex and store it in the array
                Vector2Int gridPosition = new Vector2Int(x, y);
                HexTile tile = hex.AddComponent<HexTile>();
                tile.Initialize(landType, gridPosition);
                hexGrid[x, y] = tile; // Store the HexTile in the array

            }
        }
    }

    public void SelectTile(HexTile tile)
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

    public HexTile GetCurrentlySelectedTile()
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

    public LandType GetRandomLandType()
    {
        int totalWeight = 0;
        foreach (int weight in dynamicWeights)
        {
            totalWeight += weight;
        }

        int randomIndex = UnityEngine.Random.Range(0, totalWeight);
        int sum = 0;

        for (int i = 0; i < dynamicWeights.Length; i++)
        {
            sum += dynamicWeights[i];
            if (randomIndex < sum)
            {
                AdjustWeights(i);
                LandType landType = GetLandTypeFromIdex(i);
                return landType;
            }
        }

        return LandType.FreeLand; // Should never happen unless weights are misconfigured
    }

    public LandType GetLandTypeFromIdex(int index)
    {
        switch (index)
        {
            case 1:
                return LandType.Mountain;
            case 2:
                return LandType.Forest;
            case 3:
                return LandType.Crops;
            default:
                return LandType.FreeLand;
        }
    }

    public GameObject GetLandTypePrefab(LandType landType)
    {
        switch (landType)
        {
            case LandType.Mountain:
                return landPrefabs[1];
            case LandType.Forest:
                return landPrefabs[2];
            case LandType.Crops:
                return landPrefabs[3];
            case LandType.Castle:
                return landPrefabs[4];
            case LandType.GoldMine:
                return landPrefabs[5];
            case LandType.Sawmill:
                return landPrefabs[6];
            case LandType.Farm:
                return landPrefabs[7];
            case LandType.ArmyCamp:
                return landPrefabs[8];
            case LandType.PlayerFreeLand:
                return landPrefabs[9];
            case LandType.PlayerForest:
                return landPrefabs[10];
            case LandType.PlayerCrops:
                return landPrefabs[11];
            case LandType.PlayerMountain:
                return landPrefabs[12];
            default:
                return landPrefabs[0]; // Free Land
        }
    }

    void AdjustWeights(int resourceIndex)
    {
        // Define the adjustment factor
        int decreaseAmount = 2;
        int increaseAmount = 1;

        if (resourceIndex != 0) // Not the free land
        {
            // Decrease the weight for the resource that was just placed
            dynamicWeights[resourceIndex] = Mathf.Max(1, dynamicWeights[resourceIndex] - decreaseAmount);

            // Increase the weight for the other resources
            for (int i = 1; i < dynamicWeights.Length; i++)
            {
                if (i != resourceIndex)
                {
                    dynamicWeights[i] += increaseAmount;
                }
            }
        }

    }

    public void ChangeTileLandType(HexTile tile, LandType newLandType)
    {
        Vector2Int gridPos = tile._gridPosition;

        Destroy(tile.gameObject);

        GameObject landPrefab = GetLandTypePrefab(newLandType);
        GameObject newHex = Instantiate(landPrefab, CalculatePosition(gridPos.x, gridPos.y), Quaternion.identity);
        newHex.transform.SetParent(this.transform);

        HexTile newTile = newHex.AddComponent<HexTile>();
        newTile.Initialize(newLandType, gridPos);

        hexGrid[gridPos.x, gridPos.y] = newTile;

        if (newTile._landType == LandType.GoldMine || newTile._landType == LandType.Sawmill || newTile._landType == LandType.Farm)
        {
            ResourceManager.instance.resourceGenerators.Add(newTile);
            ResourceManager.instance.UpdateResourceProductionRate();
        }
    }

    public void ChangeTileLandTypeToPlayer(HexTile tile)
    {
        switch (tile._landType)
        {
            case LandType.FreeLand:
                ChangeTileLandType(tile, LandType.PlayerFreeLand);
                break;
            case LandType.Forest:
                ChangeTileLandType(tile, LandType.PlayerForest);
                break;
            case LandType.Mountain:
                ChangeTileLandType(tile, LandType.PlayerMountain);
                break;
            case LandType.Crops:
                ChangeTileLandType(tile, LandType.PlayerCrops);
                break;
        }

    }


    public List<HexTile> Get2RingsOfNeighbours(HexTile centerTile)
    {
        List<HexTile> neighbors = new List<HexTile>();
        Vector2Int currentPosition = new Vector2Int();

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

        if (centerTile._gridPosition.y % 2 != 0)
        {
            foreach (Vector2Int direction in evenDirections)
            {
                currentPosition = centerTile._gridPosition;
                currentPosition += direction;
                if (currentPosition.x >= 0 && currentPosition.x < width && currentPosition.y >= 0 && currentPosition.y < height)
                {
                    HexTile neighbor = hexGrid[currentPosition.x, currentPosition.y];
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
                currentPosition = centerTile._gridPosition;
                currentPosition += direction;
                if (currentPosition.x >= 0 && currentPosition.x < width && currentPosition.y >= 0 && currentPosition.y < height)
                {
                    HexTile neighbor = hexGrid[currentPosition.x, currentPosition.y];
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }

    public List<HexTile> GetNeighbours(HexTile centerTile)
    {
        List<HexTile> neighbors = new List<HexTile>();
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

        if (centerTile._gridPosition.y % 2 != 0)
        {
            foreach (Vector2Int direction in evenDirections)
            {
                currentPosition = centerTile._gridPosition;
                currentPosition += direction;
                if (currentPosition.x >= 0 && currentPosition.x < width && currentPosition.y >= 0 && currentPosition.y < height)
                {
                    HexTile neighbor = hexGrid[currentPosition.x, currentPosition.y];
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
                currentPosition = centerTile._gridPosition;
                currentPosition += direction;
                if (currentPosition.x >= 0 && currentPosition.x < width && currentPosition.y >= 0 && currentPosition.y < height)
                {
                    HexTile neighbor = hexGrid[currentPosition.x, currentPosition.y];
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }

    public void TransformCastleNeighbors(HexTile castleTile)
    {
        List<HexTile> castleNeighbours = Get2RingsOfNeighbours(castleTile);

        foreach (HexTile neighbour in castleNeighbours)
        {
            ChangeTileLandTypeToPlayer(neighbour);
        }
    }

    public void TransformArmyCampNeighbors(HexTile castleTile)
    {
        List<HexTile> neighbours = GetNeighbours(castleTile);

        foreach (HexTile neighbour in neighbours)
        {
            ChangeTileLandTypeToPlayer(neighbour);
        }
        FillGaps();
    }

    public void FillGaps()
    {
        foreach(HexTile hex in hexGrid)
        {
            if(hex._landType == LandType.FreeLand || hex._landType == LandType.Mountain || hex._landType == LandType.Forest || hex._landType == LandType.Crops)
            {
                bool isGap = true;
                List<HexTile> neighbours = GetNeighbours(hex);
                foreach(HexTile neighbour in neighbours)
                {
                    if (neighbour._landType == LandType.FreeLand || neighbour._landType == LandType.Mountain || neighbour._landType == LandType.Forest || neighbour._landType == LandType.Crops)
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

public enum LandType
{
    FreeLand,
    Mountain,
    Forest,
    Crops,
    Castle,
    GoldMine,
    Sawmill,
    Farm,
    ArmyCamp,
    PlayerFreeLand,
    PlayerForest,
    PlayerCrops,
    PlayerMountain
}