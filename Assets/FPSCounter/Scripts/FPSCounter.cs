using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    private CanvasGroup fpsCg;
    private Text fpsText;
    public float updateInterval = 0.2f; //How often should the number update
    float time = 0.0f;
    int frames = 0;

    private void Start()
    {
        this.fpsCg = GetComponent<CanvasGroup>();
        this.fpsText = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        bool enableFPSMonitor = LoaderConfig.Instance.ShowFPS || Debug.isDebugBuild;

        if (this.fpsCg != null)
        {
            this.fpsCg.alpha = enableFPSMonitor ? 1f : 0f;
        }

        if(this.fpsText != null && enableFPSMonitor)
        {
            time += Time.unscaledDeltaTime;
            ++frames;

            // Interval ended - update GUI text and start new interval
            if (time >= updateInterval)
            {
                float fps = (int)(frames / time);
                time = 0.0f;
                frames = 0;

                this.fpsText.text = "FPS: " + fps.ToString();
            }
        }
    }
}

