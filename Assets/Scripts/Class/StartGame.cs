using UnityEngine;
using UnityEngine.Events;

public class StartGame : MonoBehaviour
{
    public static StartGame Instance = null;
    public UIImage startupCountDown;
    public float count = 0f;
    public bool startedGame = false;
    public UnityEvent finishedEvent;
    private CanvasGroup cg = null;
    private float lastPlayTime = 0f;
    private float playDelay = 1f;

    private void Awake()
    {
        if(Instance == null) { Instance = this;}
    }
    // Start is called before the first frame update
    void Start()
    {
        this.startedGame = false;
        this.cg = GetComponent<CanvasGroup>();
        SetUI.Set(this.cg, true, 0f);
        this.startupCountDown.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.startedGame)
        {
            if(this.count < this.startupCountDown.cg.Length)
            {
                this.count += Time.deltaTime;
                this.startupCountDown.toImage((int)this.count);

                if (Time.time - lastPlayTime >= playDelay)
                {
                    if (AudioController.Instance != null)
                    {
                        if (startupCountDown.cg.Length - (int)count <= 1)
                        {
                            AudioController.Instance.PlayAudio(5);
                        }
                        else
                        {
                            AudioController.Instance.PlayAudio(4);
                        }
                    }                    
                    lastPlayTime = Time.time;
                }

                

                //if (LogController.Instance != null) LogController.Instance.debug("prepare counting:" + this.count);
            }
            else
            {
                this.startedGame = true;
                this.count = 0;
                SetUI.Set(this.cg, false, 0f);
                if (this.finishedEvent != null) this.finishedEvent.Invoke();
            }
        }
    }
}
