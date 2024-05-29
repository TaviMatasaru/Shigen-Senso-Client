

using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float speed = 3f;
    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    private bool isMooving = false;

    void Update()
    {
        if (isMooving)
        {
            if (pathPoints.Count > 0)
            {
                Vector3 nextPoint = pathPoints.Peek();
                MoveTowards(nextPoint);
                if (Vector3.Distance(transform.position, nextPoint) < 0.1f)
                {
                    pathPoints.Dequeue();
                }
            }
        }     
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 moveDirection = target - transform.position;
        transform.position += moveDirection.normalized * speed * Time.deltaTime;
    }

    public void SetPath(List<Tile> path)
    {
        foreach (Tile tile in path)
        {
            pathPoints.Enqueue(HexGridManager.Instance.CalculatePosition(tile.tile.x, tile.tile.y));
        }
    }



}