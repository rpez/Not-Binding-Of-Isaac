using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBossController : MonoBehaviour, MonsterController
{
    public float m_health = 10f;
    private float m_maxSpeed = 4.2f; // 42 !!!
    public bool m_isDead = false;

    public Rigidbody2D m_rigidBody;
    public CircleCollider2D m_circleCollider;
    public BoxCollider2D m_boxCollider;
    public AnimationHandler m_animationHandler;
    public GameObject m_boss;
    public GameObject m_pool;
    private GameObject m_player;

    public Animator m_animator;
    private bool m_active;

    private float m_currentTime;
    private float m_waitTime;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");
        Physics2D.queriesStartInColliders = false;
        m_active = false;
        m_waitTime = 5f;
        m_pool.GetComponent<SpriteRenderer>().enabled = false;
    }
    
    void FixedUpdate()
    {
        if (m_isDead)
        {
            // Right now destroy rigidbody and collider to stop checks.
            // Later on should destroy self when changing rooms.
            Destroy(m_rigidBody);
            Destroy(m_circleCollider);
            Destroy(m_boxCollider);
            PlayAnimation("Death");
            return;
        }

        //if (!m_active) return;

        Vector3 playerPos = m_player.GetComponent<Collider2D>().bounds.center - transform.position;

        m_currentTime += Time.deltaTime;

        if (m_currentTime >= m_waitTime)
        {
            m_pool.transform.position = transform.position;
            m_pool.GetComponent<SpriteRenderer>().enabled = true;
            m_pool.GetComponent<Animator>().SetBool("Despawn", false);
            m_boss.GetComponent<Animator>().SetInt("Pool", 0);
        }
    }

    public void MoveTo(Vector3 position)
    {
        Vector2 direction = new Vector2(position.x, position.y);
        direction.Normalize();

        // Manually lerp from current speed to max speed
        SetRigidbodyVelocity(m_rigidBody.velocity * 0.8f + direction * m_maxSpeed * 0.2f);
    }

    public void DamageMonster(float amount)
    {
        m_health -= amount;
        if (m_health <= 0f)
        {
            m_health = 0f;
            m_isDead = true;
        }
    }

    public void SetRigidbodyVelocity(Vector2 velocity)
    {
        //m_rigidBody.velocity = velocity;
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        //if (collision.gameObject.tag == "Player")
        //{
        //    PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        //    player.DamagePlayer(1);
        //}
    }

    public void PlayAnimation(string animation)
    {
        //m_animator.Play(animation);
    }

    public bool IsDead()
    {
        return m_isDead;
    }

    public void Activate()
    {
        m_active = true;
    }
}
