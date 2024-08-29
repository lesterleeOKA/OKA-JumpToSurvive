using System;

[Serializable]
public class Downloader
{
    public LoadMethod loadMethod = LoadMethod.UnityWebRequest;
}

public enum LoadMethod
{
    www = 0,
    UnityWebRequest = 1,
    API = 2,
}
