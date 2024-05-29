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
                UI_BuildingOptions.instance._castleElement.SetActive(true);
            }
            else 
            {
                bool canPlace = false;
                List<Tile> neighbours = HexGridManager.Instance.Get2RingsOfNeighbours(this);
                foreach (Tile neighbour in neighbours)
                {
                    if (neighbour.tile.hexType == (int)Player.HexType.PLAYER_LAND || neighbour.tile.hexType == (int)Player.HexType.PLAYER_MOUNTAIN || neighbour.tile.hexType == (int)Player.HexType.PLAYER_FOREST || neighbour.tile.hexType == (int)Player.HexType.PLAYER_CROPS || neighbour.tile.hexType == (int)Player.HexType.PLAYER_STONE_MINE || neighbour.tile.hexType == (int)Player.HexType.PLAYER_SAWMILL || neighbour.tile.hexType == (int)Player.HexType.PLAYER_FARM)
                    {
                        if(this.tile.hexType == (int)Player.HexType.FREE_LAND)
                        {
                            canPlace = true;
                            break;
                        }                        
                    }
                }
                if (canPlace)
                {
                    UI_BuildingOptions.instance._armyCampElement.SetActive(true);
                }
                else
                {
                    UI_BuildingOptions.instance._armyCampElement.SetActive(false);
                    UI_BuildingOptions.instance._pathFindingTestElement.SetActive(true);
                }
            }
        }
        else if(this.tile.hexType == (int)Player.HexType.PLAYER_LAND)
        {            
            UI_BuildingOptions.instance._buildingElements.SetActive(true);
            UI_BuildingOptions.instance._armyCampElement.SetActive(false);
        }
        else if(this.tile.hexType == (int)Player.HexType.PLAYER_ARMY_CAMP)
        {
            UI_BuildingOptions.instance._openArmyCampElement.SetActive(true);
        }
        else
        {
            UI_BuildingOptions.instance._armyCampElement.SetActive(false);
            UI_BuildingOptions.instance._buildingElements.SetActive(false);
            UI_BuildingOptions.instance._castleElement.SetActive(false);
        }
    }

    public void Deselect()
    {
        transform.localScale = _originalScale;
        transform.position = _originalPosition;
        UI_BuildingOptions.instance.SetStatus(false);
    }

}
