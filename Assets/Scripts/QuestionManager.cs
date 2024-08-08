using System;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance = null;
    public string jsonFileName = "Question.json";
    public string UnitKey;
    public LoadMethod loadMethod = LoadMethod.UnityWebRequest;
    public LoadImageMethod loadImageMethod = LoadImageMethod.StreamingAssets;
    public ImageType imageType = ImageType.jpg;
    public LoadAudioMethod loadAudioMethod = LoadAudioMethod.StreamingAssets;
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

    public enum LoadImageMethod
    {
        Resources = 0,
        StreamingAssets = 1,
        AssetsBundle = 2
    }

    public enum LoadAudioMethod
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
       /* wav*/
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
                //AudioFormat.wav => ".wav",
                _ => throw new ArgumentOutOfRangeException(nameof(audioFormat), audioFormat, "Invalid audio type.")
            };
        }
    }

    public void LoadQuestionFile(string unitKey = "", Action onCompleted = null)
    {
        this.UnitKey = unitKey;
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

    private IEnumerator loadImage(string folderName = "", string fileName = "", Action<Texture> callback = null)
    {
        switch (this.loadImageMethod)
        {
            case LoadImageMethod.StreamingAssets: return this.LoadImageFromStreamingAssets(folderName, fileName, callback);
            case LoadImageMethod.Resources: return this.LoadImageFromResources(folderName, fileName, callback);
            case LoadImageMethod.AssetsBundle: return this.LoadImageFromAssetsBundle(folderName, fileName, callback);
            default: return this.LoadImageFromStreamingAssets(folderName, fileName, callback);
        }
    }

    private IEnumerator LoadImageFromStreamingAssets(
    string folderName = "",
    string fileName = "",
    Action<Texture> callback = null)
    {

        var imagePath =  Path.Combine(Application.streamingAssetsPath, folderName + "/" + fileName + this.ImageExtension);
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imagePath))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                if (texture != null)
                {
                    texture.filterMode = FilterMode.Bilinear;
                    texture.wrapMode = TextureWrapMode.Clamp;

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

    private AssetBundle assetBundle = null;
    private IEnumerator LoadImageFromAssetsBundle(string folderName = "", string fileName = "", Action<Texture> callback = null)
    {

        if (this.assetBundle == null)
        {
            var assetBundlePath = Path.Combine(Application.streamingAssetsPath, "picture." + this.UnitKey);
            this.assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
        }

        if (this.assetBundle != null)
        {
            Texture texture = assetBundle.LoadAsset<Texture>(fileName);

            if (texture != null)
            {
                // Use the loaded audio clip
                LogController.Instance?.debug(fileName + "loaded successfully!");
                callback(texture);
            }
            else
            {
                LogController.Instance?.debugError($"Failed to load Image asset: {fileName}");
            }
        }
        yield return null;
    }

    private IEnumerator loadAudio(string folderName = "", string fileName = "", Action<AudioClip> callback = null)
    {
        switch (this.loadAudioMethod)
        {
            case LoadAudioMethod.StreamingAssets: return this.LoadAudioFromStreamingAssets(folderName, fileName, callback);
            case LoadAudioMethod.Resources: return this.LoadAudioFromResources(folderName, fileName, callback);
            //case LoadFileMethod.AssetsBundle: return this.LoadAudioFromAssetsBundle(folderName, fileName, callback);
            default: return this.LoadAudioFromStreamingAssets(folderName, fileName, callback);
        }
    }

    private IEnumerator LoadAudioFromStreamingAssets(string folderName = "", string fileName = "", Action<AudioClip> callback = null)
    {
        string path = Path.Combine(Application.streamingAssetsPath, folderName, fileName + this.AudioExtension);

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
                using (UnityWebRequest uwq = UnityWebRequest.Get(path))
                {
                    yield return uwq.SendWebRequest();

                    if (uwq.result == UnityWebRequest.Result.Success)
                    {
                        byte[] results = uwq.downloadHandler.data;
                        var memStream = new MemoryStream(results);
                        var mpgFile = new NLayer.MpegFile(memStream);
                        var samples = new float[mpgFile.Length];
                        mpgFile.ReadSamples(samples, 0, (int)mpgFile.Length);

                        var audioClip = AudioClip.Create(fileName, samples.Length, mpgFile.Channels, mpgFile.SampleRate, false);
                        audioClip.SetData(samples, 0);

                        if (audioClip != null)
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
              
                /*using (UnityWebRequest uwq = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
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
                }*/
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

    /*private AssetBundle assetBundle = null;
    private IEnumerator LoadAudioFromAssetsBundle(string folderName = "", string fileName = "", Action<AudioClip> callback = null)
    {

        if (this.assetBundle == null)
        {
            var assetBundlePath = Path.Combine(Application.streamingAssetsPath, "audio." + this.UnitKey);
            this.assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
        }

        if (this.assetBundle != null)
        {
            AudioClip audioClip = assetBundle.LoadAsset<AudioClip>(fileName);

            if (audioClip != null && audioClip.length > 0)
            {
                // Use the loaded audio clip
                LogController.Instance?.debug(fileName + "loaded successfully!");
                callback(audioClip);
            }
            else
            {
                LogController.Instance?.debugError($"Failed to load Audio asset: {fileName}");
            }
        }

        yield return null;
    }*/


    private void ShuffleQuestions(Action onComplete = null)
    {
        this.questionData.Data.Sort((a, b) => UnityEngine.Random.Range(-1, 2));

        int totalItems = this.questionData.Data.Count;
        int loadedItems = 0;

        for (int i = 0; i < totalItems; i++)
        {
            var qa = this.questionData.Data[i];
            string folderName = qa.QuestionType;
            string qid = qa.QID;
            switch (qa.QuestionType)
            {
                case "Text":
                    loadedItems++;
                    if (loadedItems == totalItems) onComplete?.Invoke();
                    break;
                case "Picture":
                    StartCoroutine(
                       this.loadImage(
                           folderName, qid, tex =>
                           {
                               qa.texture = tex;
                               loadedItems++;
                               if (loadedItems == totalItems) onComplete?.Invoke();
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
                                loadedItems++;
                                if (loadedItems == totalItems) onComplete?.Invoke();
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
