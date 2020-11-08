using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip MainTheme;
    public AudioClip DesertTheme;
    public AudioClip ForestTheme;
    public AudioClip SnowTheme;

    private AudioSource _audioSource;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            _audioSource = GetComponent<AudioSource>();
            Instance = this;
        }
        else
        {
            if (Instance != this)
                Destroy(this);
        }
    }

    public void PlayMainTheme()
    {
        if (_audioSource.clip != null && _audioSource.clip.Equals(MainTheme)) return;
        _audioSource.clip = MainTheme;
        _audioSource.Play();
    }


    public void PlayDesertTheme()
    {
        _audioSource.clip = DesertTheme;
        _audioSource.Play();
    }

    public void PlayForestTheme()
    {
        _audioSource.clip = ForestTheme;
        _audioSource.Play();
    }

    public void PlaySnowTheme()
    {
        _audioSource.clip = SnowTheme;
        _audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        _audioSource.volume = volume;
    }

    public float GetVolume()
    {
        return _audioSource.volume;
    }
}
