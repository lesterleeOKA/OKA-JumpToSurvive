using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionController : MonoBehaviour
{
    public static QuestionController Instance = null;
    public string[] questions = new string[4] {"test", "book", "color", "car"};
    public Text qaBoard;
    public bool answered = false;
    public Word word;

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
        this.randomQuestion();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            randomQuestion();
        }
    }


    public void NextQuestion()
    {
        StartCoroutine(nextQuestion());
    }

    IEnumerator nextQuestion()
    {
        yield return new WaitForSeconds(2.0f);
        this.randomQuestion();
    }
    public void randomQuestion()
    {
        this.answered = false;

        int randQA = Random.Range(0, this.questions.Length);

        if (this.qaBoard != null) this.qaBoard.text = this.questions[randQA];

        GameController.Instance.RandomlySortChildObjects();

        if(this.word != null)
        {
            this.word.setWord(this.questions[randQA]);
        }
    }
}
