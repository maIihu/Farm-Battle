using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    public AudioClip introMusic;
    public AudioClip gameMusic;
    public AudioClip digSE;
    public AudioClip thunderSE;
    public AudioClip tsunamiSE;
    
    public bool muteMusic;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateMusic(introMusic);
        PlayMusic();
    }

    public void UpdateMusic(AudioClip music)
    {
        musicSource.clip = music;
    }

    public void PlayMusic()
    {
        musicSource.Play();
    }
    
    public void CheckMute(bool soundOn)
    {
        if(!soundOn)
            musicSource.Pause();
        else 
            musicSource.UnPause();

        muteMusic = !soundOn;
    }
    
    public void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
