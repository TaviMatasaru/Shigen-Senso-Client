using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public Tile tile;
    public int gCost; // Cost from start node
    public int hCost; // Heuristic cost to end node
    public int FCost => gCost + hCost; // Total cost
    public PathNode cameFromNode; // To track the path

    public PathNode(Tile tile)
    {
        this.tile = tile;
    }

    public bool IsWalkable()
    {
        switch ((Player.HexType)tile.tile.hexType)
        {
            case Player.HexType.FREE_MOUNTAIN:
            case Player.HexType.FREE_FOREST:
            //case Player.HexType.FREE_CROPS:
            case Player.HexType.PLAYER1_MOUNTAIN:
            case Player.HexType.PLAYER1_FOREST:
            case Player.HexType.PLAYER2_MOUNTAIN:
            case Player.HexType.PLAYER2_FOREST:
                //case Player.HexType.PLAYER_CROPS:
                return false;
            default:
                return true;
        }
    }
}
