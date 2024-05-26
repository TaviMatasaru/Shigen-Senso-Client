using System.Collections;
using System.Collections.Generic;
using DevelopersHub.RealtimeNetworking.Client;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuildingOptions : MonoBehaviour
{
    [SerializeField] public GameObject _buildingElements = null;
    [SerializeField] public GameObject _castleElement = null;
    [SerializeField] public GameObject _armyCampElement = null;
    [SerializeField] public Button _buildCastle = null;
    [SerializeField] public Button _buildStonedMine = null;
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
        _buildStonedMine.onClick.AddListener(BuildStoneMineClicked);
        _buildSawmill.onClick.AddListener(BuildSawmillClicked);
        _buildFarm.onClick.AddListener(BuildFarmClicked);
        _builArmyCamp.onClick.AddListener(BuildArmyCampClicked);
    }

    public void BuildCastleClicked()
    {

        Tile selectedTile = HexGridManager.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null)
        {
            if (HexGridManager.Instance._isCastleBuild == false)
            {
                Packet packetToSend = new Packet();
                packetToSend.Write((int)Player.RequestsID.BUILD_CASTLE);
                packetToSend.Write(SystemInfo.deviceUniqueIdentifier);
                packetToSend.Write(selectedTile.tile.x);
                packetToSend.Write(selectedTile.tile.y);
                Sender.TCP_Send(packetToSend);              
                
                _castleElement.SetActive(false);
            }           
        }
    }

    public void BuildStoneMineClicked()
    {

        Tile selectedTile = HexGridManager.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null)
        {
            Packet packetToSend = new Packet();
            packetToSend.Write((int)Player.RequestsID.BUILD_STONE_MINE);
            packetToSend.Write(SystemInfo.deviceUniqueIdentifier);
            packetToSend.Write(selectedTile.tile.x);
            packetToSend.Write(selectedTile.tile.y);
            Sender.TCP_Send(packetToSend);

            //ResourceManager.instance.BuyGoldMine();
            //HexGridManager.Instance.ChangeTileHexType(selectedTile, Player.HexType.PLAYER_STONE_MINE);           

            _buildingElements.SetActive(false);
            
        }        
    }

    public void BuildSawmillClicked()
    {

        Tile selectedTile = HexGridManager.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null)
        {
            Packet packetToSend = new Packet();
            packetToSend.Write((int)Player.RequestsID.BUILD_SAWMILL);
            packetToSend.Write(SystemInfo.deviceUniqueIdentifier);
            packetToSend.Write(selectedTile.tile.x);
            packetToSend.Write(selectedTile.tile.y);
            Sender.TCP_Send(packetToSend);

            //ResourceManager.instance.BuySawmill();
            //HexGridManager.Instance.ChangeTileHexType(selectedTile, Player.HexType.PLAYER_SAWMILL);

            _buildingElements.SetActive(false);
        }        
    }

    public void BuildFarmClicked()
    {

        Tile selectedTile = HexGridManager.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null)
        {
            Packet packetToSend = new Packet();
            packetToSend.Write((int)Player.RequestsID.BUILD_FARM);
            packetToSend.Write(SystemInfo.deviceUniqueIdentifier);
            packetToSend.Write(selectedTile.tile.x);
            packetToSend.Write(selectedTile.tile.y);
            Sender.TCP_Send(packetToSend);

            //ResourceManager.instance.BuyFarm();
            //HexGridManager.Instance.ChangeTileHexType(selectedTile, Player.HexType.PLAYER_FARM);            
            _buildingElements.SetActive(false);
        }        
    }

    public void BuildArmyCampClicked()
    {

        Tile selectedTile = HexGridManager.Instance.GetCurrentlySelectedTile();
        if (selectedTile != null)
        {
            Packet packetToSend = new Packet();
            packetToSend.Write((int)Player.RequestsID.BUILD_ARMY_CAMP);
            packetToSend.Write(SystemInfo.deviceUniqueIdentifier);
            packetToSend.Write(selectedTile.tile.x);
            packetToSend.Write(selectedTile.tile.y);
            Sender.TCP_Send(packetToSend);


            //ResourceManager.instance.BuyArmyCamp();
            //HexGridManager.Instance.ChangeTileHexType(selectedTile, Player.HexType.PLAYER_ARMY_CAMP);             
            //    HexGridManager.Instance.TransformArmyCampNeighbors(selectedTile);
            _armyCampElement.SetActive(false);
        }        
    }

    public void SetStatus(bool status)
    {
        _buildingElements.SetActive(status);
        _castleElement.SetActive(status);
        _armyCampElement.SetActive(status);
    }
}
