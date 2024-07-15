using UnityEngine;

public class LogController : MonoBehaviour
{
    public static LogController Instance = null;
    public string env = "dev";
    public string[] environments;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }


    public void debug(string _message = "")
    {
        if(LoaderConfig.Instance != null)
        {
            for (int i = 0; i < environments.Length; i++)
            {
                if (environments[i] != null)
                {
                    if (environments[i] == this.env)
                    {
                        Debug.Log(_message);
                    }
                }
            }
        }
    }

    public void debugError(string _message = "")
    {
        if (LoaderConfig.Instance != null)
        {
            for (int i = 0; i < environments.Length; i++)
            {
                if (environments[i] != null)
                {
                    if (environments[i] == this.env)
                    {
                        Debug.LogError(_message);
                    }
                }
            }
        }
    }
}
