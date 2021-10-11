using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaperController : MonoBehaviour, MonsterController
{
    public float m_health = 10f;
    private float m_maxSpeed = 2.5f;
    public bool m_isDead = false;

    public Rigidbody2D m_rigidBody;
    public CircleCollider2D m_circleCollider;
    private GameObject m_player;

    public Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");
    }
    
    void FixedUpdate()
    {
        if (m_isDead)
        {
            // Right now destroy rigidbody and collider to stop checks.
            // Later on should destroy self when changing rooms.
            Destroy(m_rigidBody);
            Destroy(m_circleCollider);
            PlayAnimation("Death");
            return;
        }

        Vector3 playerPos = m_player.GetComponent<Collider2D>().bounds.center - transform.position;

        // for debugging
        // Debug.DrawRay(transform.position, toPlayer * 15f, Color.green);

        // 15f length for the ray should be enough for current room size
        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerPos, 15f, LayerMask.GetMask("Player", "Wall"), 0, 0);

        bool canSeePlayer = hit.collider != null && hit.collider.gameObject.tag == "Player";

        if (canSeePlayer)
        {
            MoveTo(playerPos);
        } else {
            // AStarMoveTowardsPlayer();
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
        m_rigidBody.velocity = velocity;
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.DamagePlayer(1);
        }
    }

    public void PlayAnimation(string animation)
    {
        m_animator.Play(animation);
    }

    public bool IsDead()
    {
        return m_isDead;
    }
}
