using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

[Serializable]
public class LoadImage: Downloader
{
    [SerializeField] public LoadImageMethod loadImageMethod = LoadImageMethod.StreamingAssets;
    [SerializeField] private ImageType imageType = ImageType.jpg;
    private AssetBundle assetBundle = null;
    [HideInInspector]
    public Texture[] allTextures;

    public string ImageExtension
    {
        get
        {
            return this.imageType switch
            {
                ImageType.none => "",
                ImageType.jpg => ".jpg",
                ImageType.png => ".png",
                _ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, "Invalid image type.")
            };
        }
    }

    public IEnumerator Load(string folderName = "", string fileName = "", Action<Texture> callback = null)
    {
        switch (this.loadImageMethod)
        {
            case LoadImageMethod.StreamingAssets: return this.LoadImageFromStreamingAssets(folderName, fileName, callback);
            case LoadImageMethod.Resources: return this.LoadImageFromResources(folderName, fileName, callback);
            case LoadImageMethod.AssetsBundle: return this.LoadImageFromAssetsBundle(fileName, callback);
            default: return this.LoadImageFromStreamingAssets(folderName, fileName, callback);
        }
    }

    private IEnumerator LoadImageFromStreamingAssets(
    string folderName = "",
    string fileName = "",
    Action<Texture> callback = null)
    {

        var imagePath = Path.Combine(Application.streamingAssetsPath, folderName + "/" + fileName + this.ImageExtension);

        switch (this.loadMethod)
        {
            case LoadMethod.www:
                WWW www = new WWW(imagePath);
                yield return www;

                if (string.IsNullOrEmpty(www.error))
                {
                    Texture2D texture = www.texture;
                    if (texture != null)
                    {
                        texture.filterMode = FilterMode.Bilinear;
                        texture.wrapMode = TextureWrapMode.Clamp;

                        callback?.Invoke(texture);
                        if (LogController.Instance != null)
                            LogController.Instance.debug($"Loaded Image : {fileName}");
                    }
                }
                else
                {
                    if (LogController.Instance != null)
                        LogController.Instance.debugError($"Error loading image:{www.error}");
                }
                break;
            case LoadMethod.UnityWebRequest:
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
                                LogController.Instance.debug($"Loaded Image : {fileName}");
                        }
                    }
                    else
                    {
                        if (LogController.Instance != null)
                            LogController.Instance.debugError($"Error loading image:{request.error}");
                    }
                }
                break;
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


    public IEnumerator loadImageAssetBundleFile(string fileName = "")
    {
        if (this.assetBundle == null)
        {

            string unitKey = Regex.Replace(fileName, @"-c\d+", "-c");
            var assetBundlePath = Path.Combine(Application.streamingAssetsPath, "picture." + unitKey);

#if UNITY_WEBGL && !UNITY_EDITOR
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    this.assetBundle = DownloadHandlerAssetBundle.GetContent(request);
                    this.allTextures = this.assetBundle.LoadAllAssets<Texture>();
                    LogController.Instance?.debug($"downloaded AssetBundle: {this.assetBundle}");
                }
                else
                {
                    LogController.Instance?.debugError($"Failed to download AssetBundle: {request.error}");
                    yield break; // Exit if the download failed
                }
            }
#else
            var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
            yield return assetBundleCreateRequest;
            this.assetBundle = assetBundleCreateRequest.assetBundle;
            this.allTextures = this.assetBundle.LoadAllAssets<Texture>();
#endif
        }
    }

    private IEnumerator LoadImageFromAssetsBundle(string fileName = "", Action<Texture> callback = null)
    {
        if (this.assetBundle != null)
        {
            //Texture texture = assetBundle.LoadAsset<Texture>(fileName);
            Texture texture = Array.Find(this.allTextures, t => t.name == fileName);

            if (texture != null)
            {
                LogController.Instance?.debug(fileName + " loaded successfully!");
                callback(texture);
            }
            else
            {
                LogController.Instance?.debugError($"Failed to load Image asset: {fileName}");
            }

            yield return null;
        }
    }


    /*private async void loadImage(string folderName = "", string fileName = "", Action<Texture> callback = null)
{
    switch (this.loadImageMethod)
    {
        case LoadImageMethod.StreamingAssets: 
            await this.LoadImageFromStreamingAssetsAsync(folderName, fileName, callback);
            break;
        default:
            await this.LoadImageFromStreamingAssetsAsync(folderName, fileName, callback);
            break;
    }
}

private async Task LoadImageFromStreamingAssetsAsync(
string folderName = "",
string fileName = "",
Action<Texture> callback = null)
{
    var imagePath = Path.Combine(Application.streamingAssetsPath, folderName, fileName + this.ImageExtension);

    using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imagePath))
    {
        var operation = request.SendWebRequest();

        // Create a TaskCompletionSource to await the operation
        var taskCompletionSource = new TaskCompletionSource<bool>();

        operation.completed += (op) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                taskCompletionSource.SetResult(true);
            }
            else
            {
                taskCompletionSource.SetException(new Exception(request.error));
            }
        };

        // Await the task
        await taskCompletionSource.Task;

        // Handle the response
        Texture2D texture = DownloadHandlerTexture.GetContent(request);
        if (texture != null)
        {
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            callback?.Invoke(texture);
            LogController.Instance?.debug($"Loaded Image: {fileName}");
        }
    }
}*/
}


public enum ImageType
{
    none,
    jpg,
    png
}
public enum LoadImageMethod
{
    Resources = 0,
    StreamingAssets = 1,
    AssetsBundle = 2
}