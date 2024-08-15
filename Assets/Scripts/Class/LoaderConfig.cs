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

    private void Start()
    {
#if UNITY_EDITOR
        this.LoadQuestions();
#endif
    }

    public void LoadQuestions()
    {
        string currentURL = string.IsNullOrEmpty(Application.absoluteURL) ? this.testURL : Application.absoluteURL;
        LogController.Instance?.debug("currentURL: " + currentURL);
        if (string.IsNullOrEmpty(this.unitKey)) {
            this.unitKey = ParseURLParams(currentURL);
        }
        else
        {
            QuestionManager.Instance?.LoadQuestionFile(this.unitKey, () => this.finishedLoadQuestion());
        }
    }

    void finishedLoadQuestion()
    {
        ExternalCaller.HiddenLoadingBar();
        this.changeScene(1);
    }


    string ParseURLParams(string url)
    {
        string[] urlParts = url.Split('?');
        string unitKey = "";
        if (urlParts.Length > 1)
        {
            string queryString = urlParts[1];
            string[] parameters = queryString.Split('&');
            // Loop through the parameters and extract the key-value pairs
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
                            case "unit":
                                this.unitKey = value;
                                LogController.Instance?.debug("Current Game Unit: " + this.CurrentUnit);
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
        QuestionManager.Instance?.LoadQuestionFile(this.unitKey, () => this.finishedLoadQuestion());
        return unitKey;
    }

    public void changeScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    public string CurrentUnit
    {
        get { return this.unitKey; }
        set { this.unitKey = value; }
    }

}

