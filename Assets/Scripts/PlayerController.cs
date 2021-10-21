using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int m_health = 6;
    public int m_maxHealth = 6;
    public float m_invincibilityTime = 0.5f;

    public float m_attackSpeed = 1f;
    public float m_damageModifier = 1f;
    public float m_maxSpeed = 5f;

    public GameObject m_projectile;
    public GameObject m_shootPosition;
    private SoundController m_audioController;
    public GameUI m_ui;

    public Animator m_animator;

    private Rigidbody2D m_rigidBody;
    private float m_lastShot = 0f;
    private float m_shootInterval;

    private bool m_invincible;
    private float m_invincibilityCounter = 0f;
    private bool m_isDead;
    private bool m_itemAnimationQueued = false;
    private bool m_controlsActive = false;

    public bool IsDead => m_isDead;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            m_animator.runtimeAnimatorController = item.m_mutationAnimator;
            ApplyModifiers(item);

            // Because the change of animator overwrites the animation state change, queue it instead
            m_itemAnimationQueued = true;

            Destroy(item.gameObject);

            PlaySound("ItemPickup");
        }
        if (collision.gameObject.tag == "FloorComplete")
        {
            m_ui.GameOver(true);
        }
    }

    public void ApplyModifiers(Item item)
    {
        m_attackSpeed *= item.m_attackSpeedModifier;
        m_damageModifier *= item.m_damageModifier;
        m_maxSpeed *= item.m_speedModifier;
        //m_health = (int) (m_maxHealth * item.m_healthModifier) + 1;

        m_shootInterval = 1f / m_attackSpeed;
    }

    public void DamagePlayer(int amount)
    {
        if (!m_invincible)
        {
            m_health -= amount;
            m_invincible = true;
            m_invincibilityCounter = 0f;
            if (m_health <= 0)
            {
                m_health = 0;
                m_isDead = true;
                m_rigidBody.velocity = Vector2.zero;
                PlaySound("IsaacDeath");
                PlayAnimation("Death");
                m_ui.GameOver(false);
            }
            else
            {
                PlaySound("IsaacDamage", playRandom: true);
                PlayAnimation("TakeDamage");
            }
            m_ui.UpdatePlayerHealth(m_health);
            
        }
    }

    public void ToggleControls(bool active)
    {
        m_controlsActive = active;
        m_rigidBody.velocity = Vector2.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_audioController = Camera.main.GetComponent<SoundController>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_shootInterval = 1f / m_attackSpeed;
        m_lastShot = m_shootInterval;
        m_isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (!m_controlsActive) return;
        if (m_isDead) return;

        m_lastShot += Time.deltaTime;

        if (m_lastShot >= m_shootInterval)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                Shoot(Vector3.up);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                Shoot(Vector3.left);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                Shoot(Vector3.down);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                Shoot(Vector3.right);
            }
        }

        if (m_invincible)
        {
            m_invincibilityCounter += Time.deltaTime;
            if (m_invincibilityCounter >= m_invincibilityTime)
            {
                m_invincible = false;
            }
        }

        AnimateMovement();
        PlayWalkSounds();
    }

    private void FixedUpdate()
    {
        if (!m_controlsActive) return;
        if (m_isDead) return;

        // Get input direction
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direction.Normalize();

        // Manually lerp from current speed to max speed
        m_rigidBody.velocity = m_rigidBody.velocity * 0.8f + direction * m_maxSpeed * 0.2f;

        // Not entirely sure if this does what it is supposed to :D
        // Should make turning snappier tho
        if ((Input.GetAxisRaw("Horizontal") == 1 && m_rigidBody.velocity.x < 0) ||
            (Input.GetAxisRaw("Horizontal") == -1 && m_rigidBody.velocity.x > 0))
            m_rigidBody.velocity = new Vector2(m_rigidBody.velocity.x * 0f, m_rigidBody.velocity.y);
        if ((Input.GetAxisRaw("Vertical") == 1 && m_rigidBody.velocity.y < 0) ||
            (Input.GetAxisRaw("Vertical") == -1 && m_rigidBody.velocity.y > 0))
            m_rigidBody.velocity = new Vector2(m_rigidBody.velocity.x, m_rigidBody.velocity.y * 0f);
    }

    private void AnimateMovement()
    {
        if (m_itemAnimationQueued)
        {
            // Animation atm hard coded to the donut version
            PlayAnimation("Item");
            m_itemAnimationQueued = false;
            return;
        }

        // don't interrupt taking damage
        if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage") ||
            m_animator.GetCurrentAnimatorStateInfo(0).IsName("Item")) {
            return;
        }

        if (Input.GetAxisRaw("Vertical") > 0)
        {
            PlayAnimation("MoveUp");
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            PlayAnimation("MoveDown");
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            PlayAnimation("MoveLeft");
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            PlayAnimation("MoveRight");
        }
        else {
            PlayAnimation("Stand");
        }
    }

    private void PlayWalkSounds()
    {
        if (MovementKeysDown())
        {
            PlaySound("IsaacWalk", playRandom: true);
        }
        else if (MovementKeysHeld())
        {
            StartCoroutine(PlayWalkCycleSound());
        }
        else if (MovementKeysUp())
        {
            StopAllCoroutines();
        }
    }

    private bool MovementKeysHeld()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
            return true;
        }
        return false;
    }

    private bool MovementKeysUp()
    {
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D)) {
            return true;
        }
        return false;
    }

    private bool MovementKeysDown()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) {
            return true;
        }
        return false;
    }

    private void Shoot(Vector3 dir)
    {
        GameObject pro = GameObject.Instantiate(m_projectile, m_shootPosition.transform.position, Quaternion.identity);
        pro.GetComponent<Projectile>().Init(dir, m_rigidBody.velocity, m_damageModifier);
        m_lastShot = 0f;
    }

    public void PlayAnimation(string animation)
    {
        m_animator.Play(animation);
    }

    public void PlaySound(string sound, int index = 1, bool playRandom = false)
    {
        m_audioController.PlayOneShot(sound, index, playRandom);
    }

    private IEnumerator PlayWalkCycleSound()
    {
        yield return new WaitForSeconds(0.25f);
        PlaySound("IsaacWalk", playRandom: true);
    }
}
