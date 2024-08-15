using UnityEngine;
using UnityEngine.UI;

public class AudioControl : MonoBehaviour
{   
    public AudioOnOff audioOnOffPanel;
    public Image muteBtn;
    public Sprite[] audioSprites;
    private AudioListener audioListener;
    // Start is called before the first frame update
    void Start()
    {
        if (this.audioOnOffPanel != null) this.audioOnOffPanel.Init(false);
        this.setMutePanel(false);

        if(AudioController.Instance != null)
        {
            if (this.muteBtn != null && this.audioSprites[AudioController.Instance.audioStatus ? 0 : 1] != null)
                this.muteBtn.sprite = this.audioSprites[AudioController.Instance.audioStatus ? 0 : 1];
        }
    }

    public void setAudioStatus(bool status)
    {
        this.setMutePanel(false);
        if (this.audioOnOffPanel != null) this.audioOnOffPanel.set(status);
        if (this.muteBtn != null && this.audioSprites[status ? 0:1] != null)
            this.muteBtn.sprite = this.audioSprites[status ? 0 : 1];
    }

    public void setMutePanel(bool status)
    {
        if (this.audioOnOffPanel != null && !this.audioOnOffPanel.isAnimated)
        {
            if (status)
            {
                this.audioOnOffPanel.setAnimated(status, ()=> this.gameStatus(status));
            }
            else
            {
                this.gameStatus(status);
                this.audioOnOffPanel.setAnimated(status, null);
            }
        }
    }

    public void playBtnClick()
    {
        AudioController.Instance?.PlayAudio(0);
    }

    void gameStatus(bool status)
    {
        Time.timeScale = status ? 0f : 1f;
    }
}
