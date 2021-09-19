using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float m_speed = 1;
    public GameObject m_projectile;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.up * delta * m_speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * delta * m_speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.down * delta * m_speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * delta * m_speed);
        }

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

    private void Shoot(Vector3 dir)
    {
        GameObject pro = GameObject.Instantiate(m_projectile, transform.position, Quaternion.identity);
        pro.GetComponent<Projectile>().Init(dir);
    }
}
