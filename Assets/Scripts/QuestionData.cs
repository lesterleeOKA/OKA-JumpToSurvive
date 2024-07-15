using System;
using System.Collections.Generic;
using UnityEngine;


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

public static class ListExtensions
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
}