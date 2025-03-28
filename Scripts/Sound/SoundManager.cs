using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource _audioSorce;
    public static SoundManager Instance;
    public Sound[] sounds;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _audioSorce = GetComponent<AudioSource>();
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }
    public void PlaySound(AudioClip audio) {
        _audioSorce.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        _audioSorce.PlayOneShot(audio);
    }
    public void Play(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound " + soundName + " not found!");
            return;
        }

        s.source.Play();
    }
}
