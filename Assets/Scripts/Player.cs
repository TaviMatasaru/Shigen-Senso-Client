using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevelopersHub.RealtimeNetworking.Client;

public class Player : MonoBehaviour
{
   
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
        BUILD_ARMY_CAMP = 9
    }

    public enum HexType
    {
        FREE_LAND = 0,
        FREE_MOUNTAIN = 1,
        FREE_FOREST = 2,
        FREE_CROPS = 3,
        PLAYER_LAND = 4,
        PLAYER_MOUNTAIN = 5,
        PLAYER_FOREST = 6,
        PLAYER_CROPS = 7,
        PLAYER_CASTLE = 8,
        PLAYER_STONE_MINE = 9,
        PLAYER_SAWMILL = 10,
        PLAYER_FARM = 11,
        PLAYER_ARMY_CAMP = 12
    }

    bool connected = false;
    private float timer;

    public void Start()
    {
        RealtimeNetworking.OnPacketReceived += ReceivedPacket;
        ConnectToServer();
    }

    public void Update()
    {
        if (connected)
        {
            if(timer >= 1)
            {
                timer = 0;

                Packet SyncGridPacket = new Packet();
                SyncGridPacket.Write((int)RequestsID.SYNC_GRID);
                SyncGridPacket.Write(SystemInfo.deviceUniqueIdentifier);
                Sender.TCP_Send(SyncGridPacket);

                Packet SyncPlayerPacket = new Packet();
                SyncPlayerPacket.Write((int)RequestsID.SYNC);
                SyncPlayerPacket.Write(SystemInfo.deviceUniqueIdentifier);
                Sender.TCP_Send(SyncPlayerPacket);
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
                long accountID = received_packet.ReadLong();                
                Packet packetToSend = new Packet();
                packetToSend.Write((int)RequestsID.SYNC);
                packetToSend.Write(SystemInfo.deviceUniqueIdentifier);
                Sender.TCP_Send(packetToSend);
                connected = true;
                timer = 0;
                break;

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
                HexGridManager.Instance.SyncHexGrid(syncHexGrid);
                break;

            case RequestsID.BUILD_CASTLE:
                int buildCastleResponse = received_packet.ReadInt();
                switch (buildCastleResponse)
                {
                    case 0:
                        Debug.LogError("Only one Castle Allowed!");
                        break;
                    case 1:
                        Packet SyncGridPacket = new Packet();
                        HexGridManager.Instance._isCastleBuild = true;
                        SyncGridPacket.Write((int)RequestsID.SYNC_GRID);
                        SyncGridPacket.Write(SystemInfo.deviceUniqueIdentifier);
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
                        Packet SyncGridPacket = new Packet();                      
                        SyncGridPacket.Write((int)RequestsID.SYNC_GRID);
                        SyncGridPacket.Write(SystemInfo.deviceUniqueIdentifier);
                        Sender.TCP_Send(SyncGridPacket);

                        Packet SyncPlayerPacket = new Packet();
                        SyncPlayerPacket.Write((int)RequestsID.SYNC);
                        SyncPlayerPacket.Write(SystemInfo.deviceUniqueIdentifier);
                        Sender.TCP_Send(SyncPlayerPacket);
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
                        Packet SyncGridPacket = new Packet();
                        SyncGridPacket.Write((int)RequestsID.SYNC_GRID);
                        SyncGridPacket.Write(SystemInfo.deviceUniqueIdentifier);
                        Sender.TCP_Send(SyncGridPacket);

                        Packet SyncPlayerPacket = new Packet();
                        SyncPlayerPacket.Write((int)RequestsID.SYNC);
                        SyncPlayerPacket.Write(SystemInfo.deviceUniqueIdentifier);
                        Sender.TCP_Send(SyncPlayerPacket);
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
                        Packet SyncGridPacket = new Packet();
                        SyncGridPacket.Write((int)RequestsID.SYNC_GRID);
                        SyncGridPacket.Write(SystemInfo.deviceUniqueIdentifier);
                        Sender.TCP_Send(SyncGridPacket);

                        Packet SyncPlayerPacket = new Packet();
                        SyncPlayerPacket.Write((int)RequestsID.SYNC);
                        SyncPlayerPacket.Write(SystemInfo.deviceUniqueIdentifier);
                        Sender.TCP_Send(SyncPlayerPacket);
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
                        Debug.Log("Am primit raspunul 3 de la BUILD_ARMY_CAMP");
                        Packet SyncGridPacket = new Packet();
                        SyncGridPacket.Write((int)RequestsID.SYNC_GRID);
                        SyncGridPacket.Write(SystemInfo.deviceUniqueIdentifier);
                        Sender.TCP_Send(SyncGridPacket);

                        Packet SyncPlayerPacket = new Packet();
                        SyncPlayerPacket.Write((int)RequestsID.SYNC);
                        SyncPlayerPacket.Write(SystemInfo.deviceUniqueIdentifier);
                        Sender.TCP_Send(SyncPlayerPacket);
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

    }

    
    private void DisconnectedFromServer()
    {
        RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
        //TODO: Connection failed message box with retry button
    }
}

