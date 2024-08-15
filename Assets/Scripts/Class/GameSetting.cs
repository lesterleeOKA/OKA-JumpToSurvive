using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    public GameSetup gameSetup;
    protected virtual void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        DontDestroyOnLoad(this);

        StartCoroutine(this.gameSetup.Load("GameUI", "bg", _bgTexture =>
        {
            this.gameSetup.bgTexture = _bgTexture;
        }));
    }

    public void InitialGameBackground()
    {
        this.gameSetup.setBackground();
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
}

[System.Serializable]
public class GameSetup: LoadImage
{
    [Tooltip("Default Game Background Texture")]
    public Texture bgTexture;
    [Tooltip("Find Tag name of GameBackground in different scene")]
    public RawImage gameBackground;
    public float gameTime;
    public bool showFPS = false;


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
