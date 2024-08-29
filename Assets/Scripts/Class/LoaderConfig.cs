using UnityEngine;
using UnityEngine.SceneManagement;


public class LoaderConfig : GameSetting
{
    public static LoaderConfig Instance = null;
    public string unitKey = string.Empty;
    public string testURL = string.Empty;
    public APIManager apiManager;

    protected override void Awake()
    {
        if (Instance == null)
            Instance = this;

        base.Awake();
    }

    private void Start()
    {
        this.apiManager.Init();
#if UNITY_EDITOR
        this.LoadGameData();
#endif
    }

    private void Update()
    {
        this.apiManager.controlDebugLayer();
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
                            case "unit":
                                this.unitKey = value;
                                LogController.Instance?.debug("Current Game Unit: " + this.unitKey);
                                break;
                            case "gameTime":
                                this.GameTime = float.Parse(value);
                                LogController.Instance?.debug("Game Time: " + this.GameTime);
                                this.ShowFPS = true;
                                break;
                        }
                    }
                }
            }
        }
    }


    public void changeScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

}

