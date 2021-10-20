using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                if (collision.isTrigger) {
                    return;
                }

                MonsterController monster = collision.gameObject.GetComponent<MonsterController>();

                if (monster == null) monster = collision.gameObject.transform.parent.GetComponent<MonsterController>();

                // Damage the monster
                monster.DamageMonster(m_baseDamage * m_damageModifier);

                // Make the monster "jump back" from collision with the projectile
                monster.SetRigidbodyVelocity(m_rigidBody.velocity * m_collisionForce);

                // Play monster animation
                monster.PlayAnimation("TakeDamage");

                SpawnEffect();
                // Destroy projectile
                Destroy(gameObject);
                break;
            case "Wall":
                SpawnEffect();
                Destroy(gameObject);
                break;
            case "Obstacle":
                // TODO: do damage to obstacle
                SpawnEffect();
                Destroy(gameObject);
                break;

        }
    }

    private void SpawnEffect()
    {
        GameObject impact = GameObject.Instantiate(m_impactEffect, transform.position, Quaternion.identity);
        Destroy(impact, 2f);
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, m_lifeTime);
    }
}
