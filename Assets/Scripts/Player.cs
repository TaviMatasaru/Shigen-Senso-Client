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
        BUILD = 3
    }

    public void Start()
    {
        RealtimeNetworking.OnPacketReceived += ReceivedPacket;
        ConnectToServer();
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
                break;

            case RequestsID.SYNC:
                string playerData = received_packet.ReadString();
                Data.Player playerSyncData = Data.Deserialize<Data.Player>(playerData);
                SyncPlayerData(playerSyncData);
                break;

            case RequestsID.BUILD:               
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

