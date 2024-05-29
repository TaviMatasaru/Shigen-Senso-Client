
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance { get; private set; }
    public Unit[] unitPrefabs;
    private List<Data.Unit> activeUnits = new List<Data.Unit>();


    public void Update()
    {
        if (HexGridManager.Instance._isCastleBuild)
        {
            if (Player.instance.data.units.Count > 0)
            {
                foreach (Data.Unit unit in Player.instance.data.units)
                {
                    if (unit.ready == true)
                    {
                        bool containsUnit = activeUnits.Any(u => u.databaseID == unit.databaseID);
                        if (!containsUnit)
                        {
                            activeUnits.Add(unit);
                            SpawnUnitAt(HexGridManager.Instance.castleTile, HexGridManager.Instance.hexGrid[unit.armyCamp_x, unit.armyCamp_y]);
                            Debug.Log("Spawn unit for armyCamp at x: " + unit.armyCamp_x + " y: " + unit.armyCamp_x);
                        }

                    }
                }
            }
        }
    }


    public void SpawnUnitAt(Tile startTile, Tile destinationTile)
    {
        Vector3 spawnPosition = HexGridManager.Instance.CalculatePosition(startTile.tile.x, startTile.tile.y);
        Unit unit = Instantiate(unitPrefabs[(int)Units.barbarian], spawnPosition, Quaternion.identity);
        if (destinationTile != null)
        {
            List<Tile> path = HexGridManager.Instance.FindPath(startTile, destinationTile);
            unit.SetPath(path);
        }
    }

    //public bool ContainsUnit(long databaseID)
    //{
    //    // Assuming activeUnits is an array of lists of Data.Unit
    //    return activeUnits.Any(list => list.Any(unit => unit.databaseID == databaseID));
    //}
}

public enum Units
{
    barbarian = 0,
    archer = 1
}