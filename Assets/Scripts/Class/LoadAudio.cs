using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class LoadAudio: Downloader
{
    [SerializeField] private LoadAudioMethod loadAudioMethod = LoadAudioMethod.StreamingAssets;
    [SerializeField] private AudioFormat audioFormat = AudioFormat.mp3;

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

    public IEnumerator Load(string folderName = "", string fileName = "", Action<AudioClip> callback = null)
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
                    uwq.certificateHandler = new WebRequestSkipCert();
                    yield return uwq.SendWebRequest();

                    if (uwq.result == UnityWebRequest.Result.Success)
                    {
                        byte[] results = uwq.downloadHandler.data;
                        if (results != null && results.Length > 0)
                        {
                            using (var memStream = new MemoryStream(results))
                            {
                                var mpgFile = new NLayer.MpegFile(memStream);
                                var samples = new float[mpgFile.Length];

                                // Read samples in chunks if necessary for large files
                                mpgFile.ReadSamples(samples, 0, (int)mpgFile.Length);

                                var audioClip = AudioClip.Create(fileName, samples.Length, mpgFile.Channels, mpgFile.SampleRate, false);
                                audioClip.SetData(samples, 0);

                                if (audioClip != null)
                                {
                                    LogController.Instance?.debug("Audio loaded successfully!");
                                    callback?.Invoke(audioClip);
                                }
                            }
                        }
                        else
                        {
                            LogController.Instance?.debugError("Downloaded data is empty.");
                        }
                    }
                    else
                    {
                        LogController.Instance?.debugError($"Failed to load Audio from path: {path}, Error: {uwq.error}");
                    }
                }

                /*using (UnityWebRequest uwq = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
                {
                    uwq.certificateHandler = new WebRequestSkipCert();
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
}

public enum LoadAudioMethod
{
    Resources = 0,
    StreamingAssets = 1,
}

public enum AudioFormat
{
    none,
    mp3,
    /* wav*/
}
