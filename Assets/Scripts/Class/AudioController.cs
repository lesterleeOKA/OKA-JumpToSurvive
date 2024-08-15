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
        this.audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(int index, bool loop=false)
    {
        if(!this.audioStatus)
            return;

        if (index < 0 || index >= audioClips.Length)
        {
            LogController.Instance?.debugError("Audio clip index out of range.");
            return;
        }

        this.audioSource.clip = audioClips[index];
        this.audioSource.Play();
        this.audioSource.loop = loop;
    }

    public void PauseAudio()
    {
        this.audioSource.Pause();
    }

    public void ResumeAudio()
    {
        this.audioSource.UnPause();
    }

    public void StopAudio()
    {
        this.audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        this.audioSource.volume = volume;
    }

    public void SetPitch(float pitch)
    {
        this.audioSource.pitch = pitch;
    }

    public float GetCurrentTime()
    {
        return this.audioSource.time;
    }

    public float GetTotalDuration()
    {
        return this.audioSource.clip.length;
    }

    public bool IsPlaying()
    {
        return this.audioSource.isPlaying;
    }

    public void changeBGMStatus(bool status)
    {
        if (this.bgmAudioSource != null)
        {
            this.audioStatus = status;
            this.bgmAudioSource.enabled = this.audioStatus;
        }
    }

    public void showResultAudio(bool success)
    {
        this.PlayAudio(success ? 6: 7);
    }
}

