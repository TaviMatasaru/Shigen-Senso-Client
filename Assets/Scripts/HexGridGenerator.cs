using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject[] landPrefabs; // Array of prefabs for the lands
    public int width = 10;
    public int height = 10;
    public float hexWidth = 2.0f;
    public float hexHeight = 2.0f;
    private HexTile[,] hexGrid;
    private HexTile currentlySelectedTile;
    private Dictionary<LandType, GameObject> landPrefabMap; // Dictionary to map land types to prefabs
    // Weights for each type of land
    private int[] dynamicWeights = {70, 10, 10, 10}; // Corresponding to free land, mountains, forests, crops

    private void Start()
    {
        InitializeLandPrefabMap();
        hexGrid = new HexTile[width, height];
        dynamicWeights = new int[] {70, 10, 10, 10}; // Corresponding to free land, mountains, forests, crops
        GenerateGrid();
    }

    void InitializeLandPrefabMap()
    {
        landPrefabMap = new Dictionary<LandType, GameObject>
        {
            {LandType.FreeLand, landPrefabs[0]},
            {LandType.Mountain, landPrefabs[1]},
            {LandType.Forest, landPrefabs[2]},
            {LandType.Crops, landPrefabs[3]}
        };
    }

   void GenerateGrid()
    {
        // Calculate the horizontal spacing between the hex tiles
        float horizontalSpacing = hexWidth * Mathf.Sqrt(3) / 2;

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

                // Add a HexTile component to each hex and store it in the array
                HexTile tile = hex.AddComponent<HexTile>();
                tile.Initialize(landType, landPrefab, new Vector2Int(x, y), this);
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

    Vector3 CalculatePosition(int x, int y)
    {
        float horizontalSpacing = hexWidth * Mathf.Sqrt(3) / 2;
        float xPos = x * horizontalSpacing + (y % 2 == 1 ? horizontalSpacing / 2 : 0);
        float yPos = y * hexHeight * 0.75f;
        return new Vector3(xPos, 0, yPos);
    }

    LandType GetRandomLandType()
    {
        int totalWeight = 0;
        foreach (int weight in dynamicWeights)
        {
            totalWeight += weight;
        }

        int randomIndex = Random.Range(0, totalWeight);
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

    LandType GetLandTypeFromIdex(int index)
    {
        switch(index)
        {
            case 1 :
                return LandType.Mountain;
            case 2 :
                return LandType.Forest;
            case 3 :
                return LandType.Crops;
            default :
                return LandType.FreeLand;
        } 
    }

    public GameObject GetLandTypePrefab(LandType landType)
{
    switch(landType)
    {
        case LandType.Mountain:
            return landPrefabs[1];
        case LandType.Forest:
            return landPrefabs[2];
        case LandType.Crops:
            return landPrefabs[3];
        case LandType.Castle: 
            return landPrefabs[4];
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

    LandType DetermineLandType() {
        
        return LandType.FreeLand; // Placeholder
    }

}

public enum LandType {
    FreeLand,
    Mountain,
    Forest,
    Crops,
    Castle
}
