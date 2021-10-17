using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
    public GameObject m_projectilePrefab;
    public int m_burstSize;
    private bool m_active;

    private enum BossState { Normal, Shoot, IntoPool, TargetIsaac, OutOfPool, Wait };

    public float m_waitTime;
    private BossState m_state = BossState.IntoPool;

    private PlayableDirector m_director;
    private TimelineAsset m_currentTimeline;

    public PlayableAsset m_shootTimeline;
    public PlayableAsset m_intoPoolTimeline;
    public PlayableAsset m_outOfPoolTimeline;
    public PlayableAsset m_normalTimeline;

    public GameObject m_shootPosition;
    public float m_arcSegment;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");
        Physics2D.queriesStartInColliders = false;
        m_active = false;

        m_director = GetComponent<PlayableDirector>();
        ChangeTimeline(m_intoPoolTimeline);
        m_currentTimeline = m_director.playableAsset as TimelineAsset;
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

        switch (m_state)
        {
            case BossState.Normal:
                break;
            case BossState.Shoot:
                break;
            case BossState.IntoPool:
                break;
            case BossState.TargetIsaac:
                transform.position = m_player.transform.position;
                m_state = BossState.OutOfPool;
                break;
            case BossState.OutOfPool:
                break;
            default:
                break;
        }
    }

    public void OnAnimationEnd()
    {
        switch (m_state)
        {
            case BossState.Normal:
                m_state = BossState.IntoPool;
                ChangeTimeline(m_intoPoolTimeline);
                break;
            case BossState.Shoot:
                m_state = BossState.Normal;
                ChangeTimeline(m_normalTimeline);
                break;
            case BossState.IntoPool:
                m_state = BossState.TargetIsaac;
                ChangeTimeline(m_outOfPoolTimeline);
                break;
            case BossState.OutOfPool:
                m_state = BossState.Shoot;
                ChangeTimeline(m_shootTimeline);
                break;
            default:
                break;
        }
    }

    public void Shoot()
    {
        Vector3 pos = m_player.transform.position;
        Vector2 dir = pos - transform.position;
        dir.Normalize();
        int startOffset = -m_burstSize / 2;
        for (int i = 0; i < m_burstSize; i++)
        {
            GameObject pro = GameObject.Instantiate(m_projectilePrefab, m_shootPosition.transform.position, Quaternion.identity);
            Projectile script = pro.GetComponent<Projectile>();
            Vector3 newDir = Quaternion.Euler(0, (startOffset + i) * m_arcSegment, 0) * dir;
            script.Init(newDir, Vector2.zero);
        }
    }

    public void ChangeTimeline(PlayableAsset asset)
    {
        m_director.playableAsset = asset;

        m_director.RebuildGraph();
        m_director.time = 0.0;
        m_director.Play();
    }

    public void MoveTo(Vector3 position)
    {
        //Vector2 direction = new Vector2(position.x, position.y);
        //direction.Normalize();

        //// Manually lerp from current speed to max speed
        //SetRigidbodyVelocity(m_rigidBody.velocity * 0.8f + direction * m_maxSpeed * 0.2f);
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
