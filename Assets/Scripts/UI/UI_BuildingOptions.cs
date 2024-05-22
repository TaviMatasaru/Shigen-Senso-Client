using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuildingOptions : MonoBehaviour
{
    [SerializeField] public GameObject _buildingElements = null;
    [SerializeField] public GameObject _castleElement = null;
    [SerializeField] public Button _buildCastle = null;
    [SerializeField] public Button _buildGoldMine = null;
    [SerializeField] public Button _buildSawmill = null;
    [SerializeField] public Button _buildFarm = null;

    

    private static UI_BuildingOptions _instance = null; public static UI_BuildingOptions instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
        _buildingElements.SetActive(false);
        _castleElement.SetActive(false);
    }

    private void Start()
    {
        _buildCastle.onClick.AddListener(BuildCastleClicked);
        _buildGoldMine.onClick.AddListener(BuildGoldMineClicked);
        _buildSawmill.onClick.AddListener(BuildSawmillClicked);
        _buildFarm.onClick.AddListener(BuildFarmClicked);
    }

    public void BuildCastleClicked()
    {

        HexTile selectedTile = HexGridGenerator.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null)
        {
            if (HexGridGenerator.Instance._isCastleBuild == false)
            {
                HexGridGenerator.Instance.ChangeTileLandType(selectedTile, LandType.Castle);
                HexGridGenerator.Instance._isCastleBuild = true;
                HexGridGenerator.Instance.TransformNeighbors(selectedTile);
                _castleElement.SetActive(false);
            }
            else
            {
                Debug.Log("Only one castle allowed");
            }
        }
        else
        {
            Debug.LogError("No tile is currently selected when trying to build!");
        }
    }

    public void BuildGoldMineClicked()
    {

        HexTile selectedTile = HexGridGenerator.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null)
        {
            HexGridGenerator.Instance.ChangeTileLandType(selectedTile, LandType.GoldMine);
            _buildingElements.SetActive(false);
        }
        else
        {
            Debug.LogError("No tile is currently selected when trying to build!");
        }
    }

    public void BuildSawmillClicked()
    {

        HexTile selectedTile = HexGridGenerator.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null)
        {
            HexGridGenerator.Instance.ChangeTileLandType(selectedTile, LandType.Sawmill);
            _buildingElements.SetActive(false);
        }
        else
        {
            Debug.LogError("No tile is currently selected when trying to build!");
        }
    }

    public void BuildFarmClicked()
    {

        HexTile selectedTile = HexGridGenerator.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null)
        {
            HexGridGenerator.Instance.ChangeTileLandType(selectedTile, LandType.Farm);
            _buildingElements.SetActive(false);
        }
        else
        {
            Debug.LogError("No tile is currently selected when trying to build!");
        }
    }

    public void SetStatus(bool status)
    {
        _buildingElements.SetActive(status);
        _castleElement.SetActive(status);
    }
}
