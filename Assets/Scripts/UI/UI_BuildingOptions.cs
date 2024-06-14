using System.Collections;
using System.Collections.Generic;
using DevelopersHub.RealtimeNetworking.Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuildingOptions : MonoBehaviour
{
    [SerializeField] public GameObject _buildingElements = null;
    [SerializeField] public GameObject _castleElement = null;
    [SerializeField] public GameObject _armyCampElement = null;
    [SerializeField] public GameObject _openArmyCampElement = null;
    [SerializeField] public GameObject _launchAttackElement = null;
    [SerializeField] public GameObject _cancelAttackElement = null;



    [SerializeField] public TextMeshProUGUI _capacityText = null;
    //[SerializeField] public TextMeshProUGUI _maxCapacityText = null; TODO: if have time
    [SerializeField] public TextMeshProUGUI _powerText = null;
    [SerializeField] public TextMeshProUGUI _defenseText = null;
    [SerializeField] public TextMeshProUGUI _yourAttackPowerText = null;
    [SerializeField] public TextMeshProUGUI _enemyDefenseText = null;
    [SerializeField] public TextMeshProUGUI _availableUnitsText = null;
    [SerializeField] public TextMeshProUGUI _selectedUnits = null;    


    [SerializeField] public Button _buildCastle = null;
    [SerializeField] public Button _buildStonedMine = null;
    [SerializeField] public Button _buildSawmill = null;
    [SerializeField] public Button _buildFarm = null;

    [SerializeField] public Button _builArmyCamp = null;
    [SerializeField] public Button _openArmyCamp = null;

    [SerializeField] public Button _trainBarbarianButton = null;
    [SerializeField] public Button _trainArcherButton = null;

    [SerializeField] public Button _attackButton = null;
    [SerializeField] public Button _launchAttackButton = null;
    [SerializeField] public Button _cancelAttackButton = null;
    [SerializeField] public Button _cancelLaunchAttackButton = null;
    [SerializeField] public Button _addUnitButton = null;
    [SerializeField] public Button _removeUnitButton = null;


    public bool selectingEnemyArmyCamp = false;
    public Tile attackingArmyCamp;
    public int availableUnits = 0;
    public int selectedUnits = 0;
    public int selectedUnitsAttack = 0;



    private static UI_BuildingOptions _instance = null; public static UI_BuildingOptions instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
        _buildingElements.SetActive(false);
        _castleElement.SetActive(false);
        _armyCampElement.SetActive(false);
        _openArmyCampElement.SetActive(false);
        _launchAttackElement.SetActive(false);
        _cancelAttackElement.SetActive(false);
    }

    private void Start()
    {
        _buildCastle.onClick.AddListener(BuildCastleClicked);
        _buildStonedMine.onClick.AddListener(BuildStoneMineClicked);
        _buildSawmill.onClick.AddListener(BuildSawmillClicked);
        _buildFarm.onClick.AddListener(BuildFarmClicked);
        _builArmyCamp.onClick.AddListener(BuildArmyCampClicked);

        _openArmyCamp.onClick.AddListener(OpenArmyCampClicked);
        _trainBarbarianButton.onClick.AddListener(TrainBarbarianClicked);
        _trainArcherButton.onClick.AddListener(TrainArcherClicked);

        _attackButton.onClick.AddListener(AttackButtonClicked);
        _launchAttackButton.onClick.AddListener(LaunchAttackButtonClicked);
        _cancelAttackButton.onClick.AddListener(CancelAttackButtonClicked);
        _cancelLaunchAttackButton.onClick.AddListener(CancelLaunchAttackButtonClicked);
        _addUnitButton.onClick.AddListener(AddUnitButtonClicked);
        _removeUnitButton.onClick.AddListener(RemoveUnitButtonClicked);

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
            packetToSend.Write(selectedTile.tile.x);
            packetToSend.Write(selectedTile.tile.y);
            Sender.TCP_Send(packetToSend);                

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
            packetToSend.Write(selectedTile.tile.x);
            packetToSend.Write(selectedTile.tile.y);
            Sender.TCP_Send(packetToSend);
           
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
            packetToSend.Write(selectedTile.tile.x);
            packetToSend.Write(selectedTile.tile.y);
            Sender.TCP_Send(packetToSend);
           
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
            packetToSend.Write(selectedTile.tile.x);
            packetToSend.Write(selectedTile.tile.y);
            Sender.TCP_Send(packetToSend);
           
            _armyCampElement.SetActive(false);
        }        
    }




    public void TrainBarbarianClicked()
    {
        Tile armyCampTile = HexGridManager.Instance.GetCurrentlySelectedTile();
        Packet packet = new Packet();

        packet.Write((int)Player.RequestsID.TRAIN);
        packet.Write(Data.UnitID.barbarian.ToString());
        packet.Write(armyCampTile.tile.x);
        packet.Write(armyCampTile.tile.y);

        Sender.TCP_Send(packet);
    }

    public void TrainArcherClicked()
    {
        Tile armyCampTile = HexGridManager.Instance.GetCurrentlySelectedTile();
        Packet packet = new Packet();

        packet.Write((int)Player.RequestsID.TRAIN);
        packet.Write(Data.UnitID.archer.ToString());
        packet.Write(armyCampTile.tile.x);
        packet.Write(armyCampTile.tile.y);

        Sender.TCP_Send(packet);
    }


    public void OpenArmyCampClicked()
    {
        _openArmyCampElement.SetActive(false);
        UI_Train.instance.SetStatus(true);
    }

    public void AttackButtonClicked()
    {
        _openArmyCampElement.SetActive(false);
        _cancelAttackElement.SetActive(true);

        attackingArmyCamp = HexGridManager.Instance.GetCurrentlySelectedTile();
        selectingEnemyArmyCamp = true;

        HexGridManager.Instance.canSelectAnyTile = false;

        if(Player.instance.data.isPlayer1 == 1)
        {
            HexGridManager.Instance.SelectedTileTypeAllowed = Player.HexType.PLAYER2_ARMY_CAMP;
        }
        else
        {
            HexGridManager.Instance.SelectedTileTypeAllowed = Player.HexType.PLAYER1_ARMY_CAMP;
        }
                
    }

    public void CancelAttackButtonClicked()
    {
        selectingEnemyArmyCamp = false;
        _cancelAttackElement.SetActive(false);

        HexGridManager.Instance.canSelectAnyTile = true;
    }

    public void AddUnitButtonClicked()
    {
        if(availableUnits > 0)
        {
            availableUnits -= 1;
            selectedUnits += 1;
            selectedUnitsAttack += Player.instance.data.units[1].damage;
        }       
    }

    public void RemoveUnitButtonClicked()
    {
        if (selectedUnits > 0)
        {
            availableUnits += 1;
            selectedUnits -= 1;
            selectedUnitsAttack -= Player.instance.data.units[1].damage;
        }
    }



    public void LaunchAttackButtonClicked()
    {

        Packet packet = new Packet();
        packet.Write((int)Player.RequestsID.LAUNCH_ATTACK);
        packet.Write(selectedUnits);
        packet.Write(attackingArmyCamp.tile.x);
        packet.Write(attackingArmyCamp.tile.y);
        packet.Write(HexGridManager.Instance.GetCurrentlySelectedTile().tile.x);
        packet.Write(HexGridManager.Instance.GetCurrentlySelectedTile().tile.y);
        Sender.TCP_Send(packet);

        //TODO: send the LAUNCH_ATTACK request

        selectingEnemyArmyCamp = false;
        _launchAttackElement.SetActive(false);

        HexGridManager.Instance.canSelectAnyTile = true;        
    }

    public void CancelLaunchAttackButtonClicked()
    {
        selectingEnemyArmyCamp = false;
        _launchAttackElement.SetActive(false);
        HexGridManager.Instance.canSelectAnyTile = true;
    }


    public void SetStatus(bool status)
    {
        _buildingElements.SetActive(status);
        _castleElement.SetActive(status);
        _armyCampElement.SetActive(status);
        _openArmyCampElement.SetActive(status);
    }
}
