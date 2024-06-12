using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevelopersHub.RealtimeNetworking.Client;
using System;

public class Player : MonoBehaviour
{
    public Data.Player data = new Data.Player();
    private static Player _instance = null; public static Player instance { get { return _instance; } }

    public Data.InitializationData initializationData = new Data.InitializationData();
   
    public enum RequestsID
    {
        AUTH = 1,
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
            if(timer >= 1)
            {
                timer = 0;
  
                if(instance.data.inGame == 1)
                {
                    Packet SyncGridPacket = new Packet();
                    SyncGridPacket.Write((int)RequestsID.SYNC_GRID);
                    Sender.TCP_Send(SyncGridPacket);

                    Packet SyncPlayerPacket = new Packet();
                    SyncPlayerPacket.Write((int)RequestsID.SYNC);
                    Sender.TCP_Send(SyncPlayerPacket);

                    //********DEBUG********
                    Debug.Log("Trimit Sync Request!!");

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
            string device = SystemInfo.deviceUniqueIdentifier + "_TEST_" + DateTime.Now;
            Packet packet = new Packet();
            packet.Write((int)RequestsID.AUTH);
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
            case RequestsID.AUTH:
                string authData = received_packet.ReadString();
                instance.initializationData = Data.Deserialize<Data.InitializationData>(authData);

                              
                Packet packetToSend = new Packet();

                packetToSend.Write((int)RequestsID.SYNC);           

                Sender.TCP_Send(packetToSend);

                connected = true;             
                timer = 0;
                break;

            case RequestsID.SYNC:               
                string playerData = received_packet.ReadString();
                Data.Player playerSyncData = Data.Deserialize<Data.Player>(playerData);


                //********DEBUG********
                Debug.Log("Am Primit Sync Request!!");

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
                        Debug.LogError("Unit not found!");
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

