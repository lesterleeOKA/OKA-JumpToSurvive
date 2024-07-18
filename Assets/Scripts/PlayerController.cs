using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int playerId = 0;
    public Color32 playerColor = Color.white;
    public Image playerIcon;
    public int score = 0;
    public string answer = string.Empty;
    public TextMeshProUGUI userName, scoreText, resultScore;
    public Animator scoringAnimator;
    public Button jumpBtn;
    public Sprite[] characterSprites;
    public Character character;
    public float jumpSpeedMultiplier = 5f;
    public float jumpForce = 10f;
    public float downGravity = 5f;
    private Rigidbody2D rb = null;
    public CanvasGroup answerBox;
    public TextMeshProUGUI answerText;

    void Start()
    {
        if(this.character != null) {  
            rb = this.character.GetComponent<Rigidbody2D>();
            this.character.enterGround += this.SetCharacterSprite;
            this.character.exitGround += this.SetCharacterSprite;
            this.character.collidedWord += this.TriggerWord;
        }

        if(this.jumpBtn != null)
        {
            this.jumpBtn.GetComponent<Image>().color = this.playerColor;
            this.jumpBtn.onClick.AddListener(this.Jump);
        }

        if(this.userName != null && this.resultScore != null && this.playerIcon != null)
        {
            this.userName.color = this.playerColor;
            this.playerIcon.color = this.playerColor;
            this.resultScore.color = this.playerColor;
        }

        this.Init();
    }

    public void Init()
    {
        this.setAnsswer("");
        this.SetCharacterSprite(0);
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
        }
    }


    public void checkAnswer()
    {
        if(QuestionController.Instance == null) return;
        if(this.answer == QuestionController.Instance.currentQuestion.correctAnswer)
        {
            if (this.scoreText != null && this.resultScore != null)
            {
                this.score += 10;
                if(this.scoringAnimator != null)
                {
                    this.scoringAnimator.SetTrigger("addScore");
                }
                this.scoreText.text = this.score.ToString();
                this.resultScore.text = this.score.ToString();
            }
        }

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
               !QuestionController.Instance.moveTonextQuestion)
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
        if(this.characterSprites[id] != null)
            this.character.gameObject.GetComponent<Image>().sprite = this.characterSprites[id];
    }

    void TriggerWord(string word)
    {
        Debug.Log("word belong to player:" + this.playerId);
        this.setAnsswer(word);
        //QuestionController.Instance.randAnswer();
    }

    private void OnApplicationQuit()
    {
        if (this.jumpBtn != null)
        {
            this.jumpBtn.onClick.RemoveAllListeners();
        }
    }
}
