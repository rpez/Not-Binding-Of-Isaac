using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public float m_enemyActivationTime = 1f;

    public SpawnEntity[] m_spawnGrid;
    public RoomTrigger[] m_doorTriggers = new RoomTrigger[4];

    private bool m_cleared;
    private bool m_active;
    public bool m_isStartingRoom;

    private bool[] m_existingDoors = new bool[4];
    private Door[] m_doors = new Door[4];
    private Collider2D[] m_doorColliders = new Collider2D[4];
    private List<MonsterController> m_enemies = new List<MonsterController>();

    private NavigationGrid m_grid = new NavigationGrid();

    private List<(int, int)> m_obstacleCoords = new List<(int, int)>();
    public List<(int, int)> Obstacles => m_obstacleCoords;

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

        m_cleared = m_isStartingRoom ? true : false;
        m_active = m_isStartingRoom ? true : false;

        // quick, bad fix for starting room
        if (m_isStartingRoom)
        {
            for (int i = 0; i < 4; i++) OpenDoor(i);
        }
    }

    public void OnRoomEnter()
    {
        m_active = true;
        if (!m_cleared)
        {
            foreach (SpawnEntity entity in m_spawnGrid)
            {
                // Room corner plus scaled coordinates (squares are approx. 105x105 pixels)
                Vector3 spawnCoords = m_grid.GridToWorldCoordinates(entity.m_x, entity.m_y, transform.position);
                GameObject spawn = GameObject.Instantiate(entity.m_object, spawnCoords, Quaternion.identity, transform);
                MonsterController monster = spawn.GetComponent<MonsterController>();

                if (monster != null)
                {
                    m_enemies.Add(monster);
                    StartCoroutine(ReleaseEnemy(m_enemyActivationTime + UnityEngine.Random.Range(0.0f, 0.3f), monster));
                }

                if (spawn.tag == "Obstacle")
                {
                    m_obstacleCoords.Add((entity.m_x, entity.m_y));
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

    private IEnumerator ReleaseEnemy(float delay, MonsterController monster)
    {
        yield return new WaitForSeconds(delay);
        monster.Activate();
    }
}

[Serializable]
public class SpawnEntity
{
    public GameObject m_object;
    public int m_x;
    public int m_y;
}