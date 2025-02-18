using System;
using UnityEngine;

public static class ExternalCaller
{
    public static string GetCurrentDomainName
    {
        get
        {
            string absoluteUrl = Application.absoluteURL;
            Uri url = new Uri(absoluteUrl);
            if (LogController.Instance != null) LogController.Instance.debug("Host Name:" + url.Host);
            return url.Host;
        }
    }

    public static void ReLoadCurrentPage()
    {
#if !UNITY_EDITOR
        Application.ExternalEval("location.reload();");
#else
        LoaderConfig.Instance?.changeScene(1);
#endif
    }

    public static void BackToHomeUrlPage(bool isLogined = false)
    {
#if !UNITY_EDITOR
        if (isLogined)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Application.ExternalEval("history.back();");
            }
            else
            {
                Application.ExternalEval($"location.hash = 'exit'");
            }
        }
        else
        {
            string hostname = GetCurrentDomainName;
            if (hostname.Contains("dev.openknowledge.hk"))
            {
                string baseUrl = GetCurrentDomainName;
                string newUrl = $"https://{baseUrl}/RainbowOne/webapp/OKAGames/SelectGames/";
                if (LogController.Instance != null) LogController.Instance.debug("full url:" + newUrl);
                //Application.ExternalEval($"location.href = '{newUrl}', '_self'");
                Application.ExternalEval($"window.location.replace('{newUrl}')");
            }
            else if (hostname.Contains("www.rainbowone.app"))
            {
                string Production = "https://www.starwishparty.com/";
                //Application.ExternalEval($"location.href = '{Production}', '_self'");
                Application.ExternalEval($"window.location.replace('{Production}')");
            }
            else if (hostname.Contains("localhost"))
            {
                LoaderConfig.Instance?.changeScene(1);
            }
            else
            {
                Application.ExternalEval($"location.hash = 'exit'");
            }
        }   
#else
        LoaderConfig.Instance?.changeScene(1);
#endif
    }

    public static void HiddenLoadingBar()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval("hiddenLoadingBar()");     
#endif
    }

    public static void UpdateLoadBarStatus(string status = "")
    {
        LogController.Instance?.debug(status);
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval($"updateLoadingText('{status}')");
#endif
    }
}

