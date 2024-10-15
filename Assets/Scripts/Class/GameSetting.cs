using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    public HostName currentHostName = HostName.dev;
    public string currentURL;
    public GameSetup gameSetup;
    public APIManager apiManager;
    protected virtual void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        DontDestroyOnLoad(this);
    }

    protected virtual void Start()
    {
        this.apiManager.Init();
    }

    protected virtual void Update()
    {
        this.apiManager.controlDebugLayer();
    }

    public void InitialGameImages(Action onCompleted = null)
    {
        if (this.apiManager.IsLogined)
        {
           this.initialGameImagesByAPI(onCompleted);
        }
        else
        {
            this.initialGameImagesByLocal(onCompleted);
        }
    }

    private void initialGameImagesByLocal(Action onCompleted = null)
    {
        //Download game background image from local streaming assets
        this.gameSetup.loadImageMethod = LoadImageMethod.StreamingAssets;
        StartCoroutine(this.gameSetup.Load("GameUI", "bg", _bgTexture =>
        {
            LogController.Instance?.debug($"Downloaded bg Image!!");
            ExternalCaller.UpdateLoadBarStatus("Loading Bg");
            if (_bgTexture != null) this.gameSetup.bgTexture = _bgTexture;

            StartCoroutine(this.gameSetup.Load("GameUI", "preview", _previewTexture =>
            {
                LogController.Instance?.debug($"Downloaded preview Image!!");
                ExternalCaller.UpdateLoadBarStatus("Loading Instruction");
                if (_previewTexture != null) this.gameSetup.previewTexture = _previewTexture;
                onCompleted?.Invoke();
            } ));
        }));
    }

    private void initialGameImagesByAPI(Action onCompleted = null)
    {
        //Download game background image from api
        this.gameSetup.loadImageMethod = LoadImageMethod.Url;
        string backgroundImageUrl = this.apiManager.settings.backgroundImageUrl;
        string previewGameImageUrl = this.apiManager.settings.previewGameImageUrl;

        if(!string.IsNullOrEmpty( backgroundImageUrl)) {
            StartCoroutine(this.gameSetup.Load("", backgroundImageUrl, _bgTexture =>
            {
                LogController.Instance?.debug($"Downloaded bg Image!!");
                ExternalCaller.UpdateLoadBarStatus("Loading Bg");
                if(_bgTexture != null) this.gameSetup.bgTexture = _bgTexture;

                if (!string.IsNullOrEmpty(previewGameImageUrl))
                {
                    StartCoroutine(this.gameSetup.Load("", previewGameImageUrl, _previewTexture =>
                    {
                        LogController.Instance?.debug($"Downloaded preview Image!!");
                        ExternalCaller.UpdateLoadBarStatus("Loading Instruction");
                        if(_previewTexture != null) this.gameSetup.previewTexture = _previewTexture;
                        onCompleted?.Invoke();
                    }));
                }
                else
                {
                    LogController.Instance?.debug($"Missing previewGameImage Url!!");
                    onCompleted?.Invoke();
                }
            }));
        }
        else
        {
            LogController.Instance?.debug($"Missing backgroundImage Url!!");
            onCompleted?.Invoke();
        }   
    }

    public void InitialGameSetup()
    {
        this.gameSetup.setBackground();
        this.gameSetup.setInstruction(this.apiManager.settings.instructionContent);
    }

    public string CurrentURL
    {
        set { this.currentURL = value; }
        get { return this.currentURL; }
    }

    public float GameTime
    {
        get{return this.gameSetup.gameTime;}
        set{this.gameSetup.gameTime = value;}
    }

    public bool ShowFPS
    {
        get { return this.gameSetup.showFPS; }
        set { this.gameSetup.showFPS = value; }
    }

    public int PlayerNumbers
    {
        get { return this.gameSetup.playerNumber; }
        set { this.gameSetup.playerNumber = value; }
    }

    public string CurrentHostName
    {
        get
        {
            return currentHostName switch
            {
                HostName.dev => "https://dev.openknowledge.hk",
                HostName.prod => "https://www.rainbowone.app/",
                _ => throw new NotImplementedException()
            };
        }
    }

    public void Reload()
    {
        ExternalCaller.ReLoadCurrentPage();
    }

    public void changeScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
}

[Serializable]
public class GameSetup: LoadImage
{
    [Tooltip("Default Game Background Texture")]
    public Texture bgTexture;
    [Tooltip("Default Game Preview Texture")]
    public Texture previewTexture;
    [Tooltip("Find Tag name of GameBackground in different scene")]
    public RawImage gameBackground;
    [Tooltip("Instruction Preview Image")]
    public RawImage gamePreview;
    public InstructionText instructions;
    public float gameTime;
    public bool showFPS = false;
    public int playerNumber = 1;

    public void setBackground()
    {
        if (this.gameBackground == null)
        {
            var tex = GameObject.FindGameObjectWithTag("GameBackground");
            this.gameBackground = tex.GetComponent<RawImage>();
        }

        if(this.gameBackground != null)
        {
            this.gameBackground.texture = this.bgTexture;
        }
    }

    public void setInstruction(string content="")
    {
        if (!string.IsNullOrEmpty(content) && this.instructions == null)
        {
            var instructionText = GameObject.FindGameObjectWithTag("Instruction");

            if(instructionText != null)
            {
                this.instructions = instructionText.GetComponent<InstructionText>();
                this.instructions.setContent(content);
            }
        }

        if (this.gamePreview == null)
        {
            var preview = GameObject.FindGameObjectWithTag("GamePreview");
            if(preview != null)
            {
                this.gamePreview = preview.GetComponent<RawImage>();
                this.gamePreview.texture = this.previewTexture;
            }
        }
    }
}

public enum HostName
{
    dev,
    prod
}