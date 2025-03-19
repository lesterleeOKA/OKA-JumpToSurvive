using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class PlayerController : UserData
{
    public CharacterSet characterSet;
    public Scoring scoring;
    public string answer = string.Empty;
    public CanvasGroup userNameBox;
    public Button jumpBtn;
    public Character character;
    public float jumpSpeedMultiplier = 5f;
    public float jumpForce = 10f;
    public float downGravity = 5f;
    private Rigidbody2D rb = null;
    public CanvasGroup answerBox;
    public TextMeshProUGUI answerText;
    private RawImage characterImage = null;
    private AudioSource effect = null;
    public ParticleSystem jumpup_particle;
    private CanvasGroup jumpBtnGroup = null;

    public CanvasGroup[] playerNoTexts;

    public void Init(CharacterSet characterSet = null)
    {
        this.characterSet = characterSet;
        this.PlayerColor = characterSet.playerColor;
        if (this.effect == null) { this.effect = this.GetComponent<AudioSource>(); }

        if (this.playerNoTexts[this.UserId] != null)
        {
            SetUI.Set(this.playerNoTexts[this.UserId], true);
            this.playerNoTexts[this.UserId].GetComponentInChildren<TextMeshProUGUI>().color = this.PlayerColor;
        }
        if (this.character != null)
        {
            this.rb = this.character.GetComponent<Rigidbody2D>();
            this.character.enterGround += this.SetCharacterSprite;
            this.character.exitGround += this.SetCharacterSprite;
            this.character.collidedWord += this.TriggerWord;
        }

        if (this.jumpBtn != null)
        {
            this.jumpBtnGroup = this.jumpBtn.GetComponent<CanvasGroup>();
            this.jumpBtn.GetComponent<Image>().color = this.PlayerColor;
            this.jumpBtn.onClick.AddListener(this.Jump);
        }
        else
        {
            var playerController = GameObject.FindGameObjectWithTag("P" + this.RealUserId + "_controller");
            if (playerController != null)
            {
                this.jumpBtn = playerController.GetComponent<Button>();
                this.jumpBtnGroup = playerController.GetComponent<CanvasGroup>();
                this.jumpBtn.onClick.AddListener(this.Jump);
            }
        }

        if (this.PlayerIcons[0] == null)
        {
            this.PlayerIcons[0] = GameObject.FindGameObjectWithTag("P" + this.RealUserId + "_Icon").GetComponent<PlayerIcon>();
        }

        if (this.scoring.scoreTxt == null)
        {
            this.scoring.scoreTxt = GameObject.FindGameObjectWithTag("P" + this.RealUserId + "_Score").GetComponent<TextMeshProUGUI>();
        }

        if (this.scoring.answeredEffectTxt == null)
        {
            this.scoring.answeredEffectTxt = GameObject.FindGameObjectWithTag("P" + this.RealUserId + "_AnswerScore").GetComponent<TextMeshProUGUI>();
        }

        if (this.scoring.resultScoreTxt == null)
        {
            this.scoring.resultScoreTxt = GameObject.FindGameObjectWithTag("P" + this.RealUserId + "_ResultScore").GetComponent<TextMeshProUGUI>();
        }
        this.scoring.init();

        this.characterImage = this.character.gameObject.GetComponent<RawImage>();

        this.resetPlayer();
    }

    void resetPlayer()
    {
        this.setAnsswer("");
        this.SetCharacterSprite(0);
        this.scoring.bonnus = 1;
    }

    public void updatePlayerIcon(bool _status = false, string _playerName = "", Sprite _icon = null)
    {
        for (int i = 0; i < this.PlayerIcons.Length; i++)
        {
            if (this.PlayerIcons[i] != null)
            {
                this.PlayerIcons[i].playerColor = this.PlayerColor;
                this.PlayerIcons[i].SetStatus(_status, _playerName, _icon);
            }
        }
    }

    /*public void updatePlayerIcon()
    {
        var icon = LoaderConfig.Instance.apiManager.peopleIcon != null ? SetUI.ConvertTextureToSprite(LoaderConfig.Instance.apiManager.peopleIcon as Texture2D) : null;

        var _playerName = LoaderConfig.Instance.apiManager.loginName;
        if (!string.IsNullOrEmpty(_playerName))
        {
            SetUI.Set(this.userNameBox, true, 0f);
            if (this.userNameBox != null)
            {
                var nameText = this.userNameBox.GetComponentInChildren<TextMeshProUGUI>();
                nameText.text = _playerName;
            }
        }
        else
        {
            SetUI.Set(this.userNameBox, false, 0f);
        }

        for (int i = 0; i < this.PlayerIcons.Length; i++)
        {
            if (this.PlayerIcons[i] != null && icon != null)
            {
                this.PlayerIcons[i].sprite = icon;
            }
        }
    }*/

    public void setAnsswer(string word)
    {
        if(string.IsNullOrEmpty(word))
        {
            SetUI.Set(this.answerBox, false, 0f);
            this.answer = "";
            if (this.answerText != null) this.answerText.text = "";
        }
        else
        {
            this.answer = word;
            if (!string.IsNullOrEmpty(this.answer))
            {
                SetUI.Set(this.answerBox, true, 0.5f);
            }
            if (this.answerText != null) this.answerText.text = this.answer;

        }
    }

    public void showCharacterAnswer()
    {
        if (!string.IsNullOrEmpty(this.answer)) {
            SetUI.Set(this.answerBox, true, 0f);
        }
    }

    public void checkAnswer(int currentTime)
    {
        var loader = LoaderConfig.Instance;
        var currentQuestion = QuestionController.Instance?.currentQuestion;
        int eachQAScore = currentQuestion.qa.score.full == 0 ? 10 : currentQuestion.qa.score.full;
        int currentScore = this.Score;
        int resultScore = this.scoring.score(this.answer, currentScore,
                                            currentQuestion.correctAnswer,
                                            eachQAScore);
        this.Score = resultScore;
        LogController.Instance?.debug("Add marks" + this.Score);

        if(this.UserId == 0 && loader != null && loader.apiManager.IsLogined) // For first player
        {
            float currentQAPercent = 0f;
            int correctId = 0;
            float score = 0f;
            float answeredPercentage = 0f;
            int progress = (int)((float)currentQuestion.answeredQuestion / QuestionManager.Instance.totalItems * 100);

            if (this.answer == currentQuestion.correctAnswer)
            {
                if(this.CorrectedAnswerNumber < QuestionManager.Instance.totalItems)
                    this.CorrectedAnswerNumber += 1;

                correctId = 2;
                score = eachQAScore; // load from question settings score of each question
                currentQAPercent = 100f;
            }
            else{
                if (this.CorrectedAnswerNumber > 0)
                {
                    this.CorrectedAnswerNumber -= 1;
                }
            }

            if(this.CorrectedAnswerNumber < QuestionManager.Instance.totalItems) { 
                answeredPercentage = this.AnsweredPercentage(QuestionManager.Instance.totalItems); 
            }
            else{
                answeredPercentage = 100f;
            }

            loader.SubmitAnswer(
                       currentTime,
                       this.Score,
                       answeredPercentage,
                       progress,
                       correctId,
                       currentTime,
                       currentQuestion.qa.qid,
                       currentQuestion.correctAnswerId,
                       this.answer,
                       currentQuestion.correctAnswer,
                       score,
                       currentQAPercent
                       );
        }
        this.resetPlayer();
    }

    void FixedUpdate()
    {
        if(this.character == null || rb == null) return;
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = this.downGravity;
        }
        else
        {
            rb.gravityScale = 1.5f;
        }


        if(this.jumpBtn != null && this.answerText != null)
        {
            if (this.character.isGrounded &&
               StartGame.Instance.startedGame &&
               QuestionController.Instance.allowCheckingWords &&
                string.IsNullOrEmpty(this.answerText.text))
            {
                this.jumpBtnGroup.alpha = 1f;
                this.jumpBtnGroup.interactable = true;
                this.jumpBtnGroup.blocksRaycasts = true;
            }
            else
            {
                this.jumpBtnGroup.alpha = 0.75f;
                this.jumpBtnGroup.interactable = false;
                this.jumpBtnGroup.blocksRaycasts = false;
            }
        }
    }

    public void Jump()
    {
        if (AudioController.Instance != null) AudioController.Instance.PlayAudio(0);
        this.rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        this.rb.velocity = new Vector2(this.rb.velocity.x, this.rb.velocity.y * this.jumpSpeedMultiplier);
        if(this.jumpup_particle != null) this.jumpup_particle.Play();
    }

    void SetCharacterSprite(int id)
    {
        var animationSprites = this.characterSet.animationTextures;
        if(animationSprites[id] != null)
            this.characterImage.texture = animationSprites[id];
    }


    void TriggerWord(Bird bird)
    {
        LogController.Instance?.debug("word belong to player:" + this.UserId);
        this.setAnsswer(bird.word.text);
        if (AudioController.Instance.audioStatus) this.effect.Play();
        //QuestionController.Instance.randAnswer();
    }

    IEnumerator showHurt()
    {
        if(this.scoring.bonnus > 1) this.scoring.bonnus -= 1;
        if(AudioController.Instance.audioStatus) this.effect.Play();
        this.SetCharacterSprite(2);
        this.character.transform.DOShakePosition(0.3f, new Vector3(20f, 0f), 20).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo).SetAutoKill(true);
        yield return new WaitForSeconds(1f);
        this.characterImage.texture = this.characterSet.animationTextures[0];
    }

    private void OnApplicationQuit()
    {
        if (this.jumpBtn != null)
        {
            this.jumpBtn.onClick.RemoveAllListeners();
        }
    }
}
