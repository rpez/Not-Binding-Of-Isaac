using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float m_speed = 1f;
    public float m_acceleration = 10f;
    public float m_deceleration = 100f;
    public float m_maxSpeed = 5f;
    public GameObject m_projectile;

    private Rigidbody2D m_rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Shoot(Vector3.up);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Shoot(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Shoot(Vector3.down);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Shoot(Vector3.right);
        }
    }

    private void FixedUpdate()
    {
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //if (Input.GetKey(KeyCode.W)) direction.y = 1;
        //if (Input.GetKey(KeyCode.A)) direction.x = -1;
        //if (Input.GetKey(KeyCode.S)) direction.y = -1;
        //if (Input.GetKey(KeyCode.D)) direction.x = 1;
        direction.Normalize();

        Debug.Log(direction);

        m_rigidbody.AddForce(m_acceleration * direction);
        if (m_rigidbody.velocity.magnitude > m_maxSpeed) m_rigidbody.velocity = direction * m_maxSpeed;
        
        if (m_rigidbody.velocity.magnitude > 0f && direction.magnitude == 0f)
        {
            m_rigidbody.AddForce(-m_rigidbody.velocity * m_deceleration);
        }
    }

    private void Shoot(Vector3 dir)
    {
        GameObject pro = GameObject.Instantiate(m_projectile, transform.position, Quaternion.identity);
        pro.GetComponent<Projectile>().Init(dir);
    }
}
