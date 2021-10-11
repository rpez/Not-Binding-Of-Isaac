using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public SpawnEntity[] m_spawnGrid;
    public RoomTrigger[] m_doorTriggers = new RoomTrigger[4];

    private bool m_cleared;
    private bool m_active;

    private bool[] m_existingDoors = new bool[4];
    private Door[] m_doors = new Door[4];
    private Collider2D[] m_doorColliders = new Collider2D[4];
    private List<MonsterController> m_enemies = new List<MonsterController>();

    public void Init(bool[] existingDoors, Action<int> doorCallback)
    {
        m_existingDoors = existingDoors;
        for (int i = 0; i < 4; i++)
        {
            m_doors[i] = transform.GetChild(i).GetComponent<Door>();
            m_doorColliders[i] = m_doors[i].GetComponent<Collider2D>();
            if (!m_existingDoors[i]) m_doors[i].gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }
        for (int i = 4; i < 8; i++)
        {
            m_doorTriggers[i - 4] = transform.GetChild(i).GetComponent<RoomTrigger>();
            m_doorTriggers[i - 4].Init(doorCallback, i - 4);
        }

        m_cleared = false;
        m_active = false;
    }

    public void OnRoomEnter()
    {
        m_active = true;
        if (m_cleared) OpenAllDoors();
        else
        {
            foreach (SpawnEntity entity in m_spawnGrid)
            {
                // Room corner plus scaled coordinates (squares are approx. 105x105 pixels)
                Vector3 pos = new Vector3(1.05f * -6f, 1.05f * -3f) + new Vector3(1.05f * entity.m_x, 1.05f * entity.m_y);
                GameObject spawn = GameObject.Instantiate(entity.m_object, transform.position + pos, Quaternion.identity, transform);
                MonsterController monster = spawn.GetComponent<MonsterController>();
                if (monster != null)
                {
                    m_enemies.Add(monster);
                }
            }

            CloseAllDoors();
        }
    }

    public void OnRoomLeave()
    {
        m_active = false;
    }

    public void OpenAllDoors()
    {
        for (int i = 0; i < 4; i++) OpenDoor(i);
    }

    public void OpenDoor(int dir)
    {
        if (m_existingDoors[dir])
        {
            m_doors[dir].gameObject.GetComponent<SpriteRenderer>().sprite = m_doors[dir].m_openSprite;
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
        if (m_existingDoors[dir])
        {
            m_doors[dir].gameObject.GetComponent<SpriteRenderer>().sprite = m_doors[dir].m_closedSprite;
            m_doorColliders[dir].enabled = true;
        }
    }

    private void Update()
    {
        if (!m_active) return;

        // TODO: kinda ugly approach, if enough time, implement with callbacks maybe
        if (!m_cleared && m_enemies.TrueForAll(x => x.IsDead()))
        {
            m_cleared = true;
            OpenAllDoors();
        }
    }
}

[Serializable]
public class SpawnEntity
{
    public GameObject m_object;
    public int m_x;
    public int m_y;
}