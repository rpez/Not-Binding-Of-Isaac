using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private bool[] m_existingDoors = new bool[4];
    private GameObject[] m_doors = new GameObject[4];
    private Collider2D[] m_doorColliders = new Collider2D[4];
    public RoomTrigger[] m_doorTriggers = new RoomTrigger[4];


    public void Init(bool[] existingDoors, Action<int> doorCallback, Action doorLockCallback)
    {
        m_existingDoors = existingDoors;
        for (int i = 0; i < 4; i++)
        {
            m_doors[i] = transform.GetChild(i).gameObject;
            m_doorColliders[i] = m_doors[i].GetComponent<Collider2D>();
        }
        for (int i = 4; i < 8; i++)
        {
            m_doorTriggers[i - 4] = transform.GetChild(i).GetComponent<RoomTrigger>();
            m_doorTriggers[i - 4].Init(doorCallback, doorLockCallback, i - 4);
        }

        CloseAllDoors();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenAllDoors()
    {
        for (int i = 0; i < 4; i++) OpenDoor(i);
    }

    public void OpenDoor(int dir)
    {
        if (m_existingDoors[dir])
        {
            m_doorColliders[dir].enabled = false;
            m_doorTriggers[dir].SetActive(true);
        }
    }

    public void CloseAllDoors()
    {
        for (int i = 0; i < 4; i++) CloseDoor(i);
    }

    public void CloseDoor(int dir)
    {
        m_doorColliders[dir].enabled = true;
    }
}
