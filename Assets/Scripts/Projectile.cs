using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float m_speed = 1;

    private Vector3 m_direction;

    public void Init(Vector3 direction)
    {
        m_direction = direction;
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        transform.Translate(m_direction * delta * m_speed);
    }
}
