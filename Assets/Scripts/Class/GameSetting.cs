using UnityEngine;

public class GameSetting : MonoBehaviour
{

    protected virtual void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        DontDestroyOnLoad(this);
    }


}
