using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

public class QuestionController : MonoBehaviour
{
    public static QuestionController Instance = null;
    public Vector2 startPosition;
    public Transform wordParent;
    public GameObject word;
    public List<Word> createdWords = new List<Word>();
    public CurrentQuestion currentQuestion;
    public bool wordTriggering = false;
    public bool moveTonextQuestion = true;
    private bool allowCheckingWords = true;
    public float delayToNextQuestion = 2f;
    private float count = 0f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        this.createdWords = new List<Word>();
        this.count = this.delayToNextQuestion;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q") && !this.wordTriggering)
        {
            StartCoroutine(reTriggerWords());
        }

        if(GameController.Instance != null && StartGame.Instance != null && this.createdWords != null)
        {
            if (!GameController.Instance.gameTimer.endGame && StartGame.Instance.startedGame && this.createdWords.Count > 0 && this.allowCheckingWords)
            {
                this.moveTonextQuestion = this.createdWords.All(word => !word.allowMove);

                if (this.moveTonextQuestion)
                {
                    if(this.count > 0f)
                    {
                        this.count -= Time.deltaTime;
                    }
                    else
                    {
                        this.count = this.delayToNextQuestion;
                        if (GameController.Instance != null) GameController.Instance.resetPlayers();
                        this.moveTonextQuestion = false;
                        this.allowCheckingWords = false;
                    }
                }
            }
        }
    }

    public void nextQuestion()
    {
        Debug.Log("next question");
        this.GetQuestionAnswer();
    }

    private IEnumerator reTriggerWords()
    {
        this.wordTriggering = true;
        float _delay = UnityEngine.Random.Range(0.8f, 2f);
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
    }

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
        for (int i = 0; i < answers; i++)
        {
            var answer = this.currentQuestion.answersChoics[i];
            float _delay = UnityEngine.Random.Range(1f, 3f);
            if (!string.IsNullOrEmpty(answer))
            {
                this.InstantiateWord(answer, i);
                yield return new WaitForSeconds(_delay);
                if (i == answers - 1) this.wordTriggering = false;
            }
        }
    }

    void InstantiateWord(string text, int id)
    {
        // Instantiate the prefab at the current transform position and rotation
        float minY = startPosition.y - 500f;
        float maxY = startPosition.y - 250f;
        float posY = UnityEngine.Random.Range(minY, maxY); // Corrected range
        Vector2 pos = new Vector2(startPosition.x, posY);

        Word createdWord;

        if (this.createdWords.Count == this.currentQuestion.answersChoics.Length)
        {
            createdWord = this.createdWords[id];
            this.createdWords[id].gameObject.name = "word_" + id + "_" + text;
            createdWord.startPosition = pos;
            createdWord.setWord(text);
        }
        else
        {
            var rock = Instantiate(word, Vector2.zero, Quaternion.identity); // Corrected instantiation
            rock.name = "word_" + id + "_" + text;
            rock.transform.SetParent(this.wordParent, true); // Set parent and keep local scale
            rock.transform.localScale = Vector3.one; // Ensure scale is set correctly
            createdWord = rock.GetComponent<Word>();
            createdWord.startPosition = pos;
            createdWord.setWord(text);
            this.createdWords.Add(createdWord);
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
            this.allowCheckingWords = true;
            this.randAnswer();
        }
        catch (Exception e)
        {
            if (LogController.Instance != null) LogController.Instance.debugError(e.Message);
        }

    }
}

