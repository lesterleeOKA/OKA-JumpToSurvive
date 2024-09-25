using UnityEngine;
using UnityEngine.SceneManagement;


public class LoaderConfig : GameSetting
{
    public static LoaderConfig Instance = null;
    public string unitKey = string.Empty;
    public string testURL = string.Empty;

    protected override void Awake()
    {
        if (Instance == null)
            Instance = this;

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        this.LoadGameData();
#endif
    }

    protected override void Update()
    {
        base.Update();
    }

    public void LoadGameData()
    {
        StartCoroutine(apiManager.PostGameSetting(this.GetParseURLParams, this.LoadQuestions));
    }

  
    public void LoadQuestions()
    {
        this.InitialGameSetup(()=>
        {
            QuestionManager.Instance?.LoadQuestionFile(this.unitKey, () => this.finishedLoadQuestion());
        });    
    }

    void finishedLoadQuestion()
    {
        ExternalCaller.HiddenLoadingBar();
        this.changeScene(1);
    }

    void GetParseURLParams()
    {
        this.CurrentURL = string.IsNullOrEmpty(Application.absoluteURL) ? this.testURL : Application.absoluteURL;
        string[] urlParts = this.CurrentURL.Split('?');
        if (urlParts.Length > 1)
        {
            string queryString = urlParts[1];
            string[] parameters = queryString.Split('&');

            foreach (string parameter in parameters)
            {
                string[] keyValue = parameter.Split('=');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0];
                    string value = keyValue[1];
                    LogController.Instance?.debug($"Parameter Key: {key}, Value: {value}");

                    if (!string.IsNullOrEmpty(value))
                    {

                        switch (key)
                        {
                            case "jwt":
                                this.apiManager.jwt = value;
                                LogController.Instance?.debug("Current jwt: " + this.apiManager.jwt);
                                break;
                            case "id":
                                this.apiManager.appId = value;
                                LogController.Instance?.debug("Current app/book id: " + this.apiManager.appId);
                                break;
                            case "unit":
                                this.unitKey = value;
                                LogController.Instance?.debug("Current Game Unit: " + this.unitKey);
                                break;
                            case "gameTime":
                                this.GameTime = float.Parse(value);
                                LogController.Instance?.debug("Game Time: " + this.GameTime);
                                this.ShowFPS = true;
                                break;
                            case "playerNumbers":
                                this.PlayerNumbers = int.Parse(value);
                                LogController.Instance?.debug("player Numbers: " + this.PlayerNumbers);
                                break;
                        }
                    }
                }
            }
        }
    }

    public void closeLoginErrorBox()
    {
        this.apiManager.resetLoginErrorBox();
    }

    public void changeScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

}

