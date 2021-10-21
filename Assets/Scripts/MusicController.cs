using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum Music { MainMusic = 0, BossMusic = 2 };

public class MusicController : MonoBehaviour
{
    public AudioClip m_bossMusic;
    public AudioClip m_mainMusic;

    public AudioMixer m_audioMixer;
    public AudioMixerGroup m_bossMusicMixerGroup;
    public AudioMixerGroup m_mainMusicMixerGroup;

    // Store two AudioClips of current music clip for seamless looping
    public AudioClip[] m_currentClips = new AudioClip[4];

    private int m_bossMusicLoopPoint = 176327;  // in samples
    private int m_mainMusicLoopPoint = 4489493; // in samples

    private double m_nextEventTime;
    private int m_flip = 0;
    private AudioSource[] m_audioSources = new AudioSource[4];
    private bool m_running = false;
    private int m_loopNumber = 0;

    private bool m_lock = false;

    private int m_currentMusic;

    void Start()
    {
        GameObject m_mainCamera = Camera.main.gameObject;

        for (int i = 0; i < 2; i++)
        {
            m_audioSources[i] = m_mainCamera.AddComponent<AudioSource>();
            m_audioSources[i].outputAudioMixerGroup = m_mainMusicMixerGroup;
            m_currentClips[i] = m_mainMusic;
        }

        for (int i = 2; i < 4; i++)
        {
            m_audioSources[i] = m_mainCamera.AddComponent<AudioSource>();
            m_audioSources[i].outputAudioMixerGroup = m_bossMusicMixerGroup;
            m_currentClips[i] = m_bossMusic;
        }

        m_nextEventTime = m_mainMusic.samples;
        m_currentMusic = (int) Music.MainMusic;

        m_nextEventTime = AudioSettings.dspTime + 2.0d;
        m_running = true;
    }

    public void ChangeMusic(Music music)
    {
        if (m_lock) return;

        m_lock = true;

        if (music == Music.MainMusic)
        {
            StartCoroutine(FadeMixerGroup.StartFade(m_audioMixer, "BossMusicVolume", 1.0f, 0.0f));
            StartCoroutine(FadeMixerGroup.StartFade(m_audioMixer, "MainMusicVolume", 1.0f, 1.0f));
        }
        else
        {
            StartCoroutine(FadeMixerGroup.StartFade(m_audioMixer, "MainMusicVolume", 1.0f, 0.0f));
            StartCoroutine(FadeMixerGroup.StartFade(m_audioMixer, "BossMusicVolume", 1.0f, 1.0f));
        }

        m_currentMusic = (int) music;
        m_nextEventTime = AudioSettings.dspTime + 1.0f;

        m_loopNumber = 0;

        m_lock = false;
    }

    public void StopMusic()
    {
        foreach (AudioSource audioSource in m_audioSources)
        {
            audioSource.Stop();
        }
    }

    void Update()
    {
        if (!m_running)
        {
            return;
        }

        if (AudioSettings.dspTime + 1.0f > m_nextEventTime)
        {
            if (m_currentMusic == (int) Music.MainMusic)
            {
                m_audioSources[m_flip + m_currentMusic].timeSamples = m_loopNumber * m_mainMusicLoopPoint;
            }
            else
            {
                m_audioSources[m_flip + m_currentMusic].timeSamples = m_loopNumber * m_bossMusicLoopPoint;
            }

            m_audioSources[m_flip + m_currentMusic].clip = m_currentClips[m_flip + m_currentMusic];

            if (m_audioSources[1 - m_flip + m_currentMusic].clip != m_currentClips[m_flip + (int) m_currentMusic])
            {
                m_audioSources[1 - m_flip + m_currentMusic].clip = m_currentClips[m_flip + (int) m_currentMusic];
            }

            m_audioSources[m_flip + m_currentMusic].PlayScheduled(m_nextEventTime);

            Debug.Log("Scheduled source " + (m_flip + m_currentMusic) + " to start at time " + m_nextEventTime);

            if (m_currentMusic == (int) Music.MainMusic)
            {
                m_nextEventTime += (double) (m_mainMusic.samples - m_loopNumber * m_mainMusicLoopPoint) / m_mainMusic.frequency;
            }
            else
            {
                m_nextEventTime += (double) (m_bossMusic.samples - m_loopNumber * m_bossMusicLoopPoint) / m_bossMusic.frequency;
            }

            if (m_loopNumber == 0) m_loopNumber++;

            m_flip = 1 - m_flip;
        }
    }

}
