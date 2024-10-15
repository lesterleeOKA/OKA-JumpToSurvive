using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup gameStartPanel;
    public float instructionPanelStartPosY = 200f;
    public AudioOnOff audioOnOffPanel;
    private void Awake()
    {
        Time.timeScale = 1f;
        LoaderConfig.Instance?.InitialGameSetup();
    }
    private void Start()
    {
        this.audioOnOffPanel.Init(true);
        SetUI.SetMove(this.gameStartPanel, false, new Vector2(0f, this.instructionPanelStartPosY), 0f);
    }

    public void MusicOnbutton()
    {
        this.audioOnOffPanel.set(true);
        this.audioOnOffPanel.setPanel(false);
        SetUI.SetMove(this.gameStartPanel, true, Vector2.zero, 0.5f);
    }
    public void MusicOffbutton()
    {
        this.audioOnOffPanel.set(false);
        this.audioOnOffPanel.setPanel(false);
        SetUI.SetMove(this.gameStartPanel, true, Vector2.zero, 0.5f);
    }

    public void StartGame()
    {
        AudioController.Instance?.PlayAudio(0);
        SetUI.SetMove(this.gameStartPanel, false, new Vector2(0f, this.instructionPanelStartPosY), 0.5f, ()=> this.gameStart());
    }

    void gameStart()
    {
        LogController.Instance?.debug("Start Game.");
        LoaderConfig.Instance?.changeScene(2);
    }
}
