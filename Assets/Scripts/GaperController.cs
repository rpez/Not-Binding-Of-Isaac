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

    // Update is called once per frame
    void Update()
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

        Vector3 m_playerPos = m_player.transform.position;
        // replace later with raycasting to check if player is seen
        bool canSeePlayer = true;

        if (canSeePlayer)
        {
            MoveTowardsPlayer(m_playerPos);
        } else {
            // AStarMoveTowardsPlayer();
        }
    }

    public void MoveTowardsPlayer(Vector3 playerPos)
    {
        Vector2 direction = new Vector2(playerPos.x - gameObject.transform.position.x, playerPos.y - gameObject.transform.position.y);
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

    public void OnTriggerStay2D(Collider2D collision)
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
}
