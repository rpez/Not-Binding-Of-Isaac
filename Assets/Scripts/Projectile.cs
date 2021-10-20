using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject m_impactEffect;

    private float m_speed = 8f;
    private float m_parentSpeedScale = 0.5f;
    private float m_parentSpeedThreshold = 2f;
    public float m_lifeTime = 1f;
    public float m_collisionForce = 5f;
    public float m_baseDamage = 3.5f;
    protected float m_damageModifier;

    protected Rigidbody2D m_rigidBody;

    public virtual void Init(Vector2 direction, Vector2 parentVelocity, float damageModifier)
    {
        m_damageModifier = damageModifier;
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

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, m_lifeTime);
    }
}
