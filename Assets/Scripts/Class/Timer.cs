using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Timer : MonoBehaviour
{
    public float gameDuration = 180f; 
    public float currentTime = 0f;
    public TextMeshProUGUI timer = null;
    bool isSoundPlay=false;
    public bool endGame = false;
    public UnityEvent finishedEvent;

    private void Start()
    {
        this.Init();
    }

    private void Update()
    {
        if(this.timer == null)
            return;

        if (StartGame.Instance.startedGame && this.timer != null && !this.endGame)
        {
            if(this.currentTime > 0f)
            {
                if(this.currentTime < 10f)
                {
                    if(isSoundPlay== false)
                    {
                        isSoundPlay = true;
                        if (AudioController.Instance != null) AudioController.Instance.PlayAudio(6, true);
                        this.timer.color = Color.red;
                        
                    }
                    
                }
                this.currentTime -= Time.deltaTime;
                this.UpdateTimerText();
            }
            else
            {
                this.currentTime = 0f;
                this.UpdateTimerText();
                if (AudioController.Instance != null) AudioController.Instance.StopAudio();
                this.endGame = true;
                if (this.finishedEvent != null) this.finishedEvent.Invoke();
            }
        }

    }

    public void Init()
    {
        this.endGame = false;
        this.currentTime = this.gameDuration;
        UpdateTimerText();    
    }
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(this.currentTime / 60f);
        int seconds = Mathf.FloorToInt(this.currentTime % 60f);
        this.timer.text = $"{minutes:D2}:{seconds:D2}";
    }

}
