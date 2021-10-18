using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBossProjectile : Projectile
{
    public float m_randomSpeedMultiplier = 0.2f;

    public override void Init(Vector2 direction, Vector2 parentVelocity)
    {
        base.Init(direction, parentVelocity);

        float rand = 0.5f + Random.Range(-m_randomSpeedMultiplier, m_randomSpeedMultiplier);
        m_rigidBody.velocity *= rand;

        transform.GetChild(0).right = -direction;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                if (collision.isTrigger) {
                    return;
                }

                PlayerController player = collision.gameObject.GetComponent<PlayerController>();

                // Damage the monster
                player.DamagePlayer((int)m_baseDamage);

                // Play monster animation
                player.PlayAnimation("TakeDamage");

                // Destroy projectile
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
