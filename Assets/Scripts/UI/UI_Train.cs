using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Train : MonoBehaviour
{
    

    [SerializeField] public GameObject _elements = null;
    [SerializeField] private Button _closeButton = null;

    [SerializeField] private UI_UnitsTraining _trainPrefab = null;
    [SerializeField] private RectTransform _trainGrid = null;

    private List<UI_UnitsTraining> trainigItems = new List<UI_UnitsTraining>();
    [SerializeField] private List<UI_Unit> uiUnits = new List<UI_Unit>();

    private static UI_Train _instance = null; public static UI_Train instance { get { return _instance; } }

    private int armyCamp_x;
    private int armyCamp_y;


    [HideInInspector] public bool isOpen = false;

    private void ClearTrainingItems()
    {
        for (int i = 0; i < trainigItems.Count; i++)
        {
            if (trainigItems[i])
            {
                Destroy(trainigItems[i].gameObject);
            }
        }
        trainigItems.Clear();
    }

    private void Awake()
    {
        _instance = this;
        _elements.SetActive(false);
    }

    public void Initialize()
    {
        for (int i = 0; i < uiUnits.Count; i++)
        {
            for (int j = 0; j < Player.instance.data.units.Count; j++)
            {
                if (uiUnits[i].id == Player.instance.data.units[j].id)
                {
                    uiUnits[i].Initialize(Player.instance.initializationData.serverUnits[j]);
                    break;
                }
            }
        }
    }

    public void Sync()
    {
        for (int i = 0; i < uiUnits.Count; i++)
        {
            uiUnits[i].Sync(armyCamp_x, armyCamp_y);
        }

        for (int i = trainigItems.Count - 1; i >= 0; i--)
        {
            if (trainigItems[i])
            {
                if(Player.instance.data.units[i].armyCamp_x == armyCamp_x && Player.instance.data.units[i].armyCamp_y == armyCamp_y)
                {
                    int x = -1;
                    for (int j = 0; j < Player.instance.data.units.Count; j++)
                    {
                        if (Player.instance.data.units[j].databaseID == trainigItems[i].databaseID)
                        {
                            x = j;
                            break;
                        }
                    }
                    if (x >= 0)
                    {
                        //if (Player.instance.data.units[x].ready)
                        //{
                        //    RemoveTrainingItem(i);
                        //}
                    }
                    else
                    {
                        RemoveTrainingItem(i);
                    }
                }
                else
                {
                    RemoveTrainingItem(i);
                }
                
            }
            else
            {
                RemoveTrainingItem(i);
            }
        }
        UpdateTrainingList();
    }

    public void RemoveTrainingItem(int i)
    {
        if (i < 0 || i >= trainigItems.Count)
        {
            return;
        }
        if (trainigItems[i])
        {
            Destroy(trainigItems[i].gameObject);
        }
        trainigItems.RemoveAt(i);
        ResetTrainingItemsIndex();
    }

    public void ResetTrainingItemsIndex()
    {
        for (int j = 0; j < trainigItems.Count; j++)
        {
            if (trainigItems[j])
            {
                trainigItems[j].index = j;
            }
        }
    }

    private void Start()
    {
        _closeButton.onClick.AddListener(Close);
    }

    public void SetStatus(bool status)
    {
        ClearTrainingItems();
        isOpen = status;
        if (isOpen)
        {
            Tile armyCampTile = HexGridManager.Instance.GetCurrentlySelectedTile();
            armyCamp_x = armyCampTile.tile.x;
            armyCamp_y = armyCampTile.tile.y;
        }
        if (status)
        {          
            Sync();
        }
        _elements.SetActive(status);
        
    }

    private void UpdateTrainingList()
    {
        for (int i = 0; i < Player.instance.data.units.Count; i++)
        {
            //if (Player.instance.data.units[i].ready == false)
            //{
            //    int x = -1;
            //    for (int j = 0; j < trainigItems.Count; j++)
            //    {
            //        if (trainigItems[j] && Player.instance.data.units[i].databaseID == trainigItems[j].databaseID)
            //        {
            //            x = j;
            //            break;
            //        }
            //    }
            //    if (x >= 0)
            //    {

            //    }
            //    else
            //    {
            //        if (isOpen)
            //        {
            //            Tile selectedArmyCamp =  HexGridManager.Instance.GetCurrentlySelectedTile();
            //            if(Player.instance.data.units[i].armyCamp_x == selectedArmyCamp.tile.x && Player.instance.data.units[i].armyCamp_y == selectedArmyCamp.tile.y)
            //            {
            //                UI_UnitsTraining unit = Instantiate(_trainPrefab, _trainGrid.transform);
            //                unit.Initialize(Player.instance.data.units[i]);
            //                trainigItems.Add(unit);
            //            } 
            //        }                    
            //    }
            //}
        }
        ResetTrainingItemsIndex();
    }

    private void Close()
    {
        SetStatus(false);
        UI_Main.instance.SetStatus(true);
    }


}
