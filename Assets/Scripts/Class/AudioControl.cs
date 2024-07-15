using UnityEngine;
using UnityEngine.UI;

public class AudioControl : MonoBehaviour
{   
    public AudioListener sceneListener;
    public CanvasGroup mutePanel;
    public Image muteBtn;
    public Sprite[] audioSprites;
    // Start is called before the first frame update
    void Start()
    {
        if(this.sceneListener != null && LoaderConfig.Instance != null)
        {
            this.sceneListener.enabled = LoaderConfig.Instance.audioStatus ? true : false;
        }

        this.setMutPanel(false);

        if (this.muteBtn != null && this.audioSprites[0] != null) 
            this.muteBtn.sprite = this.audioSprites[0];
    }

    public void setAudioStatus(bool status)
    {
        if (this.sceneListener != null && LoaderConfig.Instance != null)
        {
            LoaderConfig.Instance.audioStatus = !status;
            this.sceneListener.enabled = LoaderConfig.Instance.audioStatus ? true : false;
        }

        this.setMutPanel(false);

        if (this.muteBtn != null && this.audioSprites[status ? 0:1] != null)
            this.muteBtn.sprite = this.audioSprites[status ? 0 : 1];
    }


    public void setMutPanel(bool status)
    {
        SetUI.Set(this.mutePanel, status, status ? 0f : 0f);
        Time.timeScale = status? 0f : 1f;
    }
}
