using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using SimpleJSON;


public class LoaderConfig : GameSetting
{
    public static LoaderConfig Instance = null;
    public string currentURL;
    public string jwt = string.Empty;
    public string questionData = string.Empty;
    public string accountData = string.Empty;
    public string photoDataUrl = string.Empty;
    public string unitKey = string.Empty;
    public string testURL = string.Empty;
    public int maxRetries = 10;
    public CanvasGroup debugLayer;
    private Text debugText = null;

    protected override void Awake()
    {
        if (Instance == null)
            Instance = this;

        base.Awake();
    }

    public string CurrentURL
    {
        set { this.currentURL = value; }
        get { return this.currentURL; } 
    }

    private void Start()
    {

        if(this.debugLayer != null)
        {
            this.debugText = this.debugLayer.GetComponentInChildren<Text>();
        }

#if UNITY_EDITOR
        this.LoadGameData();
#endif
    }

    private bool showingDebugBox = false;
    public void Update()
    {
        if(this.debugLayer != null && Input.GetKeyDown("d"))
        {
            showingDebugBox = !showingDebugBox;
            SetUI.Set(this.debugLayer.GetComponent<CanvasGroup>(), showingDebugBox, 0f);
        }
    }

    public void LoadGameData()
    {
        StartCoroutine(PostGameSetting());
    }

    private IEnumerator PostGameSetting()
    {
        ExternalCaller.UpdateLoadBarStatus("Loading Data");
        this.GetParseURLParams();
        string api = APIConstant.GameDataAPI + this.jwt;
        WWWForm form = new WWWForm();
        int retryCount = 0;
        bool requestSuccessful = false;

        while (retryCount < this.maxRetries && !requestSuccessful)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(api, form))
            {
                // Set headers
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("typ", "jwt");
                www.SetRequestHeader("alg", "HS256");

                // Send the request and wait for a response
                yield return www.SendWebRequest();

                // Check for errors
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + www.error);
                    retryCount++;
                    Debug.Log("Retrying... Attempt " + retryCount);
                    yield return new WaitForSeconds(2); // Wait for 2 seconds before retrying
                }
                else
                {
                    requestSuccessful = true;
                    string responseText = www.downloadHandler.text;
                    int jsonStartIndex = responseText.IndexOf("{\"Data\":");
                    if (jsonStartIndex != -1)
                    {
                        string jsonData = responseText.Substring(jsonStartIndex);
                        Debug.Log("Response: " + jsonData);

                        var jsonNode = JSONNode.Parse(jsonData);
                        this.questionData = jsonNode["Data"].ToString(); // Question json data;
                        this.accountData = jsonNode["account"].ToString(); // Account json data;
                        this.photoDataUrl = jsonNode["photo"].ToString(); // Account json data;

                        this.debugText.text += "Question Data: " + this.questionData + "\n\n ";
                        this.debugText.text += "Account: " + this.accountData + "\n\n ";
                        this.debugText.text += "Photo: " + this.photoDataUrl;

                        //Eg
                        Debug.Log(jsonNode["account"]["display_name"].ToString());
                        this.LoadQuestions();
                    }
                    else
                    {
                        Debug.LogError("JSON data not found in the response.");
                    }
                }
            }
        }

        if (!requestSuccessful)
        {
            Debug.LogError("Failed to get a successful response after " + maxRetries + " attempts.");
        }
    }

    public void LoadQuestions()
    {
        this.InitialGameSetup(()=>
        {
            LogController.Instance?.debug("currentURL: " + this.CurrentURL);
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
                                this.jwt = value;
                                LogController.Instance?.debug("Current jwt: " + this.jwt);
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


   /* string ParseURLParams(string url)
    {
        string[] urlParts = url.Split('?');
        string _unitKey = "";
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
                            case "jwt":
                                this.jwt = value;
                                LogController.Instance?.debug("Current jwt: " + this.jwt);
                                break;
                            case "unit":
                                _unitKey = value;
                                LogController.Instance?.debug("Current Game Unit: " + _unitKey);
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
        QuestionManager.Instance?.LoadQuestionFile(_unitKey, () => this.finishedLoadQuestion());
        return _unitKey;
    }*/

    public void changeScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

}

