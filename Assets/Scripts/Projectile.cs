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

        // Add x and y components of parent object's velocity, scaled by a factor
        // The parent movement component is neglected if it is facing the opposite way of shoot direction
        Vector2 additional = Vector2.zero;
        if (Mathf.Abs(parentVelocity.x) >= m_parentSpeedThreshold
            && (Mathf.Sign(direction.x) == Mathf.Sign(parentVelocity.x)
            || Mathf.Abs(direction.x) < 0.1f))
        {
            additional = new Vector2(Mathf.Sign(parentVelocity.x), additional.y);
        }
        if (Mathf.Abs(parentVelocity.y) >= m_parentSpeedThreshold
            && (Mathf.Sign(direction.y) == Mathf.Sign(parentVelocity.y)
            || Mathf.Abs(direction.y) < 0.1f))
        {
            additional = new Vector2(additional.x, Mathf.Sign(parentVelocity.y));
        }
        additional.Normalize();

        m_rigidBody.velocity = (direction + additional * m_parentSpeedScale) * m_speed;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                // TODO: do damage to enemy
                Destroy(gameObject);
                break;
            case "Wall":
                Destroy(gameObject);
                break;
            case "Obstacle":
                // TODO: do damage to obstacle
                Destroy(gameObject);
                break;

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, m_lifeTime);
    }
}
