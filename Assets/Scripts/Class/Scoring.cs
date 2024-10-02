using TMPro;
using UnityEngine;

[System.Serializable]
public class Scoring
{
    public bool usingDeductLogic = false;
    public int bonnus = 4;
    public bool correct = false;
    public TextMeshProUGUI scoreTxt, scoringTxt, resultScoreTxt;
    public TextMeshProUGUI answeredEffectTxt;
    public Animator scoringAnimator;

    public void init()
    {
        if (this.scoreTxt != null) this.scoreTxt.text = "0";
        if (this.scoringTxt != null) this.scoreTxt.text = "0";
        if (this.answeredEffectTxt != null) this.answeredEffectTxt.text = "0";
        if (this.resultScoreTxt != null) { 
            this.scoreTxt.text = "0";
        }
    }


    public int score(string _answer, int _playerScore, string _correctAnswer, int eachMarkOfQA=10)
    {
        int _score = _playerScore;
        if (this.scoringTxt == null || this.resultScoreTxt == null || this.scoringAnimator == null || answeredEffectTxt == null) {

            LogController.Instance?.debugError("One or more required components are missing.");
            return _score;
        }

        answeredEffectTxt.text = "0";
        if (!string.IsNullOrEmpty(_answer))
        {
            if (!string.IsNullOrEmpty(_correctAnswer))
            {
                if(_answer == _correctAnswer)
                {
                    this.correct = true;
                    int mark = eachMarkOfQA * this.bonnus;
                    _playerScore += mark;
                    answeredEffectTxt.text = "+" + mark;
                    this.scoringTxt.text = "+" + mark;
                    //this.scoringAnimator.SetTrigger("addScore");
                }
                else
                {
                    if (this.usingDeductLogic)
                    {
                        if (_playerScore >= 10)
                        {
                            _playerScore -= 10;
                            this.answeredEffectTxt.text = "-10";
                            this.scoringTxt.text = "-10";
                            //this.scoringAnimator.SetTrigger("addScore");
                        }
                        else
                        {
                            _playerScore = 0;
                            this.answeredEffectTxt.text = "0";
                            this.scoringTxt.text = "0";
                        }
                    }
                }
            }

            //this.scoreTxt.text = _playerScore.ToString();
            this.scoreTxt.GetComponent<NumberCounter>().Value = _playerScore;
            //this.resultScoreTxt.text = _playerScore.ToString();

            _score = _playerScore;
        }

        return _score;
    }
}
