using DG.Tweening;
using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance = null;
    public AudioClip[] audioClips; // Array of audio clips to be played
    private AudioSource audioSource;
    [SerializeField]
    private AudioSource bgmAudioSource;
    public bool audioStatus = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(int index, bool loop=false)
    {
        if(!this.audioStatus)
            return;

        if (index < 0 || index >= audioClips.Length)
        {
            if(LogController.Instance != null) LogController.Instance.debugError("Audio clip index out of range.");
            return;
        }

        audioSource.clip = audioClips[index];
        audioSource.Play();
        audioSource.loop = loop;
    }

    public void PauseAudio()
    {
        audioSource.Pause();
    }

    public void ResumeAudio()
    {
        audioSource.UnPause();
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void SetPitch(float pitch)
    {
        audioSource.pitch = pitch;
    }

    public float GetCurrentTime()
    {
        return audioSource.time;
    }

    public float GetTotalDuration()
    {
        return audioSource.clip.length;
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    public void changeBGMStatus(bool status)
    {
        if (this.bgmAudioSource != null)
        {
            this.audioStatus = status;
            this.bgmAudioSource.enabled = this.audioStatus;
        }
    }
}

