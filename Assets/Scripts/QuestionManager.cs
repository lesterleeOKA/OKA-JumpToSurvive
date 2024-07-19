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
    public ImageType imageType = ImageType.jpg;
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


    public enum ImageType
    {
        none,
        jpg,
        png
    }

    string GetExtension()
    {
        return this.imageType switch
        {
            ImageType.none => "",
            ImageType.jpg => ".jpg",
            ImageType.png => ".png",
            _ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, "Invalid image type.")
        };
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
                    if (!string.IsNullOrEmpty(unitKey)) this.questionData.Data = this.questionData.Data.Where(q => q.QID.StartsWith(unitKey)).ToList();
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
                        if (!string.IsNullOrEmpty(unitKey))
                            this.questionData.Data = this.questionData.Data.Where(q => q.QID.StartsWith(unitKey)).ToList();

                        this.GetRandomQuestions();
                    }
                }
                break;
        }


       
    }

    /*private Sprite LoadImageToSprite(string folderName = "", string fileName = "")
    {
        var imagePath = System.IO.Path.Combine(Application.streamingAssetsPath, folderName + "/" + fileName + this.GetExtension());
        WWW www = new WWW(imagePath);

        Sprite sprite = null;

        try
        {
            // Wait for the WWW request to complete
            while (!www.isDone) { }

            if (!string.IsNullOrEmpty(www.error))
            {
                if (LogController.Instance != null) LogController.Instance.debugError($"Error loading image: {www.error}");
            }
            else
            {
                if (LogController.Instance != null) LogController.Instance.debug(imagePath);

                Texture2D texture = www.texture;
                if (texture != null)
                {
                    sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
            }
        }
        catch (Exception e)
        {
            if (LogController.Instance != null)
            {
                LogController.Instance.debugError($"Exception loading image: {e.Message}");
            }
        }
        finally
        {
            www.Dispose();
        }

        return sprite;
    }
    */
    private IEnumerator LoadImageFromStreamingAssets(
    string folderName = "",
    string fileName = "",
    Action<Texture> callback = null)
    {

        var imagePath = System.IO.Path.Combine(Application.streamingAssetsPath, folderName + "/" + fileName + this.GetExtension());
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imagePath))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                if (texture != null)
                {
                    callback?.Invoke(texture);
                    Debug.Log($"Loaded Image : {texture.ToString()}");
                }
            }
            else
            {
                Debug.LogError($"Error loading image:{request.error}");
            }
        }

    }


    private IEnumerator LoadImageFromResources(string folderName = "", string fileName = "", Action<Sprite> callback = null)
    {
        //var imagePath = folderName + "/" + fileName + this.GetExtension();
        // Load the image from the "Resources" folder
        var imagePath = folderName + "/" + fileName;
        Sprite sprite = Resources.Load<Sprite>(imagePath);

        if (sprite != null)
        {
            // Use the loaded sprite
            Debug.Log("Image loaded successfully!");
            callback(sprite);
        }
        else
        {
            Debug.LogError($"Failed to load image from path: {imagePath}");
        }

        yield return null;
    }

    private IEnumerator LoadAudioFromResources(string folderName = "", string fileName = "", Action<AudioClip> callback = null)
    {
        var Path = folderName + "/" + fileName;
        AudioClip audioClip = Resources.Load<AudioClip>(Path);

        if (audioClip != null)
        {
            // Use the loaded sprite
            Debug.Log("Audio loaded successfully!");
            callback(audioClip);
        }
        else
        {
            Debug.LogError($"Failed to load Audio from path: {Path}");
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
                       LoadImageFromStreamingAssets(
                           "Picture", qid, tex =>
                           {
                               qa.texture = tex;
                           }
                        )
                     );
                    break;
                case "Audio":
                    StartCoroutine(
                        LoadAudioFromResources(
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
