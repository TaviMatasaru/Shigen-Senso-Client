using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public LandType LandType { get; private set; }
    public GameObject LandPrefab { get; private set; }
    public Vector2Int GridPosition { get; private set; }
    public HexGridGenerator GridGenerator { get; set; } // Add a reference to the generator to communicate back
    private Vector3 originalScale; // To store the original scale
    private Vector3 originalPosition;

    public void Initialize(LandType landType, GameObject landPrefab, Vector2Int gridPosition, HexGridGenerator generator)
    {
        this.LandType = landType;
        this.LandPrefab = landPrefab;
        this.GridPosition = gridPosition;
        this.GridGenerator = generator;
    }

    void Awake()
    {
        // Cache the original scale and position
        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    void OnMouseDown()
    {
        GridGenerator.SelectTile(this);
    }

    public void Select()
{
        transform.localScale = originalScale * 1.3f;
        transform.position = new Vector3(originalPosition.x, originalPosition.y + 0.15f, originalPosition.z);
}

public void Deselect()
{
        transform.localScale = originalScale;
        transform.position = originalPosition;
}

public void ChangeToCastle()
{
    LandType = LandType.Castle; // Update the land type
    GameObject castlePrefab = GridGenerator.GetLandTypePrefab(LandType.Castle); // Assuming you've added a castle type
    Instantiate(castlePrefab, transform.position, Quaternion.identity, transform.parent);
    Destroy(gameObject); // Destroy the old land tile
}
}
