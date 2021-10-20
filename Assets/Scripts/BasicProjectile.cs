using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{

    private AudioController m_audioController;

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
                monster.DamageMonster(m_baseDamage);

                // Make the monster "jump back" from collision with the projectile
                monster.SetRigidbodyVelocity(m_rigidBody.velocity * m_collisionForce);

                // Play monster animation
                monster.PlayAnimation("TakeDamage");

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
        m_audioController = Camera.main.GetComponent<AudioController>();
        m_audioController.PlayOneShot("IsaacProjectile", playRandom: true);
        Destroy(gameObject, m_lifeTime);
    }

    void OnDestroy()
    {
        m_audioController.PlayOneShot("IsaacProjectileDestroy");
    }
}
