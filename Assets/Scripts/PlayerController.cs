using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class PlayerController : UserData
{
    //public int playerId = 0;
    //public Color32 playerColor = Color.white;
    //public Image playerIcon;
    //public int score = 0;
    public int bonnus = 4;
    public string answer = string.Empty;
    public bool correct = false;
    public TextMeshProUGUI userNameText, scoreText, resultScore;
    public Animator scoringAnimator;
    public TextMeshProUGUI scroingTxt;
    public Button jumpBtn;
    public Sprite[] characterSprites;
    public Character character;
    public float jumpSpeedMultiplier = 5f;
    public float jumpForce = 10f;
    public float downGravity = 5f;
    private Rigidbody2D rb = null;
    public CanvasGroup answerBox;
    public TextMeshProUGUI answerText;
    private Image characterImage = null;
    private AudioSource effect = null;

    void Start()
    {
        if(this.effect == null) { this.effect = this.GetComponent<AudioSource>(); }
        if(this.character != null) {  
            rb = this.character.GetComponent<Rigidbody2D>();
            this.character.enterGround += this.SetCharacterSprite;
            this.character.exitGround += this.SetCharacterSprite;
            this.character.collidedWord += this.TriggerWord;
        }

        if(this.jumpBtn != null)
        {
            this.jumpBtn.GetComponent<Image>().color = this.PlayerColor;
            this.jumpBtn.onClick.AddListener(this.Jump);
        }

        if(this.userNameText != null && this.resultScore != null)
        {
            this.userNameText.color = this.PlayerColor;
            if (this.PlayerIcon != null) this.PlayerIcon.sprite = this.characterSprites[0];
            if (this.ScoringIcon != null) this.ScoringIcon.sprite = this.characterSprites[0];
            this.resultScore.color = this.PlayerColor;
        }

        this.characterImage = this.character.gameObject.GetComponent<Image>();
        this.Init();
    }

    public void Init()
    {
        this.setAnsswer("");
        this.SetCharacterSprite(0);
        this.bonnus = 4;
    }

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
            SetUI.Set(this.answerBox, true, 0f);
            this.answer = word;
            if (this.answerText != null) this.answerText.text = this.answer;

            if (this.answer != QuestionController.Instance.currentQuestion.correctAnswer)
                StartCoroutine(showHurt());
        }
    }


    public void checkAnswer(TextMeshProUGUI answeredEffectTxt)
    {
        if(QuestionController.Instance == null || this.scroingTxt == null || this.resultScore == null || this.scoringAnimator == null || answeredEffectTxt == null) return;
        if(this.answer == QuestionController.Instance.currentQuestion.correctAnswer)
        {
            this.correct = true;
            int mark = 10 * this.bonnus;
            this.Score += mark;
            answeredEffectTxt.text = "+" + mark;
            this.scroingTxt.text = "+" + mark;
            this.scoringAnimator.SetTrigger("addScore");
        }
        else
        {
            if (this.Score >= 10)
            {
                this.Score -= 10;
                answeredEffectTxt.text = "-10";
                this.scroingTxt.text = "-10";
                this.scoringAnimator.SetTrigger("addScore");
            }
            else
            {
                answeredEffectTxt.text = "0";
                this.scroingTxt.text = "0";
            }
        }

        this.scoreText.text = this.Score.ToString();
        this.resultScore.text = this.Score.ToString();

        this.Init();
    }

    void FixedUpdate()
    {
        if(this.character == null || rb == null) return;
        // Jump if the player presses the jump button and is grounded
       /* if (Input.GetKeyDown("space"))
        {
            //Debug.Log("Jumped");
            Jump();
        }*/

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = this.downGravity;
        }
        else
        {
            rb.gravityScale = 1.5f;
        }


        if(this.jumpBtn != null)
        {
            if (this.character.isGrounded &&
               StartGame.Instance.startedGame &&
               !QuestionController.Instance.moveTonextQuestion &&
               this.characterImage.sprite != this.characterSprites[2])
            {
                this.jumpBtn.interactable = true;
            }
            else
            {
                this.jumpBtn.interactable = false;
            }
        }
    }

    public void Jump()
    {
        if (AudioController.Instance != null) AudioController.Instance.PlayAudio(0);
        this.rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        this.rb.velocity = new Vector2(this.rb.velocity.x, this.rb.velocity.y * jumpSpeedMultiplier);
    }

    void SetCharacterSprite(int id)
    {
        //Debug.Log(id);
        if(this.characterSprites[id] != null && this.characterImage.sprite != this.characterSprites[2])
            this.characterImage.sprite = this.characterSprites[id];
    }


    void TriggerWord(string word)
    {
        Debug.Log("word belong to player:" + this.UserId);
        this.setAnsswer(word);
        //QuestionController.Instance.randAnswer();
    }

    IEnumerator showHurt()
    {
        if(this.bonnus > 0) this.bonnus -= 1;
        this.effect.Play();
        this.SetCharacterSprite(2);
        this.character.transform.DOShakePosition(0.3f, new Vector3(20f, 0f), 20).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo).SetAutoKill(true);
        yield return new WaitForSeconds(1f);
        this.characterImage.sprite = this.characterSprites[0];
    }

    private void OnApplicationQuit()
    {
        if (this.jumpBtn != null)
        {
            this.jumpBtn.onClick.RemoveAllListeners();
        }
    }
}
