using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class QuestionController : MonoBehaviour
{
    public static QuestionController Instance = null;
    public float starPositionX;
    public float[] startPositionY = new float[2]{0f, 165f};
    public float[] speedRange = new float[2]{150f, 500f};
    public Transform wordParent;
    public GameObject word;
    public List<Bird> createdWords = new List<Bird>();
    public CurrentQuestion currentQuestion;
    public bool wordTriggering = false;
    public bool moveTonextQuestion = false;
    public bool allowCheckingWords = true;
    public bool showAnswers = false;
    public float delayToNextQuestion = 2f;
    public float count = 0f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        this.createdWords = new List<Bird>();
        this.count = this.delayToNextQuestion;
    }

    public void killAllWords()
    {
        for (int i = 0; i < this.createdWords.Count; i++)
        {
            var word = this.createdWords[i].gameObject;
            if (word != null)
            {
               Destroy(word);            
            }
        }
        this.createdWords.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown("q") && !this.wordTriggering)
        {
            StartCoroutine(reTriggerWords());
        }*/

        if(GameController.Instance != null && StartGame.Instance != null && this.createdWords != null)
        {
            if (!GameController.Instance.gameTimer.endGame && StartGame.Instance.startedGame && this.createdWords.Count > 0 && this.allowCheckingWords)
            {
                this.moveTonextQuestion = this.createdWords.All(word => !word.allowMove);

                if (this.moveTonextQuestion)
                {
                    if(this.count > 0f)
                    {
                        if (GameController.Instance != null && !this.showAnswers) { 
                            GameController.Instance.showAllCharacterAnswer();
                            this.showAnswers = true;
                        }

                        this.count -= Time.deltaTime;
                    }
                    else
                    {
                        this.count = this.delayToNextQuestion;
                        if (GameController.Instance != null) GameController.Instance.checkPlayerAnswer();
                        this.allowCheckingWords = false;
                    }
                }
            }
        }
    }

    public void nextQuestion()
    {
        if (LogController.Instance != null)
            LogController.Instance.debug("next question");
        this.GetQuestionAnswer();
    }

    /*private IEnumerator reTriggerWords()
    {
        this.wordTriggering = true;
        float _delay = UnityEngine.Random.Range(0.7f, 1.5f);
        var answers = this.createdWords.Count;
        for (int i = 0; i < this.createdWords.Count; i++)
        {
            var word = this.createdWords[i];
            if (word != null)
            {
                float posY = UnityEngine.Random.Range(startPosition.y - 250f, startPosition.y - 500f);
                word.gameObject.transform.localPosition = new Vector2(startPosition.x, posY);
                word.reTrigger();
                yield return new WaitForSeconds(_delay);
                if(i == answers - 1) this.wordTriggering = false;
            }
        }
    }*/

    public void randAnswer()
    {
        if (this.currentQuestion.qa == null || String.IsNullOrEmpty(this.currentQuestion.qa.QID)) return;
        //this.createdWords.Clear();
        StartCoroutine(this.InstantiateWordsWithDelay());     
    }

    private IEnumerator InstantiateWordsWithDelay()
    {
        yield return new WaitForSeconds(2f);
        this.wordTriggering = true;
        SortExtensions.ShuffleArray(this.currentQuestion.answersChoics);
        var answers = this.currentQuestion.answersChoics.Length;

        float minY = startPositionY[0]; //min
        float maxY = startPositionY[1]; //max
        float posY = UnityEngine.Random.Range(minY, maxY);
        for (int i = 0; i < answers; i++)
        {
            var answer = this.currentQuestion.answersChoics[i];
            float _delay = UnityEngine.Random.Range(1f, 3f);
            if (!string.IsNullOrEmpty(answer) && !GameController.Instance.gameTimer.endGame)
            {
                this.InstantiateWord(answer, i, posY);
                yield return new WaitForSeconds(_delay);
                if (i == answers - 1) this.wordTriggering = false;
            }
        }
    }

    void InstantiateWord(string text, int id, float posY)
    {
        // Instantiate the prefab at the current transform position and rotation
        Vector2 pos = new Vector2(this.starPositionX, posY);
        float speed = UnityEngine.Random.Range(this.speedRange[0], this.speedRange[1]);
        Material birdOutline = new Material(Shader.Find("Hidden/GlobalOutline"));

        Bird createdWord;
        if (this.createdWords.Count == this.currentQuestion.answersChoics.Length)
        {
            createdWord = this.createdWords[id];
            this.createdWords[id].gameObject.name = "word_" + id + "_" + text;
            createdWord.startPosition = pos;
            createdWord.speed = speed;
            createdWord.setWord(text, id + 1);
        }
        else
        {
            var rock = Instantiate(word, Vector2.zero, Quaternion.identity); // Corrected instantiation
            rock.name = "word_" + id + "_" + text;
            rock.transform.SetParent(this.wordParent, true); // Set parent and keep local scale
            rock.transform.localScale = Vector3.one; // Ensure scale is set correctly
            createdWord = rock.GetComponent<Bird>();
            createdWord.speed = speed;
            createdWord.startPosition = pos;
            createdWord.setWord(text, id + 1, birdOutline);
            this.createdWords.Add(createdWord);
        }

        if(id == 0) { 
            this.allowCheckingWords = true;
            this.showAnswers = false;
        }
    }

    public void GetQuestionAnswer()
    {
        if (LoaderConfig.Instance == null || QuestionManager.Instance == null)
            return;

        try
        {
            var questionDataList = QuestionManager.Instance.questionData;
            if (LogController.Instance != null) LogController.Instance.debug("Loaded questions:" + questionDataList.Data.Count);
            if (questionDataList == null || questionDataList.Data == null || questionDataList.Data.Count == 0)
            {
                return;
            }

            string correctAnswer = this.currentQuestion.correctAnswer;
            int questionCount = questionDataList.Data.Count;
            QuestionList qa = questionDataList.Data[this.currentQuestion.numberQuestion];
            this.currentQuestion.setNewQuestion(qa, questionCount);
            this.moveTonextQuestion = false;
            this.randAnswer();
        }
        catch (Exception e)
        {
            if (LogController.Instance != null) LogController.Instance.debugError(e.Message);
        }

    }

    public void PlayCurrentQuestionAudio()
    {
        this.currentQuestion.playAudio();
    }
}

