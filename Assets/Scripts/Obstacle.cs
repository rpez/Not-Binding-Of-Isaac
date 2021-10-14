using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public int m_health = 3;  // 3 hits to destroy by default

    public void DamageObstacle()
    {
        // -1 is unbreakable
        if (m_health == -1)
        {
            return;
        }

        m_health--;

        if (m_health <= 0)
        {
            Destroy(gameObject);
            // Play animation?
        }
    }
}
