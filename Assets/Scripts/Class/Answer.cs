[System.Serializable]
public class Answer 
{
    /*string jsonPayload = $"[{{\"payloads\":{playloads}," +
        $"\"role\":{{\"uid\":{uid}}}," +
        $"\"state\":{{\"duration\":{60},\"score\":{5},\"percent\":{100},\"progress\":{25}}}," +
        $"\"currentQuestion\":{{\"correct\":{2},\"duration\":{20},\"qid\":\"{qid}\",\"answer\":{1},\"answerText\":\"{"correct2"}\",\"correctAnswerText\":\"{"correct2"}\",\"score\":{1},\"percent\":{100}}}}}]";*/

    public State state;
    public CurrentQA currentQA;
}


[System.Serializable]
public class State
{
    public int duration;
    public float score;
    public float percent;
    public int progress;
}

[System.Serializable]
public class CurrentQA
{
    public int correctId;
    public float duration;
    public string qid;
    public int answerId;
    public string answerText;
    public string correctAnswerText;
    public float score;
    public float percent;
}
