using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaperController : MonoBehaviour, MonsterController
{
    public float m_health = 10f;
    private float m_maxSpeed = 3.5f;
    public bool m_isDead = false;

    public Rigidbody2D m_rigidBody;
    public CircleCollider2D m_circleCollider;
    public BoxCollider2D m_boxCollider;
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
            Destroy(m_boxCollider);
            PlayAnimation("Death");
            return;
        }

        Vector3 playerPos = m_player.GetComponent<Collider2D>().bounds.center - transform.position;

        // 15f length for the ray should be enough for current room size
        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerPos, 15f);

        bool debugRays = true;

        if (hit.collider.gameObject.tag == "Wall")
        {
            // AStarMoveTowardsPlayer();
        }
        else if (hit.collider.gameObject.tag == "Player")
        {
            if (debugRays)
            {
                Vector3 drawRay1 = playerPos;
                drawRay1.Normalize();
                Debug.DrawRay(transform.position, drawRay1, Color.green);
            }
            
            MoveTo(playerPos);
        }
        else if (hit.collider.gameObject.tag == "Enemy")
        {
            // Try to dodge your fellow enemy. This way the enemies can try to surround the player.

            // Get the bounding box of the blocking enemy as 4x Vector3
            Bounds b = hit.collider.bounds;
            Vector3[] bounds = new Vector3[]
            {
                new Vector3(b.min.x, b.min.y),
                new Vector3(b.min.x, b.max.y),
                new Vector3(b.max.x, b.min.y),
                new Vector3(b.max.x, b.max.y)
            };

            // Find the second closest bounding box corner to me
            Vector3 closest = Vector3.positiveInfinity;
            Vector3 secondClosest = Vector3.positiveInfinity;

            foreach (Vector3 v in bounds)
            {
                if (Vector3.Distance(v, transform.position) < closest.magnitude)
                {
                    secondClosest = closest;
                    closest = v;
                }
                else if (Vector3.Distance(v, transform.position) < secondClosest.magnitude)
                {
                    secondClosest = v;
                }
            }

            // Reflect the vector secondClosest across the line playerPos, where playerPos is a straight line towards the player.
            // This makes the enemies try to scatter a bit in order to not block each other.
            // See: https://en.wikipedia.org/wiki/Reflection_(mathematics)#Reflection_across_a_line_in_the_plane
            Vector3 enemyPos = secondClosest - transform.position;
            Vector3 reflection = 2 * Vector3.Dot(enemyPos, playerPos) / Vector3.Dot(playerPos, playerPos) * playerPos - enemyPos;

            MoveTo(reflection);

            if (debugRays)
            {
                Vector3 drawRay2 = enemyPos;
                drawRay2.Normalize();
                Vector3 drawRay3 = reflection;
                drawRay3.Normalize();
                Debug.DrawRay(transform.position, drawRay2, Color.blue);
                Debug.DrawRay(transform.position, drawRay3, new Color(1.0f, 0.0f, 1.0f));
            }
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
