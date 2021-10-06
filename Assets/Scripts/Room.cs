using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public SpawnEntity[] m_spawnGrid;
    public RoomTrigger[] m_doorTriggers = new RoomTrigger[4];

    private bool m_cleared;

    private bool[] m_existingDoors = new bool[4];
    private GameObject[] m_doors = new GameObject[4];
    private Collider2D[] m_doorColliders = new Collider2D[4];
    private List<MonsterController> m_enemies = new List<MonsterController>();

    public void Init(bool[] existingDoors, Action<int> doorCallback)
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
            m_doorTriggers[i - 4].Init(doorCallback, i - 4);
        }

        m_cleared = false;
    }

    public void OnRoomEnter()
    {
        if (m_cleared) OpenAllDoors();
        else
        {
            foreach (SpawnEntity entity in m_spawnGrid)
            {
                GameObject spawn = GameObject.Instantiate(entity.m_object, transform.position, Quaternion.identity, transform);
                MonsterController monster = spawn.GetComponent<MonsterController>();
                if (monster != null)
                {
                    m_enemies.Add(monster);
                }
            }

            CloseAllDoors();
        }
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

    private void Update()
    {
        if 
    }
}

[Serializable]
public class SpawnEntity
{
    public GameObject m_object;
    public int m_x;
    public int m_y;
}