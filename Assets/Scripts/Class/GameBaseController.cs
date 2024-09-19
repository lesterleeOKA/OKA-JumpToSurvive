using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBaseController : MonoBehaviour
{
    public Timer gameTimer;
    public CanvasGroup GameUILayer, TopUILayer, TopRightUILayer, getScorePopup;
    protected Vector2 originalGetScorePos = Vector2.zero;
    public EndGamePage endGamePage;

    protected virtual void Awake()
    {
        LoaderConfig.Instance?.InitialGameBackground();
    }

    protected virtual void Start()
    {
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
    }

    public virtual void endGame()
    {
        SetUI.Set(this.TopUILayer, false, 0f);
        SetUI.Set(this.GameUILayer, false, 0f);
        SetUI.Set(this.TopRightUILayer, false, 0f);
    }

    public void retryGame()
    {
        if (AudioController.Instance != null) AudioController.Instance.changeBGMStatus(true);
        SceneManager.LoadScene(2);
    }

    public void BackToWebpage()
    {
        ExternalCaller.BackToHomeUrlPage();
    }
}
