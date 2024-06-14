

using System.Collections.Generic;
using DevelopersHub.RealtimeNetworking.Client;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Data.Unit unitData;
    public float speed = 1f;

    private string oldPath = "";

    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    private Queue<Vector2Int> gridPathPoints = new Queue<Vector2Int>();

    public int grid_x;
    public int grid_y;

    public void Initialize(Data.Unit unit)
    {
        this.unitData = unit;
        grid_x = unit.current_x;
        grid_y = unit.current_y;
    }

    public void Update()
    {      
        MovementUpdate();
        UpdateUnitPath();       
    }
    

    private void UpdateUnitPath()
    {
        if (oldPath != unitData.serializedPath)
        {
            oldPath = unitData.serializedPath;           

            List<Data.HexTile> path = Data.Deserialize<List<Data.HexTile>>(unitData.serializedPath);
            SetPath(path);
        }
    }
   
    private void MovementUpdate()
    {
        if (pathPoints.Count > 0)
        {
            Vector3 nextPoint = pathPoints.Peek();
            Vector2Int nextGridPoint = gridPathPoints.Peek();
            MoveTowards(nextPoint);
            if (Vector3.Distance(transform.position, nextPoint) < 0.1f)
            {                               
                grid_x = nextGridPoint.x;
                grid_y = nextGridPoint.y;


                if (grid_x == unitData.target_x && grid_y == unitData.target_y)
                {
                    if (Player.instance.data.isPlayer1 == 1 && unitData.ready_player1 == false)
                    {
                        Packet packet = new Packet();
                        packet.Write((int)Player.RequestsID.UNIT_READY);
                        packet.Write(unitData.databaseID);
                        packet.Write(grid_x);
                        packet.Write(grid_y);
                        packet.Write(Player.instance.data.isPlayer1);
                        Sender.TCP_Send(packet);
                    }
                    if (Player.instance.data.isPlayer1 == 0 && unitData.ready_player2 == false)
                    {
                        Packet packet = new Packet();
                        packet.Write((int)Player.RequestsID.UNIT_READY);
                        packet.Write(unitData.databaseID);
                        packet.Write(grid_x);
                        packet.Write(grid_y);
                        packet.Write(Player.instance.data.isPlayer1);
                        Sender.TCP_Send(packet);
                    }
                }


                pathPoints.Dequeue();
                gridPathPoints.Dequeue();
            }
        }
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 moveDirection = target - transform.position;
        transform.position += moveDirection.normalized * speed * Time.deltaTime;
    }


    public void SetPath(List<Data.HexTile> path)
    {
        foreach (Data.HexTile tile in path)
        {           
            pathPoints.Enqueue(HexGridManager.Instance.CalculateUnitPosition(tile.x, tile.y));
            gridPathPoints.Enqueue(new Vector2Int(tile.x, tile.y));
        }
    }
    

}