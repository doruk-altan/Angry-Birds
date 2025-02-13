using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayClip(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.Play();
    }

    public void PlayRandomClip(AudioClip[] clips, AudioSource source)
    {
        int index = Random.Range(0, clips.Length);

        source.clip = clips[index];
        source.Play();
    }
}
