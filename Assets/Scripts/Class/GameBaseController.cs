using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBaseController : MonoBehaviour
{
    public Timer gameTimer;
    public CanvasGroup GameUILayer, TopUILayer, TopRightUILayer, getScorePopup;
    protected Vector2 originalGetScorePos = Vector2.zero;
    public EndGamePage endGamePage;
    public int playerNumber = 0;
    public bool playing = false;

    protected virtual void Awake()
    {
        LoaderConfig.Instance?.InitialGameSetup();
    }

    protected virtual void Start()
    {
        if(LoaderConfig.Instance != null) this.playerNumber = LoaderConfig.Instance.PlayerNumbers;
        SetUI.Set(this.TopUILayer, false, 0f);
        SetUI.Set(this.getScorePopup, false, 0f);
        SetUI.Set(this.TopRightUILayer, true, 0f);
        if (this.getScorePopup != null) this.originalGetScorePos = this.getScorePopup.transform.localPosition;
        this.endGamePage.init();
    }

    public virtual void enterGame()
    {
        SetUI.Set(this.TopUILayer, true, 0.5f);
        SetUI.Set(this.GameUILayer, true, 0.5f);
        this.playing = true;
    }

    public virtual void endGame()
    {
        SetUI.Set(this.TopUILayer, false, 0f);
        SetUI.Set(this.GameUILayer, false, 0f);
        SetUI.Set(this.TopRightUILayer, false, 0f);
        this.playing = false;
    }

    public void retryGame()
    {
        if (AudioController.Instance != null) 
            AudioController.Instance.changeBGMStatus(true);

        LoaderConfig.Instance?.exitPage("Replay", null, ()=> SceneManager.LoadScene(2));
    }

    public void setGetScorePopup(bool status)
    {
        SetUI.SetMove(this.getScorePopup, status, status ? Vector2.zero : this.originalGetScorePos, status ? 0.5f : 0f);
    }

    public void BackToWebpage()
    {
        LoaderConfig.Instance?.exitPage("Leave Game", ExternalCaller.BackToHomeUrlPage, null);

    }
}
