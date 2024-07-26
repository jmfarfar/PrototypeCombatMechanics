using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] private AudioSource backgroundMusic;

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null)
        {
        }
        else
        {
            instance = this;
            // Destroy(gameObject);
            DontDestroyOnLoad(instance);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip, AudioMixerGroup mixer = null)
    {
        if (mixer == null)
        {
            audioSource.PlayOneShot(clip);
            audioSource.loop = true;
        }
        else
        {
            audioSource.PlayOneShot(clip);
            audioSource.outputAudioMixerGroup = mixer;
        }
    }

    public void PlayAmbiental(AudioClip clip)
    {
        backgroundMusic.PlayOneShot(clip);
    }

    public void PlayAmbiental()
    {
        backgroundMusic.Play();
    }

    public void StopBackgroundClip()
    {
        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }
    }

    public void StopClip()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    public bool IsBackgroundPlaying()
    {
        return backgroundMusic.isPlaying;
    }
}