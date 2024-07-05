using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateDemoMap : MonoBehaviour
{
    public GameObject[] hexPrefabs;
    public int width = 20;
    public int height = 20;
    public float hexWidth = 1.18f;
    public float hexHeight = 1.18f;
    public DemoHexTile[,] hexGrid;
    public DemoPathNode[,] pathGrid;
    public List<DemoHexTile> hexTiles;

    public class DemoHexTile
    {
        public long gameID = 0;
        public long accountID = -1;
        public long attackerAccountID = -1;
        public int hexType = 0;
        public int level = 1;
        public int x;
        public int y;

        public int requiredGold = 0;
        public int requiredStone = 0;
        public int requiredWood = 0;

        public int stonePerSecond = 0;
        public int woodPerSecond = 0;
        public int foodPerSecond = 0;

        public int health = 0;

        public int capacity = 0;

        public int attack = 0;
        public int defense = 0;

        public bool isAttacking = false;
        public bool isDefending = false;
        public bool isUnderAttack = false;        
    }

    public class DemoTile : MonoBehaviour
    {
        public DemoHexTile tile = new DemoHexTile();

        public void Initialize(DemoHexTile tile)
        {
            this.tile = tile;
        }
    }   

    public class DemoHexGrid
    {
        public int rows = 20;
        public int columns = 20;
        public List<DemoHexTile> hexTiles = new List<DemoHexTile>();
    }

    public class DemoPathNode
    {
        public DemoHexTile tile = new DemoHexTile();
        public int gCost; // Cost from start node
        public int hCost; // Heuristic cost to end node
        public int FCost => gCost + hCost; // Total cost
        public DemoPathNode cameFromNode; // To track the path

        public DemoPathNode(DemoHexTile tile)
        {
            this.tile = tile;
        }
        public DemoPathNode()
        {

        }

        public bool IsWalkable()
        {
            switch ((HexType)tile.hexType)
            {
                case HexType.FREE_MOUNTAIN:
                case HexType.FREE_FOREST:
                //case Player.HexType.FREE_CROPS:
                case HexType.PLAYER1_MOUNTAIN:
                case HexType.PLAYER1_FOREST:
                case HexType.PLAYER2_MOUNTAIN:
                case HexType.PLAYER2_FOREST:
                    //case Player.HexType.PLAYER_CROPS:
                    return false;
                default:
                    return true;
            }
        }
    }


    public void Start()
    {
        StartCoroutine(GenerateDemoHexGrid());
    }



    public Vector3 CalculatePosition(int x, int y)
    {
        float horizontalSpacing = hexWidth * Mathf.Sqrt(3) / 2;
        float xPos = x * horizontalSpacing + (y % 2 == 1 ? horizontalSpacing / 2 : 0);
        float yPos = y * hexHeight * 0.75f;
        return new Vector3(xPos, 0, yPos);
    }


    public List<DemoHexTile> Get2RingsOfNeighbours(Tile centerHexTile)
    {
        List<DemoHexTile> neighbors = new List<DemoHexTile>();
        Vector2Int currentPosition;

        Vector2Int[] evenDirections =
        {
            new Vector2Int(0, +1), new Vector2Int(+1, +1), new Vector2Int(+1, 0),
            new Vector2Int(+1, -1), new Vector2Int(0, -1), new Vector2Int(-1, 0),
            new Vector2Int(0, +2), new Vector2Int(+1, +2), new Vector2Int(+2, +1),
            new Vector2Int(+2,0), new Vector2Int(+2, -1), new Vector2Int(+1, -2),
            new Vector2Int(0, -2), new Vector2Int(-1, -2), new Vector2Int(-1, -1),
            new Vector2Int(-2,0), new Vector2Int(-1, +1), new Vector2Int(-1, +2),
        };
        Vector2Int[] oddDirections =
        {
            new Vector2Int(-1, +1), new Vector2Int(0, +1), new Vector2Int(+1, 0),
            new Vector2Int(0,-1), new Vector2Int(-1, -1), new Vector2Int(-1, 0),
            new Vector2Int(0, +2), new Vector2Int(+1, +2), new Vector2Int(+1, +1),
            new Vector2Int(2,0), new Vector2Int(+1, -1), new Vector2Int(+1, -2),
            new Vector2Int(0, -2), new Vector2Int(-1, -2), new Vector2Int(-2, -1),
            new Vector2Int(-2,0), new Vector2Int(-2, +1), new Vector2Int(-1, +2),
        };

        if (centerHexTile.tile.y % 2 != 0)
        {
            foreach (Vector2Int direction in evenDirections)
            {
                //currentPosition = centerHexTile._gridPosition;
                currentPosition = new Vector2Int(centerHexTile.tile.x, centerHexTile.tile.y);
                currentPosition += direction;
                if (currentPosition.x >= 0 && currentPosition.x < width && currentPosition.y >= 0 && currentPosition.y < height)
                {
                    DemoHexTile neighbor = hexGrid[currentPosition.x, currentPosition.y];
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }
        else
        {
            foreach (Vector2Int direction in oddDirections)
            {
                //currentPosition = centerHexTile._gridPosition;
                currentPosition = new Vector2Int(centerHexTile.tile.x, centerHexTile.tile.y);
                currentPosition += direction;
                if (currentPosition.x >= 0 && currentPosition.x < width && currentPosition.y >= 0 && currentPosition.y < height)
                {
                    DemoHexTile neighbor = hexGrid[currentPosition.x, currentPosition.y];
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }

    public List<DemoHexTile> GetNeighbours(Tile centerHexTile)
    {
        List<DemoHexTile> neighbors = new List<DemoHexTile>();
        Vector2Int currentPosition = new Vector2Int();

        Vector2Int[] evenDirections =
        {
            new Vector2Int(0, +1), new Vector2Int(+1, +1), new Vector2Int(+1, 0),
            new Vector2Int(+1, -1), new Vector2Int(0, -1), new Vector2Int(-1, 0)
        };
        Vector2Int[] oddDirections =
        {
            new Vector2Int(-1, +1), new Vector2Int(0, +1), new Vector2Int(+1, 0),
            new Vector2Int(0,-1), new Vector2Int(-1, -1), new Vector2Int(-1, 0),
        };

        if (centerHexTile.tile.y % 2 != 0)
        {
            foreach (Vector2Int direction in evenDirections)
            {
                currentPosition = new Vector2Int(centerHexTile.tile.x, centerHexTile.tile.y);
                currentPosition += direction;
                if (currentPosition.x >= 0 && currentPosition.x < width && currentPosition.y >= 0 && currentPosition.y < height)
                {
                    DemoHexTile neighbor = hexGrid[currentPosition.x, currentPosition.y];
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }
        else
        {
            foreach (Vector2Int direction in oddDirections)
            {
                currentPosition = new Vector2Int(centerHexTile.tile.x, centerHexTile.tile.y);
                currentPosition += direction;
                if (currentPosition.x >= 0 && currentPosition.x < width && currentPosition.y >= 0 && currentPosition.y < height)
                {
                    DemoHexTile neighbor = hexGrid[currentPosition.x, currentPosition.y];
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }

    private static DemoPathNode FindPathOrClosestObstacle(DemoHexGrid hexGrid, int startTile_x, int startTile_y, int targetTile_x, int targetTile_y)
    {

        DemoPathNode[,] pathGrid = new DemoPathNode[hexGrid.rows, hexGrid.columns];

        foreach (DemoHexTile hexTile in hexGrid.hexTiles)
        {
            DemoPathNode pathNode = new DemoPathNode(hexTile);
            pathGrid[hexTile.x, hexTile.y] = pathNode;
        }

        DemoPathNode startNode = pathGrid[startTile_x, startTile_y];
        DemoPathNode targetNode = pathGrid[targetTile_x, targetTile_y];
        DemoPathNode closestObstacle = null;
        int closestDistance = int.MaxValue;

        List<DemoPathNode> openSet = new List<DemoPathNode> { startNode };
        HashSet<DemoPathNode> closedSet = new HashSet<DemoPathNode>();

        while (openSet.Count > 0)
        {
            DemoPathNode currentNode = openSet.OrderBy(node => node.FCost).ThenBy(node => node.hCost).First();
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return targetNode; // Path found
            }

            foreach (DemoPathNode neighbor in GetPathNeighbors(pathGrid, currentNode, hexGrid.rows, hexGrid.columns))
            {
                if (!neighbor.IsWalkable() || closedSet.Contains(neighbor))
                {
                    if (neighbor.tile.hexType == 1 || neighbor.tile.hexType == 2)
                    {
                        int distance = GetDistance(neighbor, targetNode);
                        if (distance < closestDistance)
                        {
                            closestObstacle = neighbor;
                            closestDistance = distance;
                        }
                    }
                    continue;
                }

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.cameFromNode = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return closestObstacle; // No path found, return closest obstacle
    }

    private static List<DemoPathNode> GetPathNeighbors(DemoPathNode[,] pathGrid, DemoPathNode node, int height, int width)
    {
        List<DemoPathNode> neighbors = new List<DemoPathNode>();
        int x = node.tile.x;
        int y = node.tile.y;
        Vector2Int[] directions = y % 2 != 0 ? new Vector2Int[]
        {
            new Vector2Int(0, +1), new Vector2Int(+1, +1), new Vector2Int(+1, 0),
            new Vector2Int(+1, -1), new Vector2Int(0, -1), new Vector2Int(-1, 0)
        } :
        new Vector2Int[]
        {
            new Vector2Int(-1, +1), new Vector2Int(0, +1), new Vector2Int(+1, 0),
            new Vector2Int(0,-1), new Vector2Int(-1, -1), new Vector2Int(-1, 0),
        };

        foreach (var direction in directions)
        {
            int nx = x + direction.x;
            int ny = y + direction.y;
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                neighbors.Add(pathGrid[nx, ny]);
            }
        }
        return neighbors;
    }

    private static int GetDistance(DemoPathNode nodeA, DemoPathNode nodeB)
    {
        int dx = Math.Abs(nodeA.tile.x - nodeB.tile.x);
        int dy = Math.Abs(nodeA.tile.y - nodeB.tile.y);
        return dy + Math.Max(0, (dx - dy) / 2);
    }

    private static List<Data.HexTile> RetracePath(Data.PathNode startNode, Data.PathNode endNode)
    {
        List<Data.HexTile> path = new List<Data.HexTile>();
        Data.PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.tile);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private static int GetRandomHexTile(int[] dynamicWeights)
    {

        int totalWeight = 0;
        foreach (int weight in dynamicWeights)
        {
            totalWeight += weight;
        }

        System.Random random = new System.Random();

        int randomIndex = random.Next(0, totalWeight);
        int sum = 0;

        for (int i = 0; i < dynamicWeights.Length; i++)
        {
            sum += dynamicWeights[i];
            if (randomIndex < sum)
            {
                if (i != 0)
                {
                    int decreaseAmount = 2;
                    int increaseAmount = 1;

                    dynamicWeights[i] = Math.Max(1, dynamicWeights[i] - decreaseAmount);

                    for (int j = 1; j < dynamicWeights.Length; j++)
                    {
                        if (j != i)
                        {
                            dynamicWeights[j] += increaseAmount;
                        }
                    }
                }
                return i;
            }
        }

        return 0; // Should never happen unless weights are misconfigured
    }

    private static void ModifyHexTileType(DemoHexGrid grid, int targetX, int targetY, int newHexType)
    {
        foreach (DemoHexTile tile in grid.hexTiles)
        {
            if (tile.x == targetX && tile.y == targetY)
            {
                tile.hexType = newHexType;
                break;
            }
        }
    }


    private DemoHexGrid GenerateGridAsync()
    {
        
        int[] dynamicWeights = new int[] { 70, 10, 10, 10 };
        DemoHexGrid hexGrid = new DemoHexGrid();

        Data.HexTile startingTile = new Data.HexTile();

        for (int x = 0; x < hexGrid.rows; x++)
        {
            for (int y = 0; y < hexGrid.columns; y++)
            {
                int hexType = GetRandomHexTile(dynamicWeights);
                DemoHexTile tile = new DemoHexTile();
                tile.x = x;
                tile.y = y;
                tile.hexType = hexType;
                hexGrid.hexTiles.Add(tile);

                if (hexType == 0)
                {
                    startingTile.x = x;
                    startingTile.y = y;
                    startingTile.hexType = hexType;

                }
            }
        }

        bool isWalkable = true;
        do
        {
            isWalkable = true;
            foreach (DemoHexTile destinationTile in hexGrid.hexTiles)
            {
                if (destinationTile.hexType == 0 || destinationTile.hexType == 3)
                {
                    DemoPathNode pathNode = FindPathOrClosestObstacle(hexGrid, startingTile.x, startingTile.y, destinationTile.x, destinationTile.y);
                    if (!pathNode.IsWalkable())
                    {
                        isWalkable = false;
                        ModifyHexTileType(hexGrid, pathNode.tile.x, pathNode.tile.y, 0);

                        break;
                    }
                }
            }


        } while (!isWalkable);

        return hexGrid;        
    }

    //public void GenerateDemoHexGrid()
    //{
    //    DemoHexGrid demoGrid = GenerateGridAsync();

    //    DemoTile[,] demoHexGrid = new DemoTile[20, 20];
    //   // DemoPathNode[,] pathGrid = new DemoPathNode[20, 20];

    //    height = 20;
    //    width = 20;

    //    foreach (DemoHexTile tile in demoGrid.hexTiles)
    //    {


    //        GameObject hexPrefab = hexPrefabs[tile.hexType];
    //        int x_pos = tile.x;
    //        int y_pos = tile.y;


    //        GameObject hex = Instantiate(hexPrefab, CalculatePosition(x_pos, y_pos), Quaternion.identity);
    //        hex.transform.SetParent(this.transform);

    //        DemoTile hexTile = hex.AddComponent<DemoTile>();
    //        hexTile.Initialize(tile);

    //        demoHexGrid[x_pos, y_pos] = hexTile;

    //       // pathGrid[tile.x, tile.y] = new DemoPathNode(hexTile);
    //    }       
    //}

    IEnumerator GenerateDemoHexGrid()
    {
        DemoHexGrid hexGrid = GenerateInvalidHexGrid();

        hexTiles = new List<DemoHexTile>();
        int[] dynamicWeights = new int[] { 70, 10, 10, 10 };


        foreach(DemoHexTile tile in hexGrid.hexTiles)
        {
            switch (tile.hexType)
            {
                case 1:
                    dynamicWeights[1] -= 2;
                    dynamicWeights[2] += 1;
                    dynamicWeights[3] += 1;
                    break;
                case 2:
                    dynamicWeights[2] -= 2;
                    dynamicWeights[1] += 1;
                    dynamicWeights[3] += 1;
                    break;
                case 3:
                    dynamicWeights[3] -= 2;
                    dynamicWeights[2] += 1;
                    dynamicWeights[1] += 1;
                    break;

            }

            UI_Demo.instance._mountainText.text = dynamicWeights[1].ToString() + "%";
            UI_Demo.instance._forestText.text = dynamicWeights[2].ToString() + "%";
            UI_Demo.instance._cropsText.text = dynamicWeights[3].ToString() + "%";


            Vector3 position = CalculatePosition(tile.x, tile.y);
            GameObject hexPrefab = hexPrefabs[tile.hexType];
            GameObject hexObject = Instantiate(hexPrefab, position, Quaternion.identity);

            hexObject.transform.SetParent(this.transform);

            DemoHexTile hexTile = new DemoHexTile()
            {
                x = tile.x,
                y = tile.y,
                hexType = tile.hexType
            };
            hexTiles.Add(hexTile);

            
            yield return new WaitForSeconds(0.1f);
        }
                
            
        

        // After all hexes are created, check for accessibility and make adjustments if necessary
        EnsureAccessibility();
    }

    private void EnsureAccessibility()
    {
        // Implement the logic to check for inaccessible areas and adjust the hex types accordingly
        Debug.Log("Checking for inaccessible areas and making adjustments.");
    }


    private DemoHexGrid GenerateInvalidHexGrid()
    {
        DemoHexGrid hexGrid = new DemoHexGrid();
        bool isValid = true;

        while (isValid)
        {
            int[] dynamicWeights = new int[] { 70, 10, 10, 10 };
            hexGrid = new DemoHexGrid();

            DemoHexTile startingTile = new DemoHexTile();

            for (int x = 0; x < hexGrid.rows; x++)
            {
                for (int y = 0; y < hexGrid.columns; y++)
                {
                    int hexType = GetRandomHexTile(dynamicWeights);
                    DemoHexTile tile = new DemoHexTile();
                    tile.x = x;
                    tile.y = y;
                    tile.hexType = hexType;
                    hexGrid.hexTiles.Add(tile);

                    if (hexType == 0)
                    {
                        startingTile.x = x;
                        startingTile.y = y;
                        startingTile.hexType = hexType;

                    }
                }
            }

            isValid = CheckAccessibility(hexGrid, startingTile);
        }

        return hexGrid;
        
    }



    private bool CheckAccessibility(DemoHexGrid hexGrid, DemoHexTile startingTile)
    {
        bool isWalkable = true;

        
        foreach (DemoHexTile destinationTile in hexGrid.hexTiles)
        {
            if (destinationTile.hexType == 0 || destinationTile.hexType == 3)
            {
                DemoPathNode pathNode = FindPathOrClosestObstacle(hexGrid, startingTile.x, startingTile.y, destinationTile.x, destinationTile.y);
                if (!pathNode.IsWalkable())
                {
                    isWalkable = false;
                        
                    break;
                }
            }
        }

        return isWalkable;
    }
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