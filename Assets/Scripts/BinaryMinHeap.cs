using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public static class ExtensionMethods
{
    public static void Deconstruct<K, V>(this KeyValuePair<K, V> kvp, out K key, out V val)
    {
        key = kvp.Key;
        val = kvp.Value;
    }
}*/

public class BinaryMinHeap
{
    private int m_maxSize;
    private (float, (int, int))[] m_heap;
    private int m_heapSize = 0;

    public int Count => m_heapSize;

    // Binary min-heap with a range of [1, maxSize]
    public BinaryMinHeap(int maxSize)
    {
        m_maxSize = maxSize;
        m_heap = new (float, (int, int))[maxSize];
    }

    private int GetParentIndex(int i)
    {
        return i/2;
    }

    private int GetLeftChildIndex(int i)
    {
        return 2*i;
    }

    private int GetRightChildIndex(int i)
    {
        return 2*i+1;
    }

    private void Swap(int i, int j)
    {
        (float, (int, int)) tmp = m_heap[i-1];
        m_heap[i-1] = m_heap[j-1];
        m_heap[j-1] = tmp;
    }

    private void Heapify(int i)
    {
        int leftChild = GetLeftChildIndex(i);
        int rightChild = GetRightChildIndex(i);

        if (rightChild <= m_heapSize)
        {
            int smallestChild = m_heap[leftChild-1].Item1 < m_heap[rightChild-1].Item1 ? leftChild : rightChild;

            if (m_heap[i-1].Item1 > m_heap[smallestChild-1].Item1)
            {
                Swap(i, smallestChild);
            }
        }
        else if (leftChild == m_heapSize && m_heap[i-1].Item1 > m_heap[leftChild-1].Item1)
        {
            Swap(i, leftChild);
        }
    }

    public void Push(float key, (int, int) val)
    {
        if (m_heapSize >= m_maxSize)
        {
            Debug.LogError("Could not push to heap! The heap is full.");
            return;
        }

        m_heapSize++;
        int i = m_heapSize;

        while (i > 1 && m_heap[GetParentIndex(i)-1].Item1 > key)
        {
            m_heap[i-1] = m_heap[GetParentIndex(i)-1];
            i = GetParentIndex(i);
        }

        m_heap[i-1] = (key, val);
    }

    public (int, int) Pop()
    {
        if (m_heapSize <= 0)
        {
            Debug.LogError("Could not pop from heap! The heap is empty.");
            return (-1, -1);
        }

        (int, int) smallest = m_heap[0].Item2;
        m_heap[0] = m_heap[m_heapSize-1];
        m_heapSize--;
        Heapify(1);

        return smallest;
    }

    public bool ContainsValue((int, int) val)
    {
        for (int i = 0; i < m_heapSize; i++)
        {
            if (m_heap[i].Item2 == val) return true;
        }

        return false;
    }
}
