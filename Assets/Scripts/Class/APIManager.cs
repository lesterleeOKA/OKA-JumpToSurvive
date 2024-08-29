using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

[Serializable]
public class APIManager
{
    [Tooltip("Account Jwt token, upload data jwt")]
    public string jwt;
    [Tooltip("Game Setting Json")]
    public string gameSettingJson = string.Empty;
    [Tooltip("Question Json")]
    public string questionJson = string.Empty;
    [Tooltip("Account Json")]
    public string accountJson = string.Empty;
    [Tooltip("Account Icon Image Url")]
    public string photoDataUrl = string.Empty;
    public LoadImage loadPeopleIcon;
    public Texture  peopleIcon;
    public string loginName = string.Empty;
    public int maxRetries = 10;
    public CanvasGroup debugLayer;
    private Text debugText = null;
    private bool showingDebugBox = false;

    public void Init()
    {
        if (this.debugLayer != null)
        {
            this.debugText = this.debugLayer.GetComponentInChildren<Text>();
        }
    }

    public void controlDebugLayer()
    {
        if (this.debugLayer != null && Input.GetKeyDown("d"))
        {
            showingDebugBox = !showingDebugBox;
            SetUI.Set(this.debugLayer.GetComponent<CanvasGroup>(), showingDebugBox, 0f);
        }
    }

    public IEnumerator PostGameSetting(Action getParseURLParams=null, Action onCompleted = null)
    {
        ExternalCaller.UpdateLoadBarStatus("Loading Data");
        getParseURLParams?.Invoke();
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
                    LogController.Instance?.debugError("Error: " + www.error);
                    retryCount++;
                    LogController.Instance?.debug("Retrying... Attempt " + retryCount);
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
                        LogController.Instance?.debug("Response: " + jsonData);

                        var jsonNode = JSONNode.Parse(jsonData);
                        this.questionJson = jsonNode[APIConstant.QuestionDataHeaderName].ToString(); // Question json data;
                        this.accountJson = jsonNode["account"].ToString(); // Account json data;
                        this.photoDataUrl = jsonNode["photo"].ToString(); // Account json data;

                        this.debugText.text += "Question Data: " + this.questionJson + "\n\n ";
                        this.debugText.text += "Account: " + this.accountJson + "\n\n ";
                        this.debugText.text += "Photo: " + this.photoDataUrl;

                        if (!string.IsNullOrEmpty(this.photoDataUrl))
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
                        LogController.Instance?.debugError("JSON data not found in the response.");
                    }
                }
            }
        }

        if (!requestSuccessful)
        {
            LogController.Instance?.debugError("Failed to get a successful response after " + maxRetries + " attempts.");
        }
    }
}
