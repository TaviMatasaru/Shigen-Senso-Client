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
        RealtimeNetworking.OnLongReceived += ReceivedLong;
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
            Sender.TCP_Send((int)RequestsID.AUTH, device);
        }
        else
        {
            //TODO: Connection Failed message box with retry button
        }
        RealtimeNetworking.OnConnectingToServerResult -= ConnectionResponse;
    }

    private void ReceivedLong(int id, long value)
    {
        switch (id)
        {
            case 1:
                Debug.Log(value);
                Sender.TCP_Send((int)RequestsID.SYNC, SystemInfo.deviceUniqueIdentifier);
                break;
        }
    }

    private void ReceivedPacket(Packet packet)
    {
        int id = packet.ReadInt();
        switch (id)
        {
            case 2:
                string playerClass = packet.ReadString();
                Data.Player player = Data.Deserialize<Data.Player>(playerClass);
                UI_Main.instance._goldText.text = player.gold.ToString();
                UI_Main.instance._gemsText.text = player.gems.ToString();
                UI_Main.instance._woodText.text = player.wood.ToString();
                UI_Main.instance._stoneText.text = player.stone.ToString();
                UI_Main.instance._foodText.text = player.food.ToString();

                Debug.Log("Am primit datele despre player");

                break;
        }
    }


    private void DisconnectedFromServer()
    {
        RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
        //TODO: Connection failed message box with retry button
    }
}

