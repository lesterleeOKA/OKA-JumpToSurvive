using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance = null;
    private List<int> previousSiblingIndices = new List<int>();
    public List<RectTransform> playersList = new List<RectTransform>();
    public HorizontalLayoutGroup layoutGroup;
    public Timer gameTimer;
    public CanvasGroup GameUILayer, TopUILayer;
    private Vector2 originalGetScorePos = Vector2.zero;
    public CanvasGroup getScorePopup;
    public EndGamePage endGamePage;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        LoaderConfig.Instance?.InitialGameBackground();
    }

    private void Start()
    {
        SetUI.Set(this.TopUILayer, false, 0f);
        SetUI.Set(this.getScorePopup, false, 0f);
        if(this.getScorePopup != null) this.originalGetScorePos = this.getScorePopup.transform.localPosition;
        //this.RandomlySortChildObjects();
        this.endGamePage.init();
    }

    public void enterGame(bool status)
    {
        SetUI.Set(this.TopUILayer, status, status ? 0.5f: 0f);
        SetUI.Set(this.GameUILayer, status, status ? 0.5f : 0f);
        if (!status)
        {
            QuestionController.Instance.killAllWords();

            bool showSuccess = false;
            for (int i = 0; i < this.playersList.Count; i++)
            {
                var playerController = this.playersList[i].GetComponent<PlayerController>();
                if (playerController != null)
                {
                    if (playerController.Score >= 30)
                    {
                        showSuccess = true;
                    }

                    this.endGamePage.updateFinalScore(i, playerController.Score);
                }
            }

            this.endGamePage.setStatus(true, showSuccess);
        }
        else
        {
            for (int i = 0; i < this.playersList.Count; i++)
            {
                if (i == 0)
                {
                    var playerController = this.playersList[i].GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.updatePlayerIcon();
                    }
                }
            }
        }
    }


    public void showAllCharacterAnswer()
    {
        for (int i = 0; i < this.playersList.Count; i++)
        {
            var playerController = this.playersList[i].GetComponent<PlayerController>();
            if (playerController != null)
            {
               playerController.showCharacterAnswer();
            }

        }
    }

    public void checkPlayerAnswer()
    {
        StartCoroutine(checkAllAnswer());
    }

    private IEnumerator checkAllAnswer()
    {
        bool showCorrect = false;
        float delay = 1f;
        for (int i = 0; i < this.playersList.Count; i++)
        {
            var playerController = this.playersList[i].GetComponent<PlayerController>();
            if (playerController != null) {
                playerController.checkAnswer();

                if(playerController.scoring.correct && !showCorrect) {
                    SetUI.SetMove(this.getScorePopup, true, new Vector2(0f, 0f), 0.5f);
                    delay = 2f;
                    showCorrect = true;
                }
            }

        }

        AudioController.Instance?.PlayAudio(showCorrect ? 1 : 2);
        yield return new WaitForSeconds(delay);
        SetUI.SetMove(this.getScorePopup, false, this.originalGetScorePos, 0f);
        //this.RandomlySortChildObjects();

        for (int i = 0; i < this.playersList.Count; i++)
        {
            var playerController = this.playersList[i].GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.scoring.correct = false;
            }

        }
        QuestionController.Instance.nextQuestion();
    }

    public void retryGame()
    {
        if (AudioController.Instance != null) AudioController.Instance.changeBGMStatus(true);
        SceneManager.LoadScene(2);
    }

    public void RandomlySortChildObjects()
    {
        // Create a list of available sibling indices (0 to playersList.Count - 1)
        List<int> availableSiblingIndices = Enumerable.Range(0, playersList.Count).ToList();

        // Ensure the new order does not match the previous order
        List<int> newSiblingIndices;

        if (previousSiblingIndices.Count == 0)
        {
            // First shuffle, no need to compare with previous indices
            newSiblingIndices = availableSiblingIndices.OrderBy(x => UnityEngine.Random.value).ToList();
        }
        else
        {
            do
            {
                // Shuffle the available sibling indices
                newSiblingIndices = availableSiblingIndices.OrderBy(x => UnityEngine.Random.value).ToList();
            } while (HasMatchingIndices(newSiblingIndices));
        }

        // Reorder the child GameObjects in the Horizontal Layout Group
        for (int i = 0; i < playersList.Count; i++)
        {
            //Debug.Log(newSiblingIndices[i]);
            playersList[i].SetSiblingIndex(newSiblingIndices[i]);
        }

        // Clear the previousSiblingIndices list
        previousSiblingIndices.Clear();

        // Update the previousSiblingIndices list with the new sibling indices
        foreach (var player in playersList)
        {
            previousSiblingIndices.Add(player.GetSiblingIndex());

            if (player.GetComponent<PlayerController>() != null)
                player.GetComponent<PlayerController>().scoring.correct = false;
        }

        // Refresh the layout to apply the changes
        layoutGroup.SetLayoutHorizontal();
    }

    private bool HasMatchingIndices(List<int> newIndices)
    {
        // Check if any of the new indices match the previous indices
        for (int i = 0; i < newIndices.Count; i++)
        {
            if (previousSiblingIndices.Count > i && newIndices[i] == previousSiblingIndices[i])
            {
                return true;
            }
        }
        return false;
    }

    public void BackToWebpage()
    {
        ExternalCaller.BackToHomeUrlPage();
    }


}
