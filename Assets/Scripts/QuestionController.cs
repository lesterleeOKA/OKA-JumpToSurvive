using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class QuestionController : MonoBehaviour
{
    public static QuestionController Instance = null;
    public Word word;

    public CurrentQuestion currentQuestion;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        this.GetQuestionAnswer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            GetQuestionAnswer();
        }
    }

    public void nextQuestion()
    {
        this.GetQuestionAnswer();
    }

    public void randAnswer()
    {
        if(this.currentQuestion.qa == null) return;
        var randId = UnityEngine.Random.Range(0, this.currentQuestion.answersChoics.Length);
        var answer = this.currentQuestion.answersChoics[randId];
        this.word.setWord(answer);
    }

    public void GetQuestionAnswer(Action finisedAction = null)
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
            int answeredQA = this.currentQuestion.numberQuestion;
            string correctAnswer = this.currentQuestion.correctAnswer;

            if (answeredQA >= questionDataList.Data.Count)
            {
                answeredQA = 0;
            }

            int questionCount = questionDataList.Data.Count;
            QuestionList qa = questionDataList.Data[answeredQA];
            this.currentQuestion.setNewQuestion(qa);

            answeredQA = (answeredQA + 1) % questionDataList.Data.Count;
            if(GameController.Instance != null) GameController.Instance.RandomlySortChildObjects();
            this.randAnswer();
        }
        catch (Exception e)
        {
            if (LogController.Instance != null) LogController.Instance.debugError(e.Message);
        }

    }
}



[Serializable]
public class CurrentQuestion
{
    public int numberQuestion = 0;
    public QuestionType questiontype = QuestionType.None;
    public QuestionList qa = null;
    public TextMeshProUGUI questionText;
    public string correctAnswer;
    public string[] answersChoics;
    public RawImage questionImage;
    private AspectRatioFitter aspecRatioFitter = null;

    public void setNewQuestion(QuestionList qa = null)
    {
        if (qa == null) return;
        this.qa = qa;

        switch (qa.QuestionType)
        {
            case "Picture":
                this.questiontype = QuestionType.Picture;
                this.aspecRatioFitter = this.questionImage.GetComponent<AspectRatioFitter>();
                var qaImage = qa.texture;

                if (this.questionImage != null && qaImage != null)
                {
                    this.questionImage.texture = qaImage;
                    this.aspecRatioFitter.aspectRatio = (float)qaImage.width / (float)qaImage.height;
                }
                break;
            case "Audio":
                this.questiontype = QuestionType.Picture;
                break;
            case "Text":
                this.questiontype = QuestionType.Text;
                if (this.questionText != null) this.questionText.enabled = true;
                if (this.questionText != null) this.questionText.text = qa.Question;
                if (this.questionImage!= null)this.questionImage.enabled = false;
                this.correctAnswer = qa.Answer;
                this.answersChoics = qa.Answers;
                break;
        }

        if (LogController.Instance != null)
        {
            LogController.Instance.debug($"Get new {nameof(this.questiontype)} question");
        }

        this.numberQuestion += 1;
    }
}


public enum QuestionType
{
    None=0,
    Text=1,
    Picture=2,
    Audio=3,
}