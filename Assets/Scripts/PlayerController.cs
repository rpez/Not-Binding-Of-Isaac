using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject m_projectile;
    public Rigidbody2D m_rigidBody;

    public Vector2 m_movementInput;
    // public Vector2 m_shootingInput; // TO-DO: Make shooting velocity-based

    // Player movement values
    public float m_linearDragMoving = 16.0f;
    public float m_angularDragMoving = 6.0f;
    public float m_linearDragStopping = 800.0f;
    public float m_angularDragStopping = 800.0f;

    public float m_velocityScalar = 8.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        Move();

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

    void ProcessInputs() {
        m_movementInput = new Vector2(Input.GetAxis("MoveHorizontal"), Input.GetAxis("MoveVertical"));

        m_movementInput.Normalize();

        // TO-DO: Add a small bit of acceleration at the beginning to Isaac here.

        if (MovementKeysHeld()) {
            m_rigidBody.drag = m_linearDragMoving;
            m_rigidBody.angularDrag = m_angularDragMoving;
        } else {
            m_rigidBody.drag = m_linearDragStopping;
            m_rigidBody.angularDrag = m_angularDragStopping;
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

    private void Move()
    {
        m_rigidBody.velocity = m_movementInput * m_velocityScalar;
    }

    private void Shoot(Vector3 dir)
    {
        GameObject pro = GameObject.Instantiate(m_projectile, transform.position, Quaternion.identity);
        pro.GetComponent<Projectile>().Init(dir);
    }
}
