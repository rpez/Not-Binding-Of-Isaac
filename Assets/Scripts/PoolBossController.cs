using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PoolBossController : MonoBehaviour, MonsterController
{
    public float m_health = 10f;
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

    private enum BossState { Normal, Shoot, Charge, IntoPool,
        TargetIsaac, OutOfPool, Wait, Dead };

    public float m_waitTime;
    private BossState m_state = BossState.Normal;

    private PlayableDirector m_director;

    public PlayableAsset m_shootTimeline;
    public PlayableAsset m_intoPoolTimeline;
    public PlayableAsset m_outOfPoolTimeline;
    public PlayableAsset m_normalTimeline;
    public PlayableAsset m_deadTimeline;
    public PlayableAsset m_chargeTimeline;

    public GameObject m_shootPosition;
    public float m_arcSegment;

    public SpriteRenderer m_bossSprite;

    private SoundController m_audioController;

    private Vector2 m_chargeTarget;
    public float m_chargeTime = 1f;
    public int m_chargeAmount = 3;
    private int m_currentCharge = 0;
    private bool m_chargeSet;
    private float m_currentTime;

    // Start is called before the first frame update
    void Start()
    {
        m_audioController = Camera.main.GetComponent<SoundController>();
        m_player = GameObject.Find("Player");
        Physics2D.queriesStartInColliders = false;
        m_active = false;

        m_director = GetComponent<PlayableDirector>();
        ChangeTimeline(m_normalTimeline);
        m_director.Pause();
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

        if (!m_active) return;
        else if (m_director.state == PlayState.Paused) m_director.Play();

        switch (m_state)
        {
            case BossState.Charge:
                if (m_currentCharge > m_chargeAmount)
                {
                    m_chargeSet = true;
                    StartCoroutine(Delayed(0.5f, () => {
                        ChangeTimeline(m_normalTimeline);
                        m_state = BossState.Normal;
                        m_currentCharge = 0;
                    }));
                    return;
                }
                m_currentTime += Time.deltaTime;
                if (!m_chargeSet)
                {
                    m_chargeTarget = m_player.transform.position;
                    m_chargeTarget -= (new Vector2(transform.position.x, transform.position.y) - m_chargeTarget).normalized;
                    m_chargeSet = true;
                    StartCoroutine(Delayed(m_chargeTime, ResetCharge));
                    m_currentTime = 0;
                }
                Vector2 newPos = Vector2.Lerp(transform.position, m_chargeTarget, m_currentTime / m_chargeTime);
                transform.position = newPos;
                break;
            case BossState.TargetIsaac:
                transform.position = m_player.transform.position;
                m_state = BossState.OutOfPool;
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
                
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    ChangeTimeline(m_chargeTimeline);
                    StartCoroutine(Delayed(m_waitTime, () => {
                        ResetCharge();
                        m_state = BossState.Charge;
                        PlaySound("BossScream");
                    }));
                }
                else
                {
                    m_state = BossState.IntoPool;
                    ChangeTimeline(m_intoPoolTimeline);
                    /*StartCoroutine(Delayed(29/50f, () => PlaySound("BossTeleportInto")));*/
                }
                break;
            case BossState.Shoot:
                m_state = BossState.Normal;
                ChangeTimeline(m_normalTimeline);
                break;
            case BossState.IntoPool:
                m_state = BossState.TargetIsaac;
                ChangeTimeline(m_outOfPoolTimeline);
                PlaySound("BossTeleportOutof");
                break;
            case BossState.OutOfPool:
                m_state = BossState.Shoot;
                ChangeTimeline(m_shootTimeline);
                PlaySound("BossProjectiles");
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
            Vector3 newDir = Quaternion.Euler(0, 0, (startOffset + i) * m_arcSegment) * dir;
            script.Init(newDir.normalized, Vector2.zero, 1f);
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
        if (m_isDead) return;

        m_health -= amount;

        // Workaround for damaging
        m_bossSprite.color = Color.red;
        StartCoroutine(Delayed(0.1f, ResetColor));
        if (m_health <= 0f)
        {
            m_bossSprite.color = Color.white;
            m_health = 0f;
            m_isDead = true;
            StopAllCoroutines();
            ChangeTimeline(m_deadTimeline);
            m_state = BossState.Dead;
            StartCoroutine(Delayed(2f, () => Destroy(gameObject)));
        }
    }

    public void SetRigidbodyVelocity(Vector2 velocity)
    {
        //m_rigidBody.velocity = velocity;
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.DamagePlayer(1);
            m_circleCollider.enabled = false;
            StartCoroutine(Delayed(0.2f, () => m_circleCollider.enabled = true));
        }
    }

    public void PlayAnimation(string animation)
    {
        //m_animator.Play(animation);
    }

    public void PlaySound(string sound, int index = 1, bool playRandom = false)
    {
        m_audioController.PlayOneShot(sound, index, playRandom);
    }

    public bool IsDead()
    {
        return m_isDead;
    }

    public void Activate()
    {
        m_active = true;
    }

    private void ResetColor()
    {
        m_bossSprite.color = Color.white;
    }

    private void ResetCharge()
    {
        m_chargeSet = false;
        m_currentCharge++;
    }

    private IEnumerator Delayed(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback.Invoke();
    }
}
