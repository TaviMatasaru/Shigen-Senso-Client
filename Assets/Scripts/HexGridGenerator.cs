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

    // Weights for each type of land
    private int[] dynamicWeights = {70, 10, 10, 10}; // Corresponding to free land, mountains, forests, crops

    private void Start()
    {
        dynamicWeights = new int[] {70, 10, 10, 10}; // Corresponding to free land, mountains, forests, crops
        GenerateGrid();
    }

   void GenerateGrid()
    {
        // Calculate the horizontal spacing between the hex tiles
        float horizontalSpacing = hexWidth * Mathf.Sqrt(3) / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate the offset for the odd or even columns
                float xPos = x * horizontalSpacing + (y % 2 == 1 ? horizontalSpacing / 2 : 0);
                
                // Adjust yPos for flat-topped hexes
                float yPos = y * hexHeight * 0.75f;

                // Instantiate the hex prefab at the calculated position
                GameObject hex = Instantiate(GetRandomLandPrefab(), new Vector3(xPos, 0, yPos), Quaternion.identity);
                hex.transform.SetParent(this.transform);
            }
        }
    }

    GameObject GetRandomLandPrefab()
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
                return landPrefabs[i];
            }
        }

        return null; // Should never happen unless weights are misconfigured
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

}
