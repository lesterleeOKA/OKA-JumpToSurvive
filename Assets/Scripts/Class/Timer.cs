using DG.Tweening;
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
    [SerializeField]
    private Color32 pinkColor = Color.yellow;
    public bool endGame = false;
    public UnityEvent finishedEvent;
    private AudioSource lastTenDingDing = null;
    private Tween timerScaleTween = null;

    private void Start()
    {
        this.lastTenDingDing = GetComponent<AudioSource>();
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

                        if (AudioController.Instance.audioStatus)
                        {
                            this.lastTenDingDing.Play();
                            this.lastTenDingDing.loop = true;
                        }

                        if(this.timer.color == Color.white && this.timerScaleTween == null) {
                            this.timerScaleTween = this.timer.transform.DOScale(0.8f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                            this.timer.color = this.pinkColor;
                        }
                    }
                    else
                    {
                        if (!AudioController.Instance.audioStatus)
                        {
                            if(isSoundPlay)
                            {
                                this.lastTenDingDing.Stop();
                                isSoundPlay = false;
                            }
                        }
                    }
                    
                }
                this.currentTime -= Time.deltaTime;
                this.UpdateTimerText();
            }
            else
            {
                if (this.timerScaleTween != null && this.timerScaleTween.IsActive()) this.timerScaleTween.Kill();
                this.currentTime = 0f;
                this.UpdateTimerText();
                this.endGame = true;
                this.lastTenDingDing.Stop();
                if (this.finishedEvent != null) this.finishedEvent.Invoke();
            }
        }

    }

    public void Init()
    {
        this.endGame = false;

        if(LoaderConfig.Instance != null && LoaderConfig.Instance.GameTime > 0f) 
            this.currentTime = LoaderConfig.Instance.GameTime;
        else
            this.currentTime = this.gameDuration;

        this.UpdateTimerText();    
    }
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(this.currentTime / 60f);
        int seconds = Mathf.FloorToInt(this.currentTime % 60f);
        this.timer.text = $"{minutes:D2}:{seconds:D2}";
    }

}
