using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class QuestionDataWrapper
{
    public QuestionList[] QuestionDataArray;
}

[Serializable]
public class QuestionData
{ 
    public List<QuestionList> Data;
}
[Serializable]
public class QuestionList
{
    public int id;
    public string QID;
    public string QuestionType;
    public string Question;
    public string[] Answers;
    public string CorrectAnswer;
    public string[] Media;
    public Texture texture;
    public AudioClip audioClip;
}

public enum QuestionType
{
    None = 0,
    Text = 1,
    Picture = 2,
    Audio = 3,
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
    public CanvasGroup[] questionBgs;
    public RawImage questionImage;
    public Button audioPlayBtn;
    private AspectRatioFitter aspecRatioFitter = null;

    public void setNewQuestion(QuestionList qa = null, int totalQuestion = 0)
    {
        if (qa == null) return;
        this.qa = qa;

        switch (qa.QuestionType)
        {
            case "Picture":
                SetUI.SetGroup(this.questionBgs, 0, 0f);
                this.questiontype = QuestionType.Picture;
                var qaImage = qa.texture;
                if (this.questionText != null) this.questionText.enabled = false;
                if (this.questionText != null) this.questionText.text = "";
                SetUI.Set(this.audioPlayBtn.GetComponent<CanvasGroup>(), false, 0f);
                this.correctAnswer = qa.CorrectAnswer;
                this.answersChoics = qa.Answers;
                if (this.questionImage != null && qaImage != null)
                {
                    this.questionImage.enabled = true;
                    this.aspecRatioFitter = this.questionImage.GetComponent<AspectRatioFitter>();
                    this.questionImage.texture = qaImage;
                    var width = this.questionImage.GetComponent<RectTransform>().sizeDelta.x;
                    if (qaImage.width > qaImage.height)
                    {
                        this.questionImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 350f);
                    }
                    else
                    {
                        this.questionImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 430f);
                    }
                    this.aspecRatioFitter.aspectRatio = (float)qaImage.width / (float)qaImage.height;
                }
                break;
            case "Audio":
                SetUI.SetGroup(this.questionBgs, 1, 0f);
                if (this.questionText != null) this.questionText.enabled = false;
                if (this.questionText != null) this.questionText.text = "";
                if (this.questionImage != null) this.questionImage.enabled = false;
                SetUI.Set(this.audioPlayBtn.GetComponent<CanvasGroup>(), true, 0f);
                this.questiontype = QuestionType.Audio;
                this.correctAnswer = qa.CorrectAnswer;
                this.answersChoics = qa.Answers;
                this.playAudio();
                break;
            case "Text":
                SetUI.SetGroup(this.questionBgs, 2, 0f);
                SetUI.Set(this.audioPlayBtn.GetComponent<CanvasGroup>(), false, 0f);
                this.questiontype = QuestionType.Text;
                if (this.questionText != null) this.questionText.enabled = true;
                if (this.questionText != null) this.questionText.text = qa.Question;
                if (this.questionImage != null) this.questionImage.enabled = false;
                this.correctAnswer = qa.CorrectAnswer;
                this.answersChoics = qa.Answers;
                break;
        }

        if (LogController.Instance != null)
        {
            LogController.Instance.debug($"Get new {nameof(this.questiontype)} question");
        }

        if (this.numberQuestion < totalQuestion - 1)
            this.numberQuestion += 1;
        else
            this.numberQuestion = 0;
    }

    public void playAudio()
    {
        if(this.audioPlayBtn != null && this.qa.audioClip != null)
        {
            this.audioPlayBtn.GetComponent<AudioSource>().clip = this.qa.audioClip;
            this.audioPlayBtn.GetComponent<AudioSource>().Play();
        }
    }
}

public static class SortExtensions
{
    // Fisher-Yates shuffle algorithm
    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1); // Use Unity's Random class
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public static void ShuffleArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, array.Length);
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}