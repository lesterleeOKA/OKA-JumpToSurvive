using System;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

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
        switch (this.loadMethod)
        {
            case LoadMethod.www:
            case LoadMethod.UnityWebRequest:
                StartCoroutine(this.loadQuestionFile(unitKey, onCompleted));
                break;
            case LoadMethod.API:
                var questionJson = LoaderConfig.Instance.apiManager.questionJson;
                if (!string.IsNullOrEmpty(questionJson)) { 
                    QuestionDataWrapper wrapper = JsonUtility.FromJson<QuestionDataWrapper>("{\"QuestionDataArray\":" + questionJson + "}");
                    QuestionData _questionData = new QuestionData
                    {
                        Data = new List<QuestionList>(wrapper.QuestionDataArray)
                    };
                    LogController.Instance?.debug("Load Question from API");
                    this.loadQuestionFromAPI(_questionData, onCompleted);
                }
                break;
        }
    }

    private void loadQuestionFromAPI(QuestionData _questionData = null, Action onCompleted = null)
    {
        if(_questionData != null)
        {
            this.questionData = _questionData;
            LogController.Instance?.debug($"loaded api questions: {this.questionData.Data.Count}");
            this.GetRandomQuestions(onCompleted);
        }
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
                    LogController.Instance?.debugError($"Error loading question json: {www.error}");
                }
                else
                {
                    LogController.Instance?.debug(questionPath);
                    var json = www.text;
                    this.questionData = JsonUtility.FromJson<QuestionData>(json);
                    if (!string.IsNullOrEmpty(unitKey)) { 
                        this.questionData.Data = this.questionData.Data.Where(q => q.QID != null && q.QID.StartsWith(unitKey)).ToList();
                    }

                    if (this.questionData.Data[0].QuestionType == "Picture" && this.loadImage.loadImageMethod == LoadImageMethod.AssetsBundle)
                    {
                        yield return this.loadImage.loadImageAssetBundleFile(this.questionData.Data[0].QID);
                    }

                    LogController.Instance?.debug($"loaded filtered questions: {this.questionData.Data.Count}");
                    this.GetRandomQuestions(onCompleted);
                }
                break;
            case LoadMethod.UnityWebRequest:
                using (UnityWebRequest uwq = UnityWebRequest.Get(questionPath))
                {
                    yield return uwq.SendWebRequest();

                    if (uwq.result != UnityWebRequest.Result.Success)
                    {
                        LogController.Instance?.debugError($"Error loading question json: {uwq.error}");
                    }
                    else
                    {
                        LogController.Instance?.debug(questionPath);
                        var json = uwq.downloadHandler.text;
                        this.questionData = JsonUtility.FromJson<QuestionData>(json);
                        if (!string.IsNullOrEmpty(unitKey)) { 
                            this.questionData.Data = this.questionData.Data.Where(q => q.QID != null && q.QID.StartsWith(unitKey)).ToList();
                        }

                        if (this.questionData.Data[0].QuestionType == "Picture" && this.loadImage.loadImageMethod == LoadImageMethod.AssetsBundle)
                        {
                            yield return this.loadImage.loadImageAssetBundleFile(this.questionData.Data[0].QID);
                        }

                        //LogController.Instance.debug($"loaded questions: {json}");
                        LogController.Instance?.debug($"loaded filtered questions: {this.questionData.Data.Count}");
                        this.GetRandomQuestions(onCompleted);
                    }
                }
                break;
        }
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
                    ExternalCaller.UpdateLoadBarStatus("Loading Question");
                    this.loadedItems++;
                    if (this.loadedItems == this.totalItems) onComplete?.Invoke();
                    break;
                case "Picture":
                    ExternalCaller.UpdateLoadBarStatus("Loading Images");
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
                    ExternalCaller.UpdateLoadBarStatus("Loading Audio");
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
