using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuildingOptions : MonoBehaviour
{
    [SerializeField] public GameObject _buildingElements = null;
    [SerializeField] public GameObject _castleElement = null;
    [SerializeField] public GameObject _armyCampElement = null;
    [SerializeField] public Button _buildCastle = null;
    [SerializeField] public Button _buildGoldMine = null;
    [SerializeField] public Button _buildSawmill = null;
    [SerializeField] public Button _buildFarm = null;
    [SerializeField] public Button _builArmyCamp = null;

    

    private static UI_BuildingOptions _instance = null; public static UI_BuildingOptions instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
        _buildingElements.SetActive(false);
        _castleElement.SetActive(false);
        _armyCampElement.SetActive(false);
    }

    private void Start()
    {
        _buildCastle.onClick.AddListener(BuildCastleClicked);
        _buildGoldMine.onClick.AddListener(BuildGoldMineClicked);
        _buildSawmill.onClick.AddListener(BuildSawmillClicked);
        _buildFarm.onClick.AddListener(BuildFarmClicked);
        _builArmyCamp.onClick.AddListener(BuildArmyCampClicked);
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
                HexGridGenerator.Instance.TransformCastleNeighbors(selectedTile);
                _castleElement.SetActive(false);
            }
            else
            {
                Debug.Log("Only one castle allowed");
            }
        }
    }

    public void BuildGoldMineClicked()
    {

        HexTile selectedTile = HexGridGenerator.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null && ResourceManager.instance.CanBuyGoldMine())
        {
            ResourceManager.instance.BuyGoldMine();
            HexGridGenerator.Instance.ChangeTileLandType(selectedTile, LandType.GoldMine);           
            _buildingElements.SetActive(false);
            
        }
        else
        {
            Debug.LogError("No Resources to Build!");
        }
    }

    public void BuildSawmillClicked()
    {

        HexTile selectedTile = HexGridGenerator.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null && ResourceManager.instance.CanBuySawmill())
        {
            ResourceManager.instance.BuySawmill();
            HexGridGenerator.Instance.ChangeTileLandType(selectedTile, LandType.Sawmill);   
            _buildingElements.SetActive(false);
        }
        else
        {
            Debug.LogError("No Resources to Build!");
        }
    }

    public void BuildFarmClicked()
    {

        HexTile selectedTile = HexGridGenerator.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null && ResourceManager.instance.CanBuyFarm())
        {
            ResourceManager.instance.BuyFarm();
            HexGridGenerator.Instance.ChangeTileLandType(selectedTile, LandType.Farm);            
            _buildingElements.SetActive(false);
        }
        else
        {
            Debug.LogError("No Resources to Build!");
        }
    }

    public void BuildArmyCampClicked()
    {

        HexTile selectedTile = HexGridGenerator.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null && ResourceManager.instance.CanBuyArmyCamp())
        {
            ResourceManager.instance.BuyArmyCamp();
            HexGridGenerator.Instance.ChangeTileLandType(selectedTile, LandType.ArmyCamp);             
                HexGridGenerator.Instance.TransformArmyCampNeighbors(selectedTile);
                _armyCampElement.SetActive(false);
        }
        else
        {
            Debug.LogError("No Resources to Build!");
        }
    }

    public void SetStatus(bool status)
    {
        _buildingElements.SetActive(status);
        _castleElement.SetActive(status);
        _armyCampElement.SetActive(status);
    }
}
