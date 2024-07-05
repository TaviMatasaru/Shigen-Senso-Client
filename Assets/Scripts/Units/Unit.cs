

using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        if (grid_x == unitData.target_x && grid_y == unitData.target_y && !pathPoints.Any() && unitData.ready_player1 && unitData.ready_player2)
        {
            RandomWalkUpdate();
        }
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

    //private void MovementUpdate()
    //{
    //    if (pathPoints.Count > 0)
    //    {
    //        Vector3 nextPoint = pathPoints.Peek();
    //        Vector2Int nextGridPoint = gridPathPoints.Peek();
    //        MoveTowards(nextPoint);
    //        if (Vector3.Distance(transform.position, nextPoint) < 0.1f)
    //        {                               
    //            grid_x = nextGridPoint.x;
    //            grid_y = nextGridPoint.y;


    //            if (grid_x == unitData.target_x && grid_y == unitData.target_y)
    //            {
    //                if (Player.instance.data.isPlayer1 == 1 && unitData.ready_player1 == false)
    //                {
    //                    Packet packet = new Packet();
    //                    packet.Write((int)Player.RequestsID.UNIT_READY);
    //                    packet.Write(unitData.databaseID);
    //                    packet.Write(grid_x);
    //                    packet.Write(grid_y);
    //                    packet.Write(Player.instance.data.isPlayer1);
    //                    Sender.TCP_Send(packet);
    //                }
    //                if (Player.instance.data.isPlayer1 == 0 && unitData.ready_player2 == false)
    //                {
    //                    Packet packet = new Packet();
    //                    packet.Write((int)Player.RequestsID.UNIT_READY);
    //                    packet.Write(unitData.databaseID);
    //                    packet.Write(grid_x);
    //                    packet.Write(grid_y);
    //                    packet.Write(Player.instance.data.isPlayer1);
    //                    Sender.TCP_Send(packet);
    //                }
    //            }


    //            pathPoints.Dequeue();
    //            gridPathPoints.Dequeue();
    //        }
    //    }
    //}



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

                pathPoints.Dequeue();
                gridPathPoints.Dequeue();

                if (grid_x == unitData.target_x && grid_y == unitData.target_y)
                {
                    if (unitData.ready_player1 == false || unitData.ready_player2 == false)
                    {
                        NotifyUnitReady();
                    }

                    
                    RandomWalkInsideHex();
                }
            }
        }
    }


    private void RandomWalkUpdate()
    {
        // Planifică RandomWalkInsideHex dacă nu este deja planificat
        if (!IsInvoking("RandomWalkInsideHex"))
        {
            Invoke("RandomWalkInsideHex", Random.Range(2f, 4f));
        }
    }


    private void NotifyUnitReady()
    {
        Packet packet = new Packet();
        packet.Write((int)Player.RequestsID.UNIT_READY);
        packet.Write(unitData.databaseID);
        packet.Write(grid_x);
        packet.Write(grid_y);
        packet.Write(Player.instance.data.isPlayer1);
        Sender.TCP_Send(packet);
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

    private void RandomWalkInsideHex()
    {
        // Asigură-te că nu planifici mișcări noi dacă una este deja planificată
        if (unitData.ready_player1 && unitData.ready_player2 && !IsInvoking("RandomWalkInsideHex"))
        {
            float radius = 0.4f; // Radius în care unitatea poate să se deplaseze aleatoriu.
            Vector3 hexCenter = HexGridManager.Instance.CalculateUnitPosition(unitData.target_x, unitData.target_y);
            Vector3 randomDirection = Random.insideUnitCircle * radius;
            Vector3 randomDestination = hexCenter + new Vector3(randomDirection.x, 0.2f, randomDirection.y);

            // Începe mișcarea spre destinația aleatorie
            StartCoroutine(MoveToPosition(randomDestination, 2));
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        // Așteaptă între 2 și 4 secunde înainte de următoarea mișcare
        float waitTime = Random.Range(2.0f, 4.0f);
        yield return new WaitForSeconds(waitTime);

        // Reprogramarea următoarei mișcări aleatorii
        if (unitData.ready_player1 && unitData.ready_player2)
        {
            Invoke("RandomWalkInsideHex", waitTime);
        }
    }



}