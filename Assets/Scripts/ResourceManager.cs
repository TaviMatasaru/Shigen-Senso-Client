using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance { get; private set; }

    public Data.Player _player = new Data.Player();
    public List<Tile> resourceGenerators = new List<Tile>();

    public int _goldMineCost = 300;
    public int _farmCost = 300;
    public int _sawmillCost = 300;
    public int _armyCampCost = 500;

    public int _goldMineProductionRate = 10;
    public int _farmProductionRate = 10;
    public int _sawmillProductionRate = 10;

    private float _updateTimer = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject); // Ensures that there aren't multiple instances
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject); // Optional: Makes it persist across scenes
    }

    private void Update()
    {
        //if(_updateTimer > 1)
        //{
        //    _updateTimer = 0;
        //    UpdateResources();

           
        //}
        //else
        //{
        //    _updateTimer += Time.deltaTime;
        //}
        
    }


    public void UpdateResourceProductionRate()
    {
        foreach (Tile resourceGenerator in resourceGenerators)
        {
            List<Tile> neighbours = HexGridManager.Instance.GetNeighbours(resourceGenerator);

            if (resourceGenerator.tile.hexType == (int)Player.HexType.PLAYER_STONE_MINE)
            {
                foreach (Tile neighbour in neighbours)
                {
                    if (resourceGenerator.tile.hexType == (int)Player.HexType.PLAYER_MOUNTAIN)
                    {
                        _player.stoneProduction += _goldMineProductionRate;
                    }
                }
            }
            if (resourceGenerator.tile.hexType == (int)Player.HexType.PLAYER_SAWMILL)
            {
                foreach (Tile neighbour in neighbours)
                {
                    if (resourceGenerator.tile.hexType == (int)Player.HexType.PLAYER_FOREST)
                    {
                        _player.woodProduction += _sawmillProductionRate;
                    }
                }
            }
            if (resourceGenerator.tile.hexType == (int)Player.HexType.PLAYER_FARM)
            {
                foreach (Tile neighbour in neighbours)
                {
                    if (resourceGenerator.tile.hexType == (int)Player.HexType.PLAYER_CROPS)
                    {
                        _player.foodProduction += _farmProductionRate;
                    }
                }
            }
        }
    }   

    public void UpdateResources()
    {
        _player.stone += _player.stoneProduction;
        _player.wood += _player.woodProduction;
        _player.food += _player.foodProduction;

        UI_Main.instance._goldText.text = _player.gold.ToString();
        UI_Main.instance._gemsText.text = _player.gems.ToString();
        UI_Main.instance._woodText.text = _player.wood.ToString();
        UI_Main.instance._stoneText.text = _player.stone.ToString();
        UI_Main.instance._foodText.text = _player.food.ToString();
    }

    public void BuyGoldMine()
    {
        _player.wood -= _goldMineCost;
        _player.stone -= (_goldMineCost / 2);
    }

    public void BuySawmill()
    {
        _player.wood -= (_sawmillCost / 2);
        _player.stone -= _sawmillCost;
    }

    public void BuyFarm()
    {
        _player.wood -= _farmCost;
        _player.stone -= _farmCost;
    }

    public void BuyArmyCamp()
    {
        _player.wood -= _armyCampCost;
        _player.stone -= _armyCampCost;
    }

    public bool CanBuyGoldMine()
    {
        if(_player.stone >= (_goldMineCost/2) && _player.wood >= _goldMineCost)
        {
            return true;
        }
        return false;
    }

    public bool CanBuySawmill()
    {
        if (_player.stone >= _sawmillCost && _player.wood >= (_sawmillCost / 2))
        {
            return true;
        }
        return false;
    }

    public bool CanBuyFarm()
    {
        if (_player.stone >= _goldMineCost && _player.wood >= _goldMineCost)
        {
            return true;
        }
        return false;
    }

    public bool CanBuyArmyCamp()
    {
        if (_player.stone >= _armyCampCost && _player.wood >= _armyCampCost)
        {
            return true;
        }
        return false;
    }
}
