using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBaseController : MonoBehaviour
{
    public Timer gameTimer;
    public CanvasGroup GameUILayer, TopUILayer, TopRightUILayer, getScorePopup;
    protected Vector2 originalGetScorePos = Vector2.zero;
    public EndGamePage endGamePage;
    public int playerNumber = 0;

    protected virtual void Awake()
    {
        LoaderConfig.Instance?.InitialGameBackground();
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

    public void setGetScorePopup(bool status)
    {
        SetUI.SetMove(this.getScorePopup, status, status ? Vector2.zero : this.originalGetScorePos, status ? 0.5f : 0f);
    }

    public void BackToWebpage()
    {
        bool containsJwt = LoaderConfig.Instance.CurrentURL.Contains("?jwt=");
        if(containsJwt) { 
            StartCoroutine(LoaderConfig.Instance.apiManager.ExitGameRecord(
                ()=> ExternalCaller.BackToHomeUrlPage(true)
            ));
        }
        else
        {
            ExternalCaller.BackToHomeUrlPage(false);
        }
    }

    private void OnApplicationQuit()
    {
        bool containsJwt = LoaderConfig.Instance.CurrentURL.Contains("?jwt=");
        if (containsJwt){
            LogController.Instance?.debug("Quit Game, called exit api.");
            StartCoroutine(LoaderConfig.Instance.apiManager.ExitGameRecord(null));
        }
        else
        {
            LogController.Instance?.debug("Quit Game.");
        }
    }
}
