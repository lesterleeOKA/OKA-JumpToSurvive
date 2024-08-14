using System;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance = null;
    [Header("<<<<<<<<<<<<<Load Question Json methods and Settings>>>>>>>>>>>>>")]
    public LoadMethod loadMethod = LoadMethod.UnityWebRequest;
    public string jsonFileName = "Question.json";
    [Space(10)]
    [Header("<<<<<<<<<<<<<Load Images methods and Settings>>>>>>>>>>>>>")]
    public LoadImage loadImage;
    [Space(10)]
    [Header("<<<<<<<<<<<<<Load Audio methods and Settings>>>>>>>>>>>>>>")]
    public LoadAudio loadAudio;
    [Space(10)]
    public QuestionData questionData;
    public int totalItems;
    public int loadedItems;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }


    public void LoadQuestionFile(string unitKey = "", Action onCompleted = null)
    {
        StartCoroutine(this.loadQuestionFile(unitKey, onCompleted));
    }


    IEnumerator loadQuestionFile(string unitKey = "", Action onCompleted = null)
    {
        var questionPath = Path.Combine(Application.streamingAssetsPath, this.jsonFileName);

        switch (this.loadMethod) 
        { 
            case LoadMethod.www:
                WWW www = new WWW(questionPath);
                yield return www;

                if (!string.IsNullOrEmpty(www.error))
                {
                    if (LogController.Instance != null) LogController.Instance.debugError($"Error loading question json: {www.error}");
                }
                else
                {
                    if (LogController.Instance != null) LogController.Instance.debug(questionPath);
                    var json = www.text;
                    this.questionData = JsonUtility.FromJson<QuestionData>(json);
                    if (!string.IsNullOrEmpty(unitKey)) { 
                        this.questionData.Data = this.questionData.Data.Where(q => q.QID != null && q.QID.StartsWith(unitKey)).ToList();
                    }

                    if (this.questionData.Data[0].QuestionType == "Picture" && this.loadImage.loadImageMethod == LoadImageMethod.AssetsBundle)
                    {
                        yield return this.loadImage.loadImageAssetBundleFile(this.questionData.Data[0].QID);
                    }

                    //if (LogController.Instance != null) LogController.Instance.debug($"loaded questions: {json}");
                    this.GetRandomQuestions(onCompleted);
                }
                break;
            case LoadMethod.UnityWebRequest:
                using (UnityWebRequest uwq = UnityWebRequest.Get(questionPath))
                {
                    yield return uwq.SendWebRequest();

                    if (uwq.result != UnityWebRequest.Result.Success)
                    {
                        if (LogController.Instance != null)
                            LogController.Instance.debugError($"Error loading question json: {uwq.error}");
                    }
                    else
                    {
                        if (LogController.Instance != null)
                            LogController.Instance.debug(questionPath);

                        var json = uwq.downloadHandler.text;
                        this.questionData = JsonUtility.FromJson<QuestionData>(json);
                        if (!string.IsNullOrEmpty(unitKey)) { 
                            this.questionData.Data = this.questionData.Data.Where(q => q.QID != null && q.QID.StartsWith(unitKey)).ToList();
                        }

                        if (this.questionData.Data[0].QuestionType == "Picture" && this.loadImage.loadImageMethod == LoadImageMethod.AssetsBundle)
                        {
                            yield return this.loadImage.loadImageAssetBundleFile(this.questionData.Data[0].QID);
                        }

                        if (LogController.Instance != null) { 
                            //LogController.Instance.debug($"loaded questions: {json}");
                            LogController.Instance.debug($"loaded filtered questions: {this.questionData.Data.Count}");
                        }
                        this.GetRandomQuestions(onCompleted);
                    }
                }
                break;
        }
    }
 
    void updateWebglLoadingBarStatus(string status ="")
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval($"updateLoadingText('{status}')");
#endif
    }

    private void ShuffleQuestions(Action onComplete = null)
    {
        this.questionData.Data.Sort((a, b) => UnityEngine.Random.Range(-1, 2));

        this.totalItems = this.questionData.Data.Count;
        this.loadedItems = 0;

        for (int i = 0; i < this.totalItems; i++)
        {
            var qa = this.questionData.Data[i];
            string folderName = qa.QuestionType;
            string qid = qa.QID;

            switch (qa.QuestionType)
            {
                case "Text":
                    if (i == 0) this.updateWebglLoadingBarStatus("Loading Question");
                    this.loadedItems++;
                    if (this.loadedItems == this.totalItems) onComplete?.Invoke();
                    break;
                case "Picture":
                    if (i == 0) this.updateWebglLoadingBarStatus("Loading Images");
                     StartCoroutine(
                        this.loadImage.Load(
                            folderName, qid, tex =>
                            {
                                qa.texture = tex;
                                this.loadedItems++;
                                if (this.loadedItems == this.totalItems) onComplete?.Invoke();
                            }
                         )
                      );
                    break;
                case "Audio":
                    if (i == 0) this.updateWebglLoadingBarStatus("Loading Audio");
                    StartCoroutine(
                        this.loadAudio.Load(
                            folderName, qid, (audio) =>
                            {
                                qa.audioClip = audio;
                                this.loadedItems++;
                                if (this.loadedItems == this.totalItems) onComplete?.Invoke();
                            }
                        )
                    );
                    break;
            }
        }
    }

    private void GetRandomQuestions(Action onCompleted = null)
    {
        if (this.questionData.Data.Count > 1 && this.questionData.Data[0] != this.questionData.Data[this.questionData.Data.Count - 1])
        {
            this.ShuffleQuestions(onCompleted);
        }
    }
}
