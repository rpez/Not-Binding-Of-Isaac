using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public (int, int) m_coordinates;

    public Room[] m_neigbours = new Room[4];
    public int m_neighbourAmount = 0;



    public Room((int, int) coordinates)
    {
        m_coordinates = coordinates;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttachRoom(int direction, Room room)
    {
        m_neigbours[direction] = room;
        m_neighbourAmount++;
    }
}
