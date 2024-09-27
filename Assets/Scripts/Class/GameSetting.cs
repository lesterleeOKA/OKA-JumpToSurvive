using System;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    public string currentURL;
    public GameSetup gameSetup;
    public APIManager apiManager;
    protected virtual void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        DontDestroyOnLoad(this);
    }

    protected virtual void Start()
    {
        this.apiManager.Init();
    }

    protected virtual void Update()
    {
        this.apiManager.controlDebugLayer();
    }

    public void InitialGameSetup(Action onCompleted = null)
    {
        //Download game background image
        StartCoroutine(this.gameSetup.Load("GameUI", "bg", _bgTexture =>
        {
            LogController.Instance?.debug($"Downloaded bg Image!!");
            ExternalCaller.UpdateLoadBarStatus("Loading Bg");
            this.gameSetup.bgTexture = _bgTexture;
            onCompleted?.Invoke();
        }));
    }

    public void InitialGameBackground()
    {
        this.gameSetup.setBackground();
        this.gameSetup.setInstruction(LoaderConfig.Instance?.apiManager.instructionContent);
    }
    public string CurrentURL
    {
        set { this.currentURL = value; }
        get { return this.currentURL; }
    }

    public float GameTime
    {
        get{return this.gameSetup.gameTime;}
        set{this.gameSetup.gameTime = value;}
    }

    public bool ShowFPS
    {
        get { return this.gameSetup.showFPS; }
        set { this.gameSetup.showFPS = value; }
    }

    public int PlayerNumbers
    {
        get { return this.gameSetup.playerNumber; }
        set { this.gameSetup.playerNumber = value; }
    }

    public void Reload()
    {
        ExternalCaller.ReLoadCurrentPage();
    }
}

[Serializable]
public class GameSetup: LoadImage
{
    [Tooltip("Default Game Background Texture")]
    public Texture bgTexture;
    [Tooltip("Find Tag name of GameBackground in different scene")]
    public RawImage gameBackground;
    public InstructionText instructions;
    public float gameTime;
    public bool showFPS = false;
    public int playerNumber = 1;

    public void setBackground()
    {
        if (this.gameBackground == null)
        {
            var tex = GameObject.FindGameObjectWithTag("GameBackground");
            this.gameBackground = tex.GetComponent<RawImage>();
        }

        if(this.gameBackground != null)
        {
            this.gameBackground.texture = this.bgTexture;
        }
    }

    public void setInstruction(string content="")
    {
        if (!string.IsNullOrEmpty(content) && this.instructions == null)
        {
            var instructionText = GameObject.FindGameObjectWithTag("Instruction");

            if(instructionText != null)
            {
                this.instructions = instructionText.GetComponent<InstructionText>();
                this.instructions.setContent(content);
            }
        }
    }
}


public static class APIConstant
{
    public static string QuestionDataHeaderName = "questions";
    public static string GameDataAPI(string _bookId = "", string _jwt = "")
    {
        string jsonParameter = string.IsNullOrEmpty(_bookId) ? "[1]" : $"[\"{_bookId}\"]";
        return $"https://ro2.azurewebsites.net/RainbowOne/index.php/PHPGateway/proxy/2.8/?api=ROGame.get_game_setting&json={jsonParameter}&jwt=" +_jwt;
    }

    public static string SubmitAnswerAPI(string playloads, int uid, string _jwt, Answer answer = null)
    {
        if(answer == null) return null;
        
        int stateDuration = answer.state.duration;
        float stateScore = answer.state.score;
        float statePercent = answer.state.percent;
        float stateProgress = answer.state.progress;

        int correct = answer.currentQA.correctId;
        float currentQADuration = answer.currentQA.duration;
        string currentqid = answer.currentQA.qid;
        int answerId = answer.currentQA.answerId;
        string answerText = answer.currentQA.answerText;
        string correctAnswerText = answer.currentQA.correctAnswerText;
        float currentQAscore = answer.currentQA.score;
        float currentQAPercent = answer.currentQA.percent;

        string jsonPayload = $"[{{\"payloads\":{playloads}," +
        $"\"role\":{{\"uid\":{uid}}}," +
        $"\"state\":{{\"duration\":{stateDuration},\"score\":{stateScore},\"percent\":{statePercent},\"progress\":{stateProgress}}}," +
        $"\"currentQuestion\":{{\"correct\":{correct},\"duration\":{currentQADuration},\"qid\":\"{currentqid}\",\"answer\":{answerId},\"answerText\":\"{answerText}\",\"correctAnswerText\":\"{correctAnswerText}\",\"score\":{currentQAscore},\"percent\":{currentQAPercent}}}}}]";

        string submitAPI = $"https://dev.openknowledge.hk/RainbowOne/index.php/PHPGateway/proxy/2.8/?api=ROGame.submit_answer&json={jsonPayload}&jwt=" + _jwt;
        return submitAPI;
    }
}
