using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderConfig : MonoBehaviour
{
    public static LoaderConfig Instance = null;
    public string unitKey = string.Empty;
    public float gameTime;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        string currentURL = Application.absoluteURL;
        if (LogController.Instance != null) LogController.Instance.debug("currentURL: " + currentURL);
        if (string.IsNullOrEmpty(this.unitKey)) this.unitKey = ParseURLParams(currentURL);
        if (QuestionManager.Instance != null) QuestionManager.Instance.LoadQuestionFile(this.unitKey);
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
                    if (LogController.Instance != null) LogController.Instance.debug($"Parameter Key: {key}, Value: {value}");

                    if (!string.IsNullOrEmpty(value))
                    {

                        switch (key)
                        {
                            case "unit":
                                this.unitKey = value;
                                if (LogController.Instance != null) LogController.Instance.debug("Current Game Unit: " + this.CurrentUnit);
                                break;
                            case "gameTime":
                                this.gameTime = float.Parse(value);
                                if (LogController.Instance != null) LogController.Instance.debug("Game Time: " + this.GameTime);
                                break;
                        }
                    }
                }
            }
        }
        return unitKey;
    }

    public void changeScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }


    public string CurrentUnit
    {
        get
        {
            return this.unitKey;
        }
        set
        {
            this.unitKey = value;
        }
    }


    public float GameTime
    {
        get
        {
            return this.gameTime;
        }
        set
        {
            this.gameTime = value;
        }
    }

}

