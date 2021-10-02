using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    public bool[] m_existingDoors = new bool[4];
    private GameObject[] m_doors = new GameObject[4];
    private Collider2D[] m_doorColliders = new Collider2D[4];

    public void Init(bool[] existingDoors)
    {
        m_existingDoors = existingDoors;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            m_doors[i] = transform.GetChild(i).gameObject;
            m_doorColliders[i] = m_doors[i].GetComponent<Collider2D>();
        }
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
