using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaperController : MonoBehaviour, MonsterController
{
    public float m_health = 10f;
    private float m_maxSpeed = 4.2f; // 42 !!!
    public bool m_isDead = false;

    public Rigidbody2D m_rigidBody;
    public CircleCollider2D m_circleCollider;
    public BoxCollider2D m_boxCollider;
    private GameObject m_player;
    private Vector3 m_parentRoomPos;
    public GameObject m_blood;

    private AStar m_AStar = new AStar();
    private NavigationGrid m_grid = new NavigationGrid();
    private IEnumerator m_AStarMoveTo;

    private SoundController m_audioController;

    public Animator m_animator;
    private bool m_active;
    private bool m_AStarMovingToNextNode;

    private Coroutine m_voice;

    // Start is called before the first frame update
    void Start()
    {
        m_audioController = Camera.main.GetComponent<SoundController>();
        m_player = GameObject.Find("Player");
        m_parentRoomPos = transform.parent.gameObject.transform.position;
        Physics2D.queriesStartInColliders = false;
        m_active = false;
    }
    
    void Update() {
        if (m_isDead)
        {
            PlayAnimation("Death");
        }
    }

    void FixedUpdate()
    {
        if (!m_active || m_isDead) return;

        RaycastToPlayer();
    }

    private void RaycastToPlayer()
    {
        Vector3 playerPos = m_player.GetComponent<Collider2D>().bounds.center - transform.position;

        // 15f length for the ray should be enough for current room size
        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerPos, 15f);

        if (hit.collider.gameObject.tag == "Wall" || hit.collider.gameObject.tag == "Obstacle" || hit.collider.gameObject.tag == "Enemy")
        {
            if (m_AStarMovingToNextNode) {
                return;
            }

            (int, int) playerCoords = m_grid.WorldToGridCoordinates(m_player.transform.position, m_parentRoomPos);
            (int, int) monsterCoords = m_grid.WorldToGridCoordinates(transform.position, m_parentRoomPos);

            if (!m_player.GetComponent<PlayerController>().IsDead)
            {
                List<Vector3Int> route = m_AStar.Solve(monsterCoords, playerCoords, transform.parent.gameObject.GetComponent<Room>().Obstacles);
                m_AStarMoveTo = AStarMoveTo(route);
                StartCoroutine(m_AStarMoveTo);
            }
        }
        else if (hit.collider.gameObject.tag == "Player")
        {
            if (m_AStarMovingToNextNode)
            {
                StopCoroutine(m_AStarMoveTo);
                m_AStarMovingToNextNode = false;
            }

            MoveTo(playerPos);
        }
    }

    private IEnumerator AStarMoveTo(List<Vector3Int> route)
    {
        foreach (Vector3Int node in route)
        {
            (int x, int y) = m_grid.WorldToGridCoordinates(transform.position, m_parentRoomPos);

            Vector3 nextNode = m_grid.GridToWorldCoordinates(node.x + x, node.y + y, m_parentRoomPos);
            MoveTo(nextNode - transform.position);

            while (Vector3.Distance(nextNode, transform.position) >= 0.01f) {
                yield return null;
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
            StartCoroutine(OnDeath());
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

    public void Activate()
    {
        m_active = true;
        PlayAnimation("Move");
        m_audioController.PlayOneShot("GaperScream", playRandom: true);
        m_voice = StartCoroutine(PlaySound());
    }

    private IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(3.0f, 5.5f));
        m_audioController.PlayOneShot("GaperScream", playRandom: true);
        if (m_active && !m_isDead) {
            PlaySound();
        }
    }

    private IEnumerator OnDeath()
    {
        if (m_voice != null) StopCoroutine(m_voice);

        Destroy(m_rigidBody);
        Destroy(m_circleCollider);
        Destroy(m_boxCollider);

        gameObject.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Background");
        gameObject.GetComponent<Renderer>().sortingOrder = 2;

        yield return new WaitForSeconds(1.0f);

        CreateBlood();

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);

    }

    private void CreateBlood()
    {
        Instantiate(m_blood, transform.position, Quaternion.identity, transform.parent);
    }
}
