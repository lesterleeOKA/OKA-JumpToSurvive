using UnityEngine;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup gameStartPanel;
    public AudioOnOff audioOnOffPanel;

    private void Start()
    {
        Time.timeScale = 1f;
        this.audioOnOffPanel.Init(true);
        SetUI.Set(this.gameStartPanel, false, 0f);
        if (this.gameStartPanel != null) this.gameStartPanel.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0f);
    }

    public void MusicOnbutton()
    {
        this.audioOnOffPanel.set(true);
        this.audioOnOffPanel.setPanel(false);
        SetUI.Set(this.gameStartPanel, true, 1f);
        if (this.gameStartPanel != null) gameStartPanel.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
    }
    public void MusicOffbutton()
    {
        this.audioOnOffPanel.set(false);
        this.audioOnOffPanel.setPanel(false);
        SetUI.Set(this.gameStartPanel, true, 1f);
        if (this.gameStartPanel != null) gameStartPanel.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
    }

    public void StartGame()
    {
        if (AudioController.Instance != null) AudioController.Instance.PlayAudio(0);
        SetUI.Set(this.gameStartPanel, false, 0.5f);
        this.gameStart();
        /*if (this.gameStartPanel != null) { 
            this.gameStartPanel.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f).OnComplete(()=> this.gameStart());
        }*/
    }

    void gameStart()
    {
        if (LogController.Instance != null) LogController.Instance.debug("Start Game.");
        if (LoaderConfig.Instance != null)
            LoaderConfig.Instance.changeScene(2);
    }
}
