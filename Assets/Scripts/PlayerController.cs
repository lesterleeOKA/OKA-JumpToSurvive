using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class PlayerController : UserData
{
    public Scoring scoring;
    public string answer = string.Empty;
    public TextMeshProUGUI userNameText;
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
    public ParticleSystem jumpup_particle;

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

        if(this.userNameText != null)
        {
            this.userNameText.color = this.PlayerColor;
            for(int i=0;i < this.PlayerIcons.Length; i++)
            {
                if (this.PlayerIcons[i] != null) this.PlayerIcons[i].sprite = this.characterSprites[0];
            }
            this.scoring.init();
        }

        this.characterImage = this.character.gameObject.GetComponent<Image>();
        this.Init();
    }

    public void Init()
    {
        this.setAnsswer("");
        this.SetCharacterSprite(0);
        this.scoring.bonnus = 4;
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

            //if (this.answer != QuestionController.Instance.currentQuestion.correctAnswer)
              //  StartCoroutine(showHurt());
        }
    }


    public void checkAnswer()
    {
        int currentScore = this.Score;
        int resultScore = this.scoring.score(this.answer, currentScore);
        this.Score = resultScore;
        if (LogController.Instance != null) LogController.Instance.debug("Add marks" + this.Score);
        this.Init();
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
               this.characterImage.sprite != this.characterSprites[2] &&
                string.IsNullOrEmpty(this.answerText.text))
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
        if(this.jumpup_particle != null) this.jumpup_particle.Play();
    }

    void SetCharacterSprite(int id)
    {
        if(this.characterSprites[id] != null && this.characterImage.sprite != this.characterSprites[2])
            this.characterImage.sprite = this.characterSprites[id];
    }


    void TriggerWord(string word)
    {
        if (LogController.Instance != null) 
            LogController.Instance.debug("word belong to player:" + this.UserId);
        this.setAnsswer(word);
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
