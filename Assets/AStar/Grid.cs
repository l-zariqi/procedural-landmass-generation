using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize; // The area in world coordinates that the grid is going to cover
    public float nodeRadius; // How much space each node covers
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius*2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2; // Gets the left edge of the world and the bottom left corner

        for (int x = 0; x < gridSizeX; x++) // Loops through all positions that the nodes will be in to see whether they're walkable or not
        {
            for (int y = 0; y < gridSizeY; y++) 
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius); // As x increases, increment nodeDiameter to find the edge
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius,unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x,y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neightbours = new List<Node>();

        for (int x = -1; x <= -1; x++)
        {
            for (int y = -1; y <= -1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neightbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neightbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x; // Convert world position to percentage for x and y coord
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y; // To see how far along the grid it is
                                                                                    // E.g. if worldPosition on x-axis is 0, it returns a half
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = (int)(Mathf.RoundToInt(gridSizeX - 1) * percentX);
        int y = (int)(Mathf.RoundToInt(gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> path;
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
