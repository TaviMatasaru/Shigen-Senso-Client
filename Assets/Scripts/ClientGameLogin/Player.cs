using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevelopersHub.RealtimeNetworking.Client;
using System;

public class Player : MonoBehaviour
{
    public Data.Player data = new Data.Player();
    public Data.Game game = new Data.Game();

    private static Player _instance = null; public static Player instance { get { return _instance; } }

    public Data.InitializationData initializationData = new Data.InitializationData();
   
    public enum RequestsID
    {
        LOGIN = 1,
        SYNC = 2,
        NEW_GRID = 3,
        SYNC_GRID = 4,
        BUILD_CASTLE = 5,
        BUILD_STONE_MINE = 6,
        BUILD_SAWMILL = 7,
        BUILD_FARM = 8,
        BUILD_ARMY_CAMP = 9,
        TRAIN = 10,
        CANCEL_TRAIN = 11,
        SEARCH = 12,
        CANCEL_SEARCH = 13,        
        UNIT_READY = 14,
        LAUNCH_ATTACK = 15,
        REGISTER = 16,
        AUTO_LOGIN = 17,
        LOGOUT = 18,
        LEAVE_MATCH = 19,
        SYNC_GAME = 20
    }

    public enum HexType
    {
        FREE_LAND = 0,
        FREE_MOUNTAIN = 1,
        FREE_FOREST = 2,
        FREE_CROPS = 3,
        PLAYER1_LAND = 4,
        PLAYER1_MOUNTAIN = 5,
        PLAYER1_FOREST = 6,
        PLAYER1_CROPS = 7,
        PLAYER1_CASTLE = 8,
        PLAYER1_STONE_MINE = 9,
        PLAYER1_SAWMILL = 10,
        PLAYER1_FARM = 11,
        PLAYER1_ARMY_CAMP = 12,
        PLAYER2_LAND = 13,
        PLAYER2_MOUNTAIN = 14,
        PLAYER2_FOREST = 15,
        PLAYER2_CROPS = 16,
        PLAYER2_CASTLE = 17,
        PLAYER2_STONE_MINE = 18,
        PLAYER2_SAWMILL = 19,
        PLAYER2_FARM = 20,
        PLAYER2_ARMY_CAMP = 21,
        PLAYER1_ARMY_CAMP_UNDER_ATTACK = 22,
        PLAYER2_ARMY_CAMP_UNDER_ATTACK = 23,
        PLAYER1_CASTLE_UNDER_ATTACK = 24,
        PLAYER2_CASTLE_UNDER_ATTACK = 25
    }

    bool connected = false;
    bool isFirstGrid = true;
    private double timer;

    public void Start()
    {
        RealtimeNetworking.OnPacketReceived += ReceivedPacket;
        ConnectToServer();
    }

    public void Awake()
    {
        _instance = this;
    }


    public void Update()
    {
        if (connected)
        {
            if(timer >= 1f)
            {
                timer = 0;
  
                if(instance.data.inGame == 1)
                {                    
                    Packet SyncPlayerPacket = new Packet();
                    SyncPlayerPacket.Write((int)RequestsID.SYNC);
                    Sender.TCP_Send(SyncPlayerPacket);

                    Packet SyncGridPacket = new Packet();
                    SyncGridPacket.Write((int)RequestsID.SYNC_GRID);
                    Sender.TCP_Send(SyncGridPacket);

                    Packet SyncGamePacket = new Packet();
                    SyncGamePacket.Write((int)RequestsID.SYNC_GAME);
                    SyncGamePacket.Write(instance.data.gameID);
                    Sender.TCP_Send(SyncGamePacket);

                }
                else
                {
                    Packet SyncPlayerPacket = new Packet();
                    SyncPlayerPacket.Write((int)RequestsID.SYNC);
                    Sender.TCP_Send(SyncPlayerPacket);
                }
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        
    }

    private void ConnectToServer()
    {
        RealtimeNetworking.OnConnectingToServerResult += ConnectionResponse;
        RealtimeNetworking.Connect();
    }

    private void ConnectionResponse(bool successfull)
    {
        if (successfull)
        {
            RealtimeNetworking.OnDisconnectedFromServer += DisconnectedFromServer;                      

            string device = SystemInfo.deviceUniqueIdentifier;
            Packet packet = new Packet();
            packet.Write((int)RequestsID.AUTO_LOGIN);
            packet.Write(device);
            Sender.TCP_Send(packet);
        }
        else
        {
            //TODO: Connection Failed message box with retry button
        }
        RealtimeNetworking.OnConnectingToServerResult -= ConnectionResponse;
    }


    private void ReceivedPacket(Packet received_packet)
    {        
        int id = received_packet.ReadInt();

        switch ((RequestsID)id)
        {
            case RequestsID.AUTO_LOGIN:
                string autoLoginData = received_packet.ReadString();

                Data.InitializationData autoLoginreceivedInitializationData = Data.Deserialize<Data.InitializationData>(autoLoginData);
                instance.initializationData.accountID = autoLoginreceivedInitializationData.accountID;
                instance.initializationData.username = autoLoginreceivedInitializationData.username;
              
                if (instance.initializationData.accountID == -2)
                {
                    UI_Main.instance._connectingToServerElements.SetActive(false);
                    UI_Main.instance._loginElements.SetActive(true);
                    
                }
                else
                {
                    UI_Main.instance._menuUsernameText.text = "Hello, " + instance.initializationData.username;
                    UI_Main.instance._connectingToServerElements.SetActive(false);
                    UI_Main.instance._menuElements.SetActive(true);

                    connected = true;
                    timer = 0;
                    HexGridManager.Instance.ResetGrid();
                    isFirstGrid = true;
                    
                }                                             
                
                break;

            case RequestsID.LOGIN:
                string loginData = received_packet.ReadString();

                Data.InitializationData receivedInitializationData = Data.Deserialize<Data.InitializationData>(loginData);
                instance.initializationData.accountID = receivedInitializationData.accountID;
                instance.initializationData.username = receivedInitializationData.username;
              

                if (instance.initializationData.accountID == -2)
                {
                    UI_Main.instance._loginErrorText.text = "Username or Password is incorrect";
                    UI_Main.instance._loginPasswordInput.text = "";
                }
                else
                {
                    if(instance.initializationData.accountID == -3)
                    {
                        UI_Main.instance._loginErrorText.text = "User is already online";
                        UI_Main.instance._loginPasswordInput.text = "";
                    }
                    else
                    {
                        UI_Main.instance._menuUsernameText.text = "Hello, " + instance.initializationData.username;
                        UI_Main.instance._menuElements.SetActive(true);
                        UI_Main.instance._loginElements.SetActive(false);

                        connected = true;
                        timer = 0;
                        HexGridManager.Instance.ResetGrid();
                        isFirstGrid = true;
                        
                    }
                }
                

                connected = true;
                timer = 0;
                break;

            case RequestsID.REGISTER:
                string registerData = received_packet.ReadString();

                Data.InitializationData registerReceivedInitializationData = Data.Deserialize<Data.InitializationData>(registerData);
                instance.initializationData.accountID = registerReceivedInitializationData.accountID;
                instance.initializationData.username = registerReceivedInitializationData.username;

                if (instance.initializationData.accountID == -2)
                {
                    UI_Main.instance._registerErrorText.text = "Username not available";
                    UI_Main.instance._registerPasswordInput.text = "";
                }
                else
                {
                    UI_Main.instance._menuUsernameText.text = "Hello, " + instance.initializationData.username;
                    UI_Main.instance._menuElements.SetActive(true);
                    UI_Main.instance._registerElements.SetActive(false);

                    connected = true;
                    HexGridManager.Instance.ResetGrid();
                    isFirstGrid = true;
                    timer = 0;
                }
                break;

            case RequestsID.LOGOUT:
                {
                    int logoutResponse = received_packet.ReadInt();
                    if(logoutResponse == 1)
                    {
                        UI_Main.instance._loginErrorText.text = "";
                        UI_Main.instance._loginUsernameInput.text = "";
                        UI_Main.instance._loginPasswordInput.text = "";

                        UI_Main.instance._menuElements.SetActive(false);
                        UI_Main.instance._loginElements.SetActive(true);

                        connected = false;
                        HexGridManager.Instance.ResetGrid();
                        isFirstGrid = true;
                    }
                    break;
                }

            case RequestsID.SYNC:               
                string playerData = received_packet.ReadString();
                Data.Player playerSyncData = Data.Deserialize<Data.Player>(playerData);               

                SyncPlayerData(playerSyncData);               
                break;

            case RequestsID.NEW_GRID:              
                string new_grid = received_packet.ReadString();
                Data.HexGrid newHexGrid = Data.Deserialize<Data.HexGrid>(new_grid);
                HexGridManager.Instance.GenerateHexGrid(newHexGrid);
                break;

            case RequestsID.SYNC_GRID:
                string syncGrid = received_packet.ReadString();
                Data.HexGrid syncHexGrid = Data.Deserialize<Data.HexGrid>(syncGrid);

                if(isFirstGrid)
                {
                    isFirstGrid = false;
                    HexGridManager.Instance.GenerateHexGrid(syncHexGrid);
                    UI_Main.instance.SetPlayerUIColor(Player.instance.data.isPlayer1);
                    UI_Main.instance._searchingElements.SetActive(false);
                    UI_Main.instance._elements.SetActive(true);
                }
                else
                {
                    HexGridManager.Instance.SyncHexGrid(syncHexGrid);
                }              
                break;

            case RequestsID.BUILD_CASTLE:
                int buildCastleResponse = received_packet.ReadInt();
                switch (buildCastleResponse)
                {
                    case 0:
                        Debug.LogError("Only one Castle Allowed!");
                        break;
                    case 1:
                        //RushSyncRequest();

                        Packet SyncGridPacket = new Packet();
                        HexGridManager.Instance._isCastleBuild = true;
                        SyncGridPacket.Write((int)RequestsID.SYNC_GRID);
                        Sender.TCP_Send(SyncGridPacket);
                        break;
                }
                break;

            case RequestsID.BUILD_STONE_MINE:
                int buildStoneMineResponse = received_packet.ReadInt();
                switch (buildStoneMineResponse)
                {
                    case 0:
                        Debug.LogError("You can only build on YOUR LAND");
                        break;

                    case 1:
                        RushSyncRequest();                       
                        break;

                    case 2:
                        Debug.LogError("You don't have enough resources!");
                        break;
                }
                break;

            case RequestsID.BUILD_SAWMILL:
                int buildSawmillResponse = received_packet.ReadInt();
                switch (buildSawmillResponse)
                {
                    case 0:
                        Debug.LogError("You can only build on YOUR LAND");
                        break;

                    case 1:
                        RushSyncRequest();                        
                        break;

                    case 2:
                        Debug.LogError("You don't have enough resources!");
                        break;
                }
                break;

            case RequestsID.BUILD_FARM:
                int buildFarmResponse = received_packet.ReadInt();
                switch (buildFarmResponse)
                {
                    case 0:
                        Debug.LogError("You can only build on YOUR LAND");
                        break;

                    case 1:
                        RushSyncRequest();                        
                        break;

                    case 2:
                        Debug.LogError("You don't have enough resources!");
                        break;
                }
                break;

            case RequestsID.BUILD_ARMY_CAMP:
                int buildArmyCampResponse = received_packet.ReadInt();
                switch (buildArmyCampResponse)
                {
                    case 0:
                        Debug.LogError("You can only build on FREE LAND");
                        break;

                    case 1:
                        Debug.LogError("You don't have enough resources!");                     
                        break;

                    case 2:
                        Debug.LogError("You can't build that far!");
                        break;

                    case 3:
                        RushSyncRequest();         
                        break;
                }
                break;

            case RequestsID.TRAIN:
                int trainResponse = received_packet.ReadInt();
                switch (trainResponse)
                {
                    case 0:
                        Debug.LogError("You can not train more units if your armycamp is attacking");
                        break;

                    case 1:
                        Debug.LogError("You don't have enough space in the ArmyCamp!");
                        break;

                    case 2:
                        Debug.LogError("You don't have enough food!");
                        break;

                    case 3:
                        RushSyncRequest();                                             

                        break;
                }
                break;

            case RequestsID.CANCEL_TRAIN:
                int cancelTrainResponse = received_packet.ReadInt();
                switch (cancelTrainResponse)
                {
                    case 0:
                        Debug.LogError("Unit not found!");
                        break;

                    case 1:
                        RushSyncRequest();
                        break;
                
                }
                break;

            case RequestsID.SEARCH:
                int startSearchResponse = received_packet.ReadInt();
                switch (startSearchResponse)
                {
                    case 0:
                        Debug.LogError("Can not start searching!");
                        break;

                    case 1:
                        SendPreSync();
                        break;

                }
                break;

            case RequestsID.UNIT_READY:
                int unitReadyResponse = received_packet.ReadInt();
                switch (unitReadyResponse)
                {
                    case 1:
                        Debug.LogError("Unit not on the target coords!");
                        break;

                    case 2:
                        RushSyncRequest();
                        break;

                }
                break;

            case RequestsID.LEAVE_MATCH:               
                int leaveMatchResponse = received_packet.ReadInt();
                if(leaveMatchResponse == 1)
                {
                    UI_InGameMenu.instance._elements.SetActive(false);
                    UI_InGameMenu.instance._endGameElements.SetActive(false);
                    UI_Main.instance._menuElements.SetActive(true);

                    HexGridManager.Instance.ResetGrid();
                    isFirstGrid = true;                    
                }
                else
                {
                    Debug.Log("Unknown response from LEAVE_MATCH");
                }
                break;

            case RequestsID.SYNC_GAME:
                string serilisedGame = received_packet.ReadString();
                Data.Game gameData = Data.Deserialize<Data.Game>(serilisedGame);
                SyncGameData(gameData);

                break;
        }

    }

    private void SyncPlayerData(Data.Player player)
    {
        UI_Main.instance._goldText.text = player.gold.ToString();
        UI_Main.instance._gemsText.text = player.gems.ToString();
        UI_Main.instance._woodText.text = player.wood.ToString();
        UI_Main.instance._stoneText.text = player.stone.ToString();
        UI_Main.instance._foodText.text = player.food.ToString();

        instance.data.gems = player.gems;
        instance.data.gold = player.gold;
        instance.data.stone = player.stone;
        instance.data.wood = player.wood;
        instance.data.food = player.food;

        instance.data.units = player.units;
       
        instance.data.hasCastle = player.hasCastle;
        instance.data.castle_x = player.castle_x;
        instance.data.castle_y = player.castle_y;

        instance.data.isOnline = player.isOnline;
        instance.data.isSearching = player.isSearching;
        instance.data.inGame = player.inGame;
        instance.data.gameID = player.gameID;
        instance.data.isPlayer1 = player.isPlayer1;         
    }

    private void SyncGameData(Data.Game gameData)
    {
        game.player1_username = gameData.player1_username;
        game.player2_username = gameData.player2_username;
        game.player1_victories = gameData.player1_victories;
        game.player2_victories = gameData.player2_victories;
        game.player1_rank = gameData.player1_rank;
        game.player2_rank = gameData.player2_rank;

        game.gameData = gameData.gameData;
     
        if(data.isPlayer1 == 1)
        {
            UI_Main.instance._opponentNameText.text = game.player2_username;
            UI_Main.instance._opponentVictoriesText.text = game.player2_victories.ToString();
            UI_Main.instance._opponentRankText.text = game.player2_rank.ToString();

            if(game.gameData.player2Status == Data.PlayerStatus.DISCONNECTED)
            {
                UI_Main.instance._opponentNameText.color = Color.red;
            }
        }
        else
        {
            UI_Main.instance._opponentNameText.text = game.player1_username;
            UI_Main.instance._opponentVictoriesText.text = game.player1_victories.ToString();
            UI_Main.instance._opponentRankText.text = game.player1_rank.ToString();

            if (game.gameData.player1Status == Data.PlayerStatus.DISCONNECTED)
            {
                UI_Main.instance._opponentNameText.color = Color.red;
            }
        }


        //TODO : result check to update UI

        switch (game.gameData.gameResult)
        {
            case Data.GameResultID.P1_WON:
                UI_Main.instance._elements.SetActive(false);
                UI_BuildingOptions.instance.SetStatus(false);
                UI_InGameMenu.instance._elements.SetActive(false);
                if (data.isPlayer1 == 1)
                {                   
                    UI_InGameMenu.instance._matchResultText.text = "YOU WON";
                    UI_InGameMenu.instance._resultReasonText.text = "Enemy Castle was Destroyed";                   
                }
                else
                {                   
                    UI_InGameMenu.instance._matchResultText.text = "YOU LOST";
                    UI_InGameMenu.instance._resultReasonText.text = "Your Castle was destroyed";                    
                }
                UI_InGameMenu.instance._endGameElements.SetActive(true);
                break;

            case Data.GameResultID.P2_WON:
                UI_Main.instance._elements.SetActive(false);
                UI_BuildingOptions.instance.SetStatus(false);
                UI_InGameMenu.instance._elements.SetActive(false);
                if (data.isPlayer1 == 0)
                {                    
                    UI_InGameMenu.instance._matchResultText.text = "YOU WON";
                    UI_InGameMenu.instance._resultReasonText.text = "Enemy Castle was Destroyed";                   
                }
                else
                {                    
                    UI_InGameMenu.instance._matchResultText.text = "YOU LOST";
                    UI_InGameMenu.instance._resultReasonText.text = "Your Castle was destroyed";                    
                }
                UI_InGameMenu.instance._endGameElements.SetActive(true);
                break;

            case Data.GameResultID.P1_LEFT:
                UI_Main.instance._elements.SetActive(false);
                UI_BuildingOptions.instance.SetStatus(false);
                UI_InGameMenu.instance._elements.SetActive(false);
                if (data.isPlayer1 == 0)
                {
                    UI_InGameMenu.instance._matchResultText.text = "YOU WON";
                    UI_InGameMenu.instance._resultReasonText.text = "Enemy left the match";
                }                
                UI_InGameMenu.instance._endGameElements.SetActive(true);
                break;

            case Data.GameResultID.P2_LEFT:
                UI_Main.instance._elements.SetActive(false);
                UI_BuildingOptions.instance.SetStatus(false);
                UI_InGameMenu.instance._elements.SetActive(false);
                if (data.isPlayer1 == 1)
                {
                    UI_InGameMenu.instance._matchResultText.text = "YOU WON";
                    UI_InGameMenu.instance._resultReasonText.text = "Enemy left the match";
                }
                UI_InGameMenu.instance._endGameElements.SetActive(true);
                break;
        }




        if(game.gameData.gameResult == Data.GameResultID.P1_WON)
        {
            if(data.isPlayer1 == 1)
            {
                UI_Main.instance._elements.SetActive(false);
                UI_BuildingOptions.instance.SetStatus(false);
                UI_InGameMenu.instance._elements.SetActive(false);

                UI_InGameMenu.instance._matchResultText.text = "YOU WON";
                UI_InGameMenu.instance._resultReasonText.text = "Enemy Castle was Destroyed";

                UI_InGameMenu.instance._endGameElements.SetActive(true);
            }
        }
    }

    private void RushSyncRequest()
    {
        Packet SyncGridPacket = new Packet();
        SyncGridPacket.Write((int)RequestsID.SYNC_GRID);
        Sender.TCP_Send(SyncGridPacket);

        Packet SyncPlayerPacket = new Packet();
        SyncPlayerPacket.Write((int)RequestsID.SYNC);
        Sender.TCP_Send(SyncPlayerPacket);
    }

    private void SendPreSync()
    {
        Packet SyncPlayerPacket = new Packet();
        SyncPlayerPacket.Write((int)RequestsID.SYNC);
        Sender.TCP_Send(SyncPlayerPacket);
    }

    private void DisconnectedFromServer()
    {
        RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
        //TODO: Connection failed message box with retry button
    }
}

