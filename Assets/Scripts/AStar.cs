using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class AStar
{
    private (int, int) m_startNode;
    private (int, int) m_goalNode;
    private IDictionary<(int, int), float> m_gScores;
    private IDictionary<(int, int), float> m_fScores;
    private IDictionary<(int, int), (int, int)> m_path;
    private BinaryMinHeap m_openNodes;
    private int m_xMax;
    private int m_yMax;

    // Operate on a grid with a range of [0..GRID_MAX_X-1], [0..GRID_MAX_Y-1]
    public AStar()
    {
        m_xMax = GRID_MAX_X;
        m_yMax = GRID_MAX_Y;
    }

    private void Initialize((int, int) startNode, (int, int) goalNode, List<(int, int)> obstacles)
    {
        m_startNode = startNode;
        m_goalNode = goalNode;

        m_gScores = new Dictionary<(int, int), float>();

        for (int x = 0; x < m_xMax; x++)
        {
            for (int y = 0; y < m_yMax; y++)
            {
                if (!obstacles.Contains((x, y)))
                {
                    m_gScores.Add((x, y), Mathf.Infinity);  // initialize all nodes with Inf
                }
            }
        }

        m_gScores[m_startNode] = 0;  // initialize starting node with 0

        m_fScores = new Dictionary<(int, int), float>();
        m_fScores.Add(m_startNode, CalculateFScore(m_startNode));

        m_openNodes = new BinaryMinHeap(m_xMax * m_yMax);
        m_openNodes.Push(m_fScores[m_startNode], m_startNode);

        m_path = new Dictionary<(int, int), (int, int)>();
    }

    private (int, int) TupleAdd((int, int) node1, (int, int) node2)
    {
        return (node1.Item1 + node2.Item1, node1.Item2 + node2.Item2);
    }

    private (int, int) TupleSubtract((int, int) node1, (int, int) node2)
    {
        return (node1.Item1 - node2.Item1, node1.Item2 - node2.Item2);
    }

    private float CalculateFScore((int, int) node)
    {
        // Use Euclidean distance squared from goal node as admissible heuristic to guide A*
        // We're quite greedy and leave out Mathf.Sqrt() since our obstacles are quite easy
        // http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html#euclidean-distance-squared

        float hScore = (node.Item1 - m_goalNode.Item1) * (node.Item1 - m_goalNode.Item1) +
            (node.Item2 - m_goalNode.Item2) * (node.Item2 - m_goalNode.Item2);
        return m_gScores[node] + hScore;
    }

    private List<(int, int)> GetNeighbours((int, int) node)
    {
        List<(int, int)> neighbourValues = new List<(int, int)>()
        {
            (-1, 0),   // center left
            (-1, -1),  // upper left
            (-1, 1),   // lower left
            (1, 0),    // center right
            (1, -1),   // upper right
            (1, 1),    // lower right
            (0, -1),   // upper center
            (0, 1)     // lower center
        };

        List<(int, int)> neighbours = new List<(int, int)>();

        foreach ((int, int) neighbourVal in neighbourValues)
        {
            bool diagonalNeighbour = Mathf.Abs(neighbourVal.Item1) + Mathf.Abs(neighbourVal.Item2) == 2;
            (int, int) neighbour = TupleAdd(node, neighbourVal);

            if (m_gScores.ContainsKey(neighbour))
            {
                // make sure we are actually able to move diagonally
                if (!diagonalNeighbour || m_gScores.ContainsKey((neighbour.Item1, node.Item2)) || m_gScores.ContainsKey((node.Item1, neighbour.Item2)))
                {
                    neighbours.Add(neighbour);
                }
            }
        }

        return neighbours;
    }

    // Reconstruct route. Returns properly scaled Vector3Ints from startNode to goalNode.
    private List<Vector3Int> ReconstructRoute()
    {
        List<Vector3Int> route = new List<Vector3Int>();
        (int, int) currentNode = m_goalNode;

        (int, int) prevNode = (-1, -1);
        (int, int) prevDirection = (0, 0);

        // Build the route piece by piece from goalNode to startNode.
        while (true) {
            if (prevNode != (-1, -1))
            {
                (int, int) direction = TupleSubtract(prevNode, currentNode);

                // If we have a different direction, create a new Vector3Int from here.
                if (direction != prevDirection)
                {
                    route.Add(new Vector3Int(direction.Item1, direction.Item2, 0));
                }
                // If we have the same direction, add to the previous Vector3Int in order to scale it properly.
                else
                {
                    route[route.Count - 1] = route[route.Count - 1] + new Vector3Int(direction.Item1, direction.Item2, 0);
                }

                prevDirection = direction;
            }

            // Break here in order to add startNode to the last Vector3Int.
            if (currentNode == m_startNode) break;
            
            prevNode = currentNode;
            currentNode = m_path[currentNode];
        }

        // Reverse the route so we have a path from startNode to goalNode.
        route.Reverse();

        return route;
    }

    /*
    Solve for the shortest route using A*.
    The third method obstacles is optional and defaults to null, so you can either use it with no obstacles:

    AStar.Solve((startNodeX, startNodeY), (goalNodeX, goalNodeY))

    or for example with a single obstacle:

    AStar.Solve((startNodeX, startNodeY), (goalNodeX, goalNodeY), new List<(int, int)>(){ (obstacleX, obstacleY) }).

    Returns a list of Vector3Ints to travel or an empty list if not successful.
    */
    public List<Vector3Int> Solve((int, int) startNode, (int, int) goalNode, List<(int, int)> obstacles = null)
    {
        if (startNode.Item1 < 0 || startNode.Item1 >= m_xMax || startNode.Item2 < 0 || startNode.Item2 >= m_yMax)
        {
            Debug.LogError("Cannot solve A* path. The start node exceeds the bounds of the grid.");
            return new List<Vector3Int>();
        }
        else if (goalNode.Item1 < 0 || goalNode.Item1 >= m_xMax || goalNode.Item2 < 0 || goalNode.Item2 >= m_yMax)
        {
            Debug.LogError("Cannot solve A* path. The goal node exceeds the bounds of the grid.");
            return new List<Vector3Int>();
        }

        obstacles ??= new List<(int, int)>();

        Initialize(startNode, goalNode, obstacles);

        while (m_openNodes.Count > 0)
        {
            (int, int) currentNode = m_openNodes.Pop();

            if (currentNode == m_goalNode)
            {
                return ReconstructRoute();
            }

            foreach ((int, int) neighbour in GetNeighbours(currentNode))
            {
                bool diagonalNeighbour = Mathf.Abs(neighbour.Item1) + Mathf.Abs(neighbour.Item2) == 2;

                float distanceToNeighbour = 1;
                if (diagonalNeighbour) distanceToNeighbour = Mathf.Sqrt(2);

                float potential = m_gScores[currentNode] + distanceToNeighbour;

                if (potential < m_gScores[neighbour])
                {
                    m_path[neighbour] = currentNode;
                    m_gScores[neighbour] = potential;
                    m_fScores[neighbour] = CalculateFScore(neighbour);
                    if (!m_openNodes.ContainsValue(neighbour))
                    {
                        m_openNodes.Push(m_fScores[neighbour], neighbour);
                    }
                }
            }
        }
        return new List<Vector3Int>();  // visited all nodes but failed to find a path
    }
}
