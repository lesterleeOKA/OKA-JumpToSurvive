using UnityEngine;

public class LogController : MonoBehaviour
{
    public static LogController Instance = null;
    [Tooltip("The environment allow to show debug log")]
    public string showDebugEnv = "dev";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        Debug.Log("Current Environment--------------------------------------------------" +LoaderConfig.Instance?.currentHostName.ToString());
    }


    public void debug(string _message = "")
    {
        if(LoaderConfig.Instance != null)
        {
            if (this.showDebugEnv == LoaderConfig.Instance?.currentHostName.ToString())
            {
                Debug.Log(_message);
            }
        }
    }

    public void debugError(string _message = "")
    {
        if (LoaderConfig.Instance != null)
        {
            if (this.showDebugEnv == LoaderConfig.Instance?.currentHostName.ToString())
            {
                Debug.Log(_message);
            }
        }
    }
}
