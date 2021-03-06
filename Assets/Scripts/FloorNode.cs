using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorNode
{
    public (int, int) m_coordinates;

    public FloorNode[] m_neigbours = new FloorNode[4];
    public int m_neighbourAmount = 0;

    public Room m_room;

    public FloorNode((int, int) coordinates)
    {
        m_coordinates = coordinates;
    }

    public void AttachRoom(int direction, FloorNode room)
    {
        m_neigbours[direction] = room;
        m_neighbourAmount++;
    }
}
