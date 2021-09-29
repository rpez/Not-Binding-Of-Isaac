using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int m_health = 6;
    public int m_maxHealth = 6;

    public float m_attackSpeed = 1f;
    public float m_accelerationTime = 1f;
    public float m_maxSpeed = 5f;

    public GameObject m_projectile;
    public GameObject m_shootPosition;

    private Rigidbody2D m_rigidbody;
    private float m_lastShot = 0f;
    private float m_shootInterval;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_shootInterval = 1f / m_attackSpeed;
        m_lastShot = m_shootInterval;
    }

    // Update is called once per frame
    void Update()
    {
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

            m_lastShot = 0f;
        }
    }

    private void FixedUpdate()
    {
        // Get input direction
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direction.Normalize();

        // Manually lerp from current speed to max speed
        m_rigidbody.velocity = m_rigidbody.velocity * 0.8f + direction * m_maxSpeed * 0.2f;

        // Not entirely sure if this does what it is supposed to :D
        // Should make turning snappier tho
        if ((Input.GetAxisRaw("Horizontal") == 1 && m_rigidbody.velocity.x < 0) ||
            (Input.GetAxisRaw("Horizontal") == -1 && m_rigidbody.velocity.x > 0))
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x * 0f, m_rigidbody.velocity.y);
        if ((Input.GetAxisRaw("Vertical") == 1 && m_rigidbody.velocity.y < 0) ||
            (Input.GetAxisRaw("Vertical") == -1 && m_rigidbody.velocity.y > 0))
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, m_rigidbody.velocity.y * 0f);
    }

    private void Shoot(Vector3 dir)
    {
        GameObject pro = GameObject.Instantiate(m_projectile, m_shootPosition.transform.position, Quaternion.identity);
        pro.GetComponent<Projectile>().Init(dir, m_rigidbody.velocity);
    }
}
