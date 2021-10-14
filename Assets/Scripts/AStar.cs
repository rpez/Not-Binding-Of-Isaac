using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Operate on a grid with a range of ([0..xSize-1], [0..ySize-1])
    public AStar(int xSize, int ySize)
    {
        m_xMax = xSize;
        m_yMax = ySize;
    }

    private void Initialize((int, int) startNode, (int, int) goalNode)
    {
        m_startNode = startNode;
        m_goalNode = goalNode;

        m_gScores = new Dictionary<(int, int), float>();

        for (int x = 0; x < m_xMax; x++)
        {
            for (int y = 0; y < m_yMax; y++)
            {
                m_gScores.Add((x, y), Mathf.Infinity);  // initialize all nodes with Inf
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

    private float CalculateFScore((int, int) node)
    {
        // Use Chebyshev distance from goal node as admissible heuristic to guide A*
        // Euclidean distance is not admissible as it might over-estimate the cost
        float hScore = Mathf.Max(Mathf.Abs(node.Item1 - m_goalNode.Item1), Mathf.Abs(node.Item2 - m_goalNode.Item2));
        return m_gScores[node] + hScore;
    }

    private List<(int, int)> GetNeighbours((int, int) node)
    {
        List<(int, int)> neighbours = new List<(int, int)>();

        if (node.Item1 != 0)
        {
            neighbours.Add(TupleAdd(node, (-1, 0)));                                // center left

            if (node.Item2 != 0) neighbours.Add(TupleAdd(node, (-1, -1)));          // upper left

            if (node.Item2 != m_yMax - 1) neighbours.Add(TupleAdd(node, (-1, 1)));  // lower left
        }

        if (node.Item1 != m_xMax - 1)
        {
            neighbours.Add(TupleAdd(node, (1, 0)));                                 // center right

            if (node.Item2 != 0) neighbours.Add(TupleAdd(node, (1, -1)));           // upper right

            if (node.Item2 != m_yMax - 1) neighbours.Add(TupleAdd(node, (1, 1)));   // lower right
        }

        if (node.Item2 != 0) neighbours.Add(TupleAdd(node, (0, -1)));               // upper center
        if (node.Item2 != m_yMax - 1) neighbours.Add(TupleAdd(node, (0, 1)));       // lower center

        return neighbours;
    }

    private List<(int, int)> ReconstructRoute()
    {
        List<(int, int)> route = new List<(int, int)>();
        (int, int) currentNode = m_goalNode;

        while (currentNode != m_startNode) {
            route.Add(currentNode);
            currentNode = m_path[currentNode];
        }

        route.Reverse();

        return route;
    }

    public List<(int, int)> Solve((int, int) startNode, (int, int) goalNode)
    {
        if (startNode.Item1 < 0 || startNode.Item1 >= m_xMax || startNode.Item2 < 0 || startNode.Item2 >= m_yMax)
        {
            Debug.LogError("Cannot solve A* path. The start node exceeds the bounds of the grid.");
            return null;
        }
        else if (goalNode.Item1 < 0 || goalNode.Item1 >= m_xMax || goalNode.Item2 < 0 || goalNode.Item2 >= m_yMax)
        {
            Debug.LogError("Cannot solve A* path. The goal node exceeds the bounds of the grid.");
            return null;
        }

        Initialize(startNode, goalNode);

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
        return null;  // visited all nodes but failed to find a path
    }
}
