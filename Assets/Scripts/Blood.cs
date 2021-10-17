using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{

    public SpriteRenderer m_spriteRenderer;
    public List<Sprite> m_sprites;
    public Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        // Randomly select from a list of sprites
        m_spriteRenderer.sprite = m_sprites[UnityEngine.Random.Range(0, m_sprites.Count)];

        // Randomly select a sprite color from a range of light to dark red tones
        m_spriteRenderer.color = new Color(UnityEngine.Random.Range(0.53f, 0.73f), 0.0f, 0.0f, 1.0f);

        // Rotate randomly by 0, 45, ..., 315 degrees
        gameObject.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 8) * 45);

        PlayAnimation("SpawnBlood");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAnimation(string animation)
    {
        m_animator.Play(animation);
    }
}
