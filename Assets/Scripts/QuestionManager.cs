using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance = null;
    public string jsonFileName = "Question.json";
    public LoadMethod loadMethod = LoadMethod.UnityWebRequest;
    public LoadFileMethod loadFileMethod = LoadFileMethod.StreamingAssets;
    public ImageType imageType = ImageType.jpg;
    public AudioFormat audioFormat = AudioFormat.mp3;
    public QuestionData questionData;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public enum LoadMethod 
    { 
        www=0,
        UnityWebRequest=1,
    }

    public enum LoadFileMethod
    {
        Resources = 0,
        StreamingAssets = 1,
    }

    public enum ImageType
    {
        none,
        jpg,
        png
    }

    public enum AudioFormat
    {
        none,
        mp3,
        wav
    }

    string ImageExtension
    {
        get { 
            return this.imageType switch
            {
                ImageType.none => "",
                ImageType.jpg => ".jpg",
                ImageType.png => ".png",
                _ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, "Invalid image type.")
            };
        }
    }

    string AudioExtension
    {
        get
        {
            return this.audioFormat switch
            {
                AudioFormat.none => "",
                AudioFormat.mp3 => ".mp3",
                AudioFormat.wav => ".wav",
                _ => throw new ArgumentOutOfRangeException(nameof(audioFormat), audioFormat, "Invalid audio type.")
            };
        }
    }

    public void LoadQuestionFile(string unitKey = "")
    {
        StartCoroutine(this.loadQuestionFile(unitKey));
    }


    IEnumerator loadQuestionFile(string unitKey = "")
    {
        var questionPath = System.IO.Path.Combine(Application.streamingAssetsPath, this.jsonFileName);


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

                    if (LogController.Instance != null) LogController.Instance.debug($"loaded questions: {json}");
                    this.GetRandomQuestions();
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

                        if (LogController.Instance != null) { 
                            LogController.Instance.debug($"loaded questions: {json}");
                            LogController.Instance.debug($"loaded filtered questions: {this.questionData.Data.Count}");
                        }
                        this.GetRandomQuestions();
                    }
                }
                break;
        }       
    }

    private IEnumerator loadImage(string folderName = "", string fileName = "", Action<Texture> callback = null)
    {
        switch (this.loadFileMethod)
        {
            case LoadFileMethod.StreamingAssets: return this.LoadImageFromStreamingAssets(folderName, fileName, callback);
            case LoadFileMethod.Resources: return this.LoadImageFromResources(folderName, fileName, callback);
            default: return this.LoadImageFromStreamingAssets(folderName, fileName, callback);
        }
    }



    private IEnumerator LoadImageFromStreamingAssets(
    string folderName = "",
    string fileName = "",
    Action<Texture> callback = null)
    {

        var imagePath = System.IO.Path.Combine(Application.streamingAssetsPath, folderName + "/" + fileName + this.ImageExtension);
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imagePath))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                if (texture != null)
                {
                    callback?.Invoke(texture);
                    if (LogController.Instance != null) 
                        LogController.Instance.debug($"Loaded Image : {texture.ToString()}");
                }
            }
            else
            {
                if (LogController.Instance != null)
                    LogController.Instance.debugError($"Error loading image:{request.error}");
            }
        }

    }


    private IEnumerator LoadImageFromResources(string folderName = "", string fileName = "", Action<Texture> callback = null)
    {
        // Load the image from the "Resources" folder
        var imagePath = folderName + "/" + fileName;
        Texture texture = Resources.Load<Texture>(imagePath);

        if (texture != null)
        {
            // Use the loaded sprite
            Debug.Log("Image loaded successfully!");
            callback(texture);
        }
        else
        {
            Debug.LogError($"Failed to load image from path: {imagePath}");
        }

        yield return null;
    }

    private IEnumerator loadAudio(string folderName = "", string fileName = "", Action<AudioClip> callback = null)
    {
        switch (this.loadFileMethod)
        {
            case LoadFileMethod.StreamingAssets: return this.LoadAudioFromStreamingAssets(folderName, fileName, callback);
            case LoadFileMethod.Resources: return this.LoadAudioFromResources(folderName, fileName, callback);
            default: return this.LoadAudioFromStreamingAssets(folderName, fileName, callback);
        }
    }

    private IEnumerator LoadAudioFromStreamingAssets(string folderName = "", string fileName = "", Action<AudioClip> callback = null)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, folderName, fileName + this.AudioExtension);

        switch (this.loadMethod)
        {
            case LoadMethod.www:
                using (WWW www = new WWW(path))
                {
                    yield return www;

                    if (string.IsNullOrEmpty(www.error))
                    {
                        AudioClip audioClip = www.GetAudioClip();

                        if (audioClip != null && audioClip.length > 0)
                        {
                            LogController.Instance?.debug("Audio loaded successfully!");
                            callback?.Invoke(audioClip);
                        }
                        else
                        {
                            LogController.Instance?.debugError($"Failed to load Audio from path: {path}, Error: Audio clip is null or empty");
                        }
                    }
                    else
                    {
                        LogController.Instance?.debugError($"Failed to load Audio from path: {path}, Error: {www.error}");
                    }
                }
                break;
            case LoadMethod.UnityWebRequest:
                using (UnityWebRequest uwq = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
                {
                    yield return uwq.SendWebRequest();

                    if (uwq.result == UnityWebRequest.Result.Success)
                    {
                        AudioClip audioClip = DownloadHandlerAudioClip.GetContent(uwq);

                        if (audioClip != null && audioClip.length > 0)
                        {
                            LogController.Instance?.debug("Audio loaded successfully!");
                            callback?.Invoke(audioClip);
                        }
                    }
                    else
                    {
                        LogController.Instance?.debugError($"Failed to load Audio from path: {path}, Error: {uwq.error}");
                    }
                }
                break;
        }   
    }


    private IEnumerator LoadAudioFromResources(string folderName = "", string fileName = "", Action<AudioClip> callback = null)
    {
        var Path = folderName + "/" + fileName;

        AudioClip audioClip = Resources.Load<AudioClip>(Path);

        if (audioClip != null && audioClip.length > 0)
        {
            // Use the loaded sprite
            LogController.Instance?.debug("Audio loaded successfully!");
            callback(audioClip);
        }
        else
        {
            LogController.Instance?.debugError($"Failed to load Audio from path: {Path}");
        }

        yield return null;
    }


    private void ShuffleQuestions()
    {
        this.questionData.Data.Sort((a, b) => UnityEngine.Random.Range(-1, 2));

        for (int i = 0; i < this.questionData.Data.Count; i++)
        {
            var qa = this.questionData.Data[i];
            string folderName = qa.QuestionType;
            string qid = qa.QID;
            switch (qa.QuestionType)
            {
                case "Text":
                    break;
                case "Picture":
                    StartCoroutine(
                       this.loadImage(
                           folderName, qid, tex =>
                           {
                               qa.texture = tex;
                           }
                        )
                     );
                    break;
                case "Audio":
                    StartCoroutine(
                        this.loadAudio(
                            folderName, qid, (audio) =>
                            {
                                qa.audioClip = audio;
                            }
                        )
                    );
                    break;
            }

        }
    }

    private void GetRandomQuestions()
    {
        if (this.questionData.Data.Count > 1 && this.questionData.Data[0] != this.questionData.Data[this.questionData.Data.Count - 1])
        {
            this.ShuffleQuestions();
        }
    }
}
