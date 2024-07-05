
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private static UnitManager _instance = null; public static UnitManager instance { get { return _instance; } }

    public GameObject[] unitPrefabs;
    private double timer;

    public Dictionary<long, Unit> activeUnits = new Dictionary<long, Unit>();

    public void Awake()
    {
        _instance = this;
    }


    public void Update()
    {
        if (timer >= 1)
        {
            timer = 0;           
            SpawnNewUnits();
        }
        else
        {
            timer += Time.deltaTime;
        }


    }

    //public void SpawnNewUnits()
    //{
    //    if (Player.instance.data.hasCastle)
    //    {
    //        if (Player.instance.data.units.Count > 0)
    //        {
    //            foreach (Data.Unit unit in Player.instance.data.units)
    //            {
    //                if (unit.trained)
    //                {                        
    //                    bool containsUnit = activeUnits.ContainsKey(unit.databaseID);
    //                    if (!containsUnit)
    //                    {                            
    //                        SpawnUnit(unit);
    //                    }
    //                    else
    //                    {
    //                        activeUnits[unit.databaseID].unitData = unit;                            
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    public void SpawnNewUnits()
    {
        if(Player.instance.data.inGame == 1)
        {
            if (Player.instance.data.hasCastle)
            {
                HashSet<long> playerUnitIDs = new HashSet<long>(Player.instance.data.units.Select(u => u.databaseID));

                var toRemove = activeUnits.Keys.Where(id => !playerUnitIDs.Contains(id)).ToList();
                foreach (var id in toRemove)
                {
                    RemoveUnit(id);
                }

                foreach (Data.Unit unit in Player.instance.data.units)
                {
                    if (unit.trained && !activeUnits.ContainsKey(unit.databaseID))
                    {
                        SpawnUnit(unit);
                    }
                    else if (unit.trained && activeUnits.ContainsKey(unit.databaseID))
                    {
                        activeUnits[unit.databaseID].unitData = unit;
                    }
                }
            }
        }        
    }


    public void SpawnUnit(Data.Unit unitData)
    {
        Vector3 spawnPosition = HexGridManager.Instance.CalculatePosition(unitData.current_x, unitData.current_y);

        GameObject unitPrefab = unitPrefabs[(int)unitData.id];

        GameObject unitObject = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);

        unitObject.transform.SetParent(this.transform);

        Unit unit = unitObject.AddComponent<Unit>();
        unit.Initialize(unitData);


        activeUnits.Add(unit.unitData.databaseID, unit);
    }

    public void RemoveUnit(long unitID)
    {
        if (activeUnits.TryGetValue(unitID, out Unit unit))
        {
            Destroy(unit.gameObject);
            activeUnits.Remove(unitID);
        }
    }

    public void RemoveAllUnits()
    {
        foreach (Unit unit in activeUnits.Values)
        {
            Destroy(unit.gameObject);
            
        }
        activeUnits.Clear();
    }

    public enum Units
    {
        barbarian = 0,
        archer = 1
    }
}