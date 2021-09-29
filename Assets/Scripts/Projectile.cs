using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float m_speed = 1f;
    public float m_parentSpeedScale = 1f;
    public float m_parentSpeedThreshold = 2f;
    public float m_lifeTime = 1f;
    public int m_damage = 1;

    private Rigidbody2D m_rigidBody;

    public void Init(Vector2 direction, Vector2 parentVelocity)
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        Vector2 additional = Vector2.zero;
        if (Vector2.Dot(parentVelocity, direction) * parentVelocity.magnitude >= m_parentSpeedThreshold)
        {
            additional = parentVelocity;
            additional = additional.normalized + direction.normalized;
            additional.Normalize();
        }
        
        m_rigidBody.velocity = (direction + additional * m_parentSpeedScale) * m_speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, m_lifeTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
}
