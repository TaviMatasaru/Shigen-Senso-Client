using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public Data.HexTile tile = new Data.HexTile();
    private Vector3 _originalScale;
    private Vector3 _originalPosition;

    public void Initialize(Data.HexTile tile)
    {
        this.tile = tile;
    }

    void Awake()
    {
        _originalScale = transform.localScale;
        _originalPosition = transform.position;
    }

    void OnMouseDown()
    {        
        if (EventSystem.current.IsPointerOverGameObject())
            return;  
  
        HexGridManager.Instance.SelectTile(this);
    }

    public void Select()
    {
        transform.localScale = _originalScale * 1.3f;
        transform.position = new Vector3(_originalPosition.x, _originalPosition.y + 0.15f, _originalPosition.z);



        if(this.tile.hexType == (int)Player.HexType.FREE_LAND)
        {
            if(HexGridManager.Instance._isCastleBuild == false)
            {
                bool canPlaceCastle = true;
                List<Tile> neighbours = HexGridManager.Instance.Get2RingsOfNeighbours(this);
                foreach (Tile neighbour in neighbours)
                {
                    if (Player.instance.data.isPlayer1 == 1)
                    {
                        if (neighbour.tile.hexType == (int)Player.HexType.PLAYER2_LAND || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_MOUNTAIN || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_FOREST || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_CROPS || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_CASTLE || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_STONE_MINE || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_SAWMILL || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_FARM || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_ARMY_CAMP)
                        {
                            if (this.tile.hexType == (int)Player.HexType.FREE_LAND)
                            {
                                canPlaceCastle = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (neighbour.tile.hexType == (int)Player.HexType.PLAYER1_LAND || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_MOUNTAIN || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_FOREST || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_CROPS || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_CASTLE || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_STONE_MINE || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_SAWMILL || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_FARM || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_ARMY_CAMP)
                        {
                            if (this.tile.hexType == (int)Player.HexType.FREE_LAND)
                            {
                                canPlaceCastle = false;
                                break;
                            }
                        }
                    }
                }
                if (canPlaceCastle)
                {
                    UI_BuildingOptions.instance._castleElement.SetActive(true);
                }
                else
                {
                    UI_BuildingOptions.instance._castleElement.SetActive(false);
                }
                
            }
            else 
            {
                bool isOverlapping = false;
                bool isInRange = false;               
                List<Tile> neighbours = HexGridManager.Instance.Get2RingsOfNeighbours(this);
                foreach (Tile neighbour in neighbours)
                {
                    if (Player.instance.data.isPlayer1 == 1)
                    {
                        if (neighbour.tile.hexType == (int)Player.HexType.PLAYER2_LAND || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_MOUNTAIN || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_FOREST || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_CROPS || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_CASTLE || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_STONE_MINE || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_SAWMILL || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_FARM || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_ARMY_CAMP)
                        {
                            if (this.tile.hexType == (int)Player.HexType.FREE_LAND)
                            {
                                isOverlapping = true;
                                break;
                            }
                        }
                        if (neighbour.tile.hexType == (int)Player.HexType.PLAYER1_LAND || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_MOUNTAIN || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_FOREST || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_CROPS || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_CASTLE || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_STONE_MINE || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_SAWMILL || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_FARM || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_ARMY_CAMP)
                        {
                            if (this.tile.hexType == (int)Player.HexType.FREE_LAND)
                            {
                                isInRange = true;                                
                            }
                        }
                    }
                    else
                    {
                        if (neighbour.tile.hexType == (int)Player.HexType.PLAYER2_LAND || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_MOUNTAIN || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_FOREST || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_CROPS || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_CASTLE || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_STONE_MINE || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_SAWMILL || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_FARM || neighbour.tile.hexType == (int)Player.HexType.PLAYER2_ARMY_CAMP)
                        {
                            if (this.tile.hexType == (int)Player.HexType.FREE_LAND)
                            {
                                isInRange = true;
                            }
                        }
                        if (neighbour.tile.hexType == (int)Player.HexType.PLAYER1_LAND || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_MOUNTAIN || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_FOREST || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_CROPS || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_CASTLE || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_STONE_MINE || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_SAWMILL || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_FARM || neighbour.tile.hexType == (int)Player.HexType.PLAYER1_ARMY_CAMP)
                        {
                            if (this.tile.hexType == (int)Player.HexType.FREE_LAND)
                            {
                                isOverlapping = true;
                                break;
                            }
                        }
                    }                   
                }
                if (isOverlapping)
                {
                    UI_BuildingOptions.instance._armyCampElement.SetActive(false);
                }
                else
                {
                    if (isInRange)
                    {                                               
                            UI_BuildingOptions.instance._armyCampElement.SetActive(true);                       
                    }
                    else
                    {
                        UI_BuildingOptions.instance._armyCampElement.SetActive(false);
                    }
                   
                }
            }
        }
        else
        {
            if(Player.instance.data.isPlayer1 == 1)
            {
                if (this.tile.hexType == (int)Player.HexType.PLAYER1_LAND)
                {
                    UI_BuildingOptions.instance._buildingElements.SetActive(true);
                    UI_BuildingOptions.instance._armyCampElement.SetActive(false);
                }
                else if (this.tile.hexType == (int)Player.HexType.PLAYER1_ARMY_CAMP)
                {
                    UI_BuildingOptions.instance._openArmyCampElement.SetActive(true);
                }
                else if(this.tile.hexType == (int)Player.HexType.PLAYER2_ARMY_CAMP)
                {
                    if(UI_BuildingOptions.instance.selectingEnemyArmyCamp == true)
                    {
                        UI_BuildingOptions.instance._cancelAttackElement.SetActive(false);
                        UI_BuildingOptions.instance._launchAttackElement.SetActive(true);
                        HexGridManager.Instance.canSelectAnyTile = false;                        
                    }
                }
                else
                {
                    UI_BuildingOptions.instance._armyCampElement.SetActive(false);
                    UI_BuildingOptions.instance._buildingElements.SetActive(false);
                    UI_BuildingOptions.instance._castleElement.SetActive(false);
                    UI_BuildingOptions.instance._launchAttackElement.SetActive(false);
                }                
            }
            else
            {
                if (this.tile.hexType == (int)Player.HexType.PLAYER2_LAND)
                {
                    UI_BuildingOptions.instance._buildingElements.SetActive(true);
                    UI_BuildingOptions.instance._armyCampElement.SetActive(false);
                }
                else if (this.tile.hexType == (int)Player.HexType.PLAYER2_ARMY_CAMP)
                {
                    UI_BuildingOptions.instance._openArmyCampElement.SetActive(true);
                }
                else if (this.tile.hexType == (int)Player.HexType.PLAYER1_ARMY_CAMP)
                {
                    if (UI_BuildingOptions.instance.selectingEnemyArmyCamp == true)
                    {
                        UI_BuildingOptions.instance._cancelAttackElement.SetActive(false);
                        UI_BuildingOptions.instance._launchAttackElement.SetActive(true);
                        HexGridManager.Instance.canSelectAnyTile = false;                       
                    }                        
                }
                else
                {
                    UI_BuildingOptions.instance._armyCampElement.SetActive(false);
                    UI_BuildingOptions.instance._buildingElements.SetActive(false);
                    UI_BuildingOptions.instance._castleElement.SetActive(false);
                    UI_BuildingOptions.instance._launchAttackElement.SetActive(false);
                }               
            }
        }
        
    }
   

    public void Deselect()
    {
        transform.localScale = _originalScale;
        transform.position = _originalPosition;
        UI_BuildingOptions.instance.SetStatus(false);
    }

}
