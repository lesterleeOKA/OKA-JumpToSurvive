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
        //this.MusicOnbutton(false);
    }

    public void MusicOnbutton(bool PlayClick = true)
    {
        this.audioOnOffPanel.set(true, PlayClick);
        this.audioOnOffPanel.setPanel(false);
        SetUI.SetMove(this.gameStartPanel, true, Vector2.zero, 0.5f);
    }
    public void MusicOffbutton(bool PlayClick = true)
    {
        this.audioOnOffPanel.set(false, PlayClick);
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
