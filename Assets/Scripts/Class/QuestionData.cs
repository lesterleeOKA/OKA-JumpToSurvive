using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class QuestionData
{ 
    public List<QuestionList> Data;
}
[Serializable]
public class QuestionList
{
    public string QID;
    public string Question;
    public string[] Answers;
    public string Answer;
    public string QuestionType;
    public Texture texture;
    public AudioClip audioClip;
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

    public enum QuestionType
    {
        None = 0,
        Text = 1,
        Picture = 2,
        Audio = 3,
    }

    public void setNewQuestion(QuestionList qa = null, int totalQuestion = 0)
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
                if (this.questionImage != null) this.questionImage.enabled = false;
                this.correctAnswer = qa.Answer;
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