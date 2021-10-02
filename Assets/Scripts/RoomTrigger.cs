using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomTrigger : MonoBehaviour
{
    private Collider2D m_trigger;

    private Action<int> m_doorCallback;
    private Action doorLockCallback;
    private int m_direction;

    public void Init(Action<int> doorCallback, Action doorLockCallback, int direction)
    {
        m_doorCallback = doorCallback;
        m_direction = direction;

        m_trigger = GetComponent<Collider2D>();
    }

    public void SetActive(bool active)
    {
        m_trigger.enabled = active;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_doorCallback.Invoke(m_direction);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            doorLockCallback.Invoke();
        }
    }
}
