using System;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    public string currentURL;
    public GameSetup gameSetup;
    protected virtual void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        DontDestroyOnLoad(this);
    }

    public void InitialGameSetup(Action onCompleted = null)
    {
        //Download game background image
        StartCoroutine(this.gameSetup.Load("GameUI", "bg", _bgTexture =>
        {
            LogController.Instance?.debug($"Downloaded bg Image!!");
            ExternalCaller.UpdateLoadBarStatus("Loading Bg");
            this.gameSetup.bgTexture = _bgTexture;
            onCompleted?.Invoke();
        }));
    }

    public void InitialGameBackground()
    {
        this.gameSetup.setBackground();
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
}

[Serializable]
public class GameSetup: LoadImage
{
    [Tooltip("Default Game Background Texture")]
    public Texture bgTexture;
    [Tooltip("Find Tag name of GameBackground in different scene")]
    public RawImage gameBackground;
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
}


public static class APIConstant
{
    public static string QuestionDataHeaderName = "Data";
    public static string GameDataAPI {
        get
        {
            return "https://dev.openknowledge.hk/RainbowOne/index.php/PHPGateway/proxy/2.8/?api=ROGame.get_game_setting&json=[1]&jwt=";
        }
    }
}
