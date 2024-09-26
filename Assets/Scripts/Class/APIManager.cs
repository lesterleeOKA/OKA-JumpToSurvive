using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro;

[Serializable]
public class APIManager
{
    [Tooltip("Account Jwt token, upload data jwt")]
    public string jwt;
    [Tooltip("Created App/Book current id")]
    public string appId;
    [Tooltip("Game Setting Json")]
    public string gameSettingJson = string.Empty;
    [Tooltip("Question Json")]
    public string questionJson = string.Empty;
    [Tooltip("Account Json")]
    public string accountJson = string.Empty;
    [Tooltip("Account Icon Image Url")]
    public string photoDataUrl = string.Empty;
    [Tooltip("Payloads Object for data send out")]
    public string payloads = string.Empty;
    public LoadImage loadPeopleIcon;
    public Texture  peopleIcon;
    public string loginName = string.Empty;
    public string instructionContent = string.Empty;
    public int maxRetries = 10;
    public CanvasGroup debugLayer;
    public CanvasGroup loginErrorBox;
    public TextMeshProUGUI loginErrorMessage;
    private Text debugText = null;
    private string errorMessage = "";
    public bool isShowLoginErrorBox = false;
    private bool showingDebugBox = false;

    public void Init()
    {
        if (this.debugLayer != null)
        {
            this.debugText = this.debugLayer.GetComponentInChildren<Text>();
        }
        this.resetLoginErrorBox();
    }

    public void controlDebugLayer()
    {
        if (this.debugLayer != null && Input.GetKeyDown("d"))
        {
            showingDebugBox = !showingDebugBox;
            SetUI.Set(this.debugLayer.GetComponent<CanvasGroup>(), showingDebugBox, 0f);
        }
        this.checkLoginErrorBox();
    }

    public bool IsShowLoginErrorBox
    {
        set { this.isShowLoginErrorBox = value; }
        get { return this.isShowLoginErrorBox; }
    }

    public void resetLoginErrorBox()
    {
        this.IsShowLoginErrorBox = false;
        SetUI.Set(this.loginErrorBox, false, 0f);
    }

    public void checkLoginErrorBox()
    {
        SetUI.Set(this.loginErrorBox, this.IsShowLoginErrorBox, 0f);
    }

    public IEnumerator PostGameSetting(Action getParseURLParams=null, Action onCompleted = null)
    {
        ExternalCaller.UpdateLoadBarStatus("Loading Data");
        getParseURLParams?.Invoke();
        string api = APIConstant.GameDataAPI(this.appId, this.jwt);
        LogController.Instance?.debug("called login api: " + api);
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
                www.certificateHandler = new WebRequestSkipCert();
                // Send the request and wait for a response
                yield return www.SendWebRequest();

                // Check for errors
                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.errorMessage = "Error: " + www.error + "Retrying..." + retryCount;
                    this.IsShowLoginErrorBox = true;
                    retryCount++;
                    LogController.Instance?.debug(this.errorMessage);
                    yield return new WaitForSeconds(2); // Wait for 2 seconds before retrying
                }
                else
                {
                    requestSuccessful = true;
                    string responseText = www.downloadHandler.text;
                    int jsonStartIndex = responseText.IndexOf("{\"questions\":");
                    LogController.Instance?.debug("www.downloadHandler.text: " + responseText);

                    if (jsonStartIndex != -1)
                    {
                        string jsonData = responseText.Substring(jsonStartIndex);
                        LogController.Instance?.debug("Response: " + jsonData);

                        var jsonNode = JSONNode.Parse(jsonData);
                        this.questionJson = jsonNode[APIConstant.QuestionDataHeaderName].ToString(); // Question json data;
                        this.accountJson = jsonNode["account"].ToString(); // Account json data;
                        this.photoDataUrl = jsonNode["photo"].ToString(); // Account json data;
                        this.payloads = jsonNode["payloads"].ToString();

                        if (this.debugText != null)
                        {
                            this.debugText.text += "Question Data: " + this.questionJson + "\n\n ";
                            this.debugText.text += "Account: " + this.accountJson + "\n\n ";
                            this.debugText.text += "Photo: " + this.photoDataUrl + "\n\n ";
                            this.debugText.text += "PayLoad: " + this.payloads;
                        }

                        if (!string.IsNullOrEmpty(this.photoDataUrl) && this.photoDataUrl != "null")
                        {
                            string modifiedPhotoDataUrl = photoDataUrl.Replace("\"", "");

                            string imageUrl = modifiedPhotoDataUrl;
                            if (!modifiedPhotoDataUrl.StartsWith("https://"))
                            {
                                imageUrl = "https:" + modifiedPhotoDataUrl;
                            }
                            LogController.Instance?.debug($"Downloading People Icon!!{imageUrl}");
                            yield return this.loadPeopleIcon.Load("", imageUrl, _peopleIcon =>
                            {
                                LogController.Instance?.debug($"Downloaded People Icon!!");
                                this.peopleIcon = _peopleIcon;
                            });
                        }

                        if (jsonNode["account"] != null && !string.IsNullOrEmpty(this.accountJson))
                        {
                            var name = jsonNode["account"]["display_name"].ToString();
                            this.loginName = name.Replace("\"", "");
                        }

                        //E.g
                        //Debug.Log(jsonNode["account"]["display_name"].ToString());
                        LogController.Instance?.debug(this.questionJson);
                        onCompleted?.Invoke();
                    }
                    else
                    {
                        this.errorMessage = "JSON data not found in the response.";
                        LogController.Instance?.debug(this.errorMessage);
                        this.IsShowLoginErrorBox = true;
                        onCompleted?.Invoke();
                    }
                }
            }
        }

        if (!requestSuccessful)
        {
            this.errorMessage = "Failed to get a successful response after " + maxRetries + " attempts.";
            LogController.Instance?.debug(this.errorMessage);
            this.IsShowLoginErrorBox = true;
            onCompleted?.Invoke();
        }
    }
}
