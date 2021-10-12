using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomTrigger : MonoBehaviour
{
    private Collider2D m_trigger;

    private Action<int> m_doorCallback;
    private int m_direction;
    private Vector3 m_playerOffset;

    public void Init(Action<int> doorCallback, int direction)
    {
        m_doorCallback = doorCallback;
        m_direction = direction;

        m_trigger = GetComponent<Collider2D>();

        switch (direction)
        {
            case 0:
                m_playerOffset = new Vector3(0f, 2.5f, 0f);
                break;
            case 1:
                m_playerOffset = new Vector3(-5.5f, 0f, 0f);
                break;
            case 2:
                m_playerOffset = new Vector3(0f, -2.5f, 0f);
                break;
            case 3:
                m_playerOffset = new Vector3(5.5f, 0f, 0f);
                break;
            default:
                break;
        }
    }

    public void SetActive(bool active)
    {
        m_trigger.enabled = active;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.position = collision.gameObject.transform.position + m_playerOffset;
            m_doorCallback.Invoke(m_direction);
        }
    }
}
