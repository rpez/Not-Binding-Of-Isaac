using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    private SoundController m_audioController;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger && collision.gameObject.tag != "RoomTrigger") return;
        
        switch (collision.gameObject.tag)
        {
            case "Enemy":
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
            case "RoomTrigger":
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
        m_audioController = Camera.main.GetComponent<SoundController>();
        m_audioController.PlayOneShot("IsaacProjectile", playRandom: true);
        Destroy(gameObject, m_lifeTime);
    }

    void OnDestroy()
    {
        m_audioController.PlayOneShot("IsaacProjectileDestroy");
    }
}
