using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public PlayerController m_player;

    public Sprite m_fullHeart;
    public Sprite m_halfHeart;
    public Sprite m_emptyHeart;

    public GameObject m_healthContainer;
    public GameObject m_bossScreen;
    public GameObject m_credits;
    public GameObject m_gameOver;
    public GameObject m_title;

    private List<Image> m_hearts = new List<Image>();

    private MusicController m_musicController;
    private SoundController m_audioController;

    private void Start()
    {
        m_audioController = Camera.main.GetComponent<SoundController>();
        m_musicController = Camera.main.GetComponent<MusicController>();
        m_hearts.AddRange(m_healthContainer.GetComponentsInChildren<Image>());
    }

    public void UpdatePlayerHealth(int health)
    {
        int fulls = health / 2;
        for (int i = 0; i < m_hearts.Count; i++)
        {
            if (i < fulls) m_hearts[i].sprite = m_fullHeart;
            else if (i == fulls && health % 2 == 1) m_hearts[i].sprite = m_halfHeart;
            else m_hearts[i].sprite = m_emptyHeart;
        }
    }

    public void BossScreen()
    {
        m_bossScreen.SetActive(true);
        m_player.ToggleControls(false);
        StartCoroutine(CloseBossScreen(3f));
    }

    public void GameOver(bool victory)
    {
        if (victory)
        {
            m_credits.SetActive(true);
            m_player.ToggleControls(false);
        }
        else
        {
            m_gameOver.SetActive(true);
            m_musicController.StopMusic();
            m_audioController.PlayOneShot("GameOver");
            m_player.ToggleControls(false);
        }
    }

    public void CloseTitleScreen()
    {
        m_title.SetActive(false);
        m_player.ToggleControls(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
        m_musicController.ChangeMusic(Music.MainMusic);
        m_player.ToggleControls(false);
    }

    private IEnumerator CloseBossScreen(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_bossScreen.SetActive(false);
        m_player.ToggleControls(true);
    }
}
