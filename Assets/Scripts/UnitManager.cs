
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public Unit unitPrefab;

    public Tile startTile;
    public Tile destinationTile;

    public void SpawnUnitAt(Tile startTile)
    {
        Vector3 spawnPosition = HexGridManager.Instance.CalculatePosition(startTile.tile.x, startTile.tile.y);
        Unit unit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        if (destinationTile != null)
        {
            List<Tile> path = HexGridManager.Instance.FindPath(startTile, destinationTile);
            unit.SetPath(path);
        }
    }
}