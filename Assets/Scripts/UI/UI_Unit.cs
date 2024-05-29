using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DevelopersHub.RealtimeNetworking.Client;

public class UI_Unit : MonoBehaviour
{
    [SerializeField] private Data.UnitID _id = Data.UnitID.barbarian; public Data.UnitID id { get { return _id; } }
    [SerializeField] private Button _button = null;
    [SerializeField] private TextMeshProUGUI unitsCount;
    [SerializeField] private TextMeshProUGUI unitCostText = null;

    private int count = 0; public int haveCount { get { return count; } set { count = value; unitsCount.text = count.ToString(); } }

    private void Start()
    {
        _button.onClick.AddListener(Clicked);
    }


    public void Initialize(Data.ServerUnit unit)
    {
        if(unit.requiredFood > 0)
        {
            unitCostText.text = "Food: " + unit.requiredFood.ToString();
        }
    }


    private void Clicked()
    {
        Tile armyCampTile = HexGridManager.Instance.GetCurrentlySelectedTile();

        Packet packet = new Packet();
        packet.Write((int)Player.RequestsID.TRAIN);
        packet.Write(_id.ToString());
        packet.Write(armyCampTile.tile.x);
        packet.Write(armyCampTile.tile.y);
        Sender.TCP_Send(packet);
    }

    public void Sync(int armyCamp_x, int armyCamp_y)
    {
        count = 0;
        for (int i = 0; i < Player.instance.initializationData.serverUnits.Count; i++)
        {
            if(Player.instance.data.units[i].id == _id && Player.instance.data.units[i].ready)
            {
                count++;

                //if(Player.instance.data.units[i].armyCamp_x == armyCamp_x && Player.instance.data.units[i].armyCamp_y == armyCamp_y)
                //{
                //    count++;
                //}                
            }       
        }
        haveCount = count;
    }

}
