using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : GameBaseController
{
    public static GameController Instance = null;
    private List<int> previousSiblingIndices = new List<int>();
    public List<RectTransform> playersList = new List<RectTransform>();
    public HorizontalLayoutGroup layoutGroup;

    protected override void Awake()
    {
        if(Instance == null) Instance = this;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        //this.RandomlySortChildObjects();
    }

    public override void enterGame()
    {
        base.enterGame();
        for (int i = 0; i < this.playersList.Count; i++)
        {
            var playerController = this.playersList[i].GetComponent<PlayerController>();
            if (playerController != null)
            {
                if (i < this.playerNumber)
                {
                    if (i == 0 && LoaderConfig.Instance != null && LoaderConfig.Instance.apiManager.peopleIcon != null)
                    {
                        var _playerName = LoaderConfig.Instance?.apiManager.loginName;
                        var icon = SetUI.ConvertTextureToSprite(LoaderConfig.Instance.apiManager.peopleIcon as Texture2D);
                        playerController.updatePlayerIcon(true, _playerName, icon);
                    }
                    else
                    {
                        playerController.updatePlayerIcon(true);
                    }
                }
                else
                {
                    playerController.gameObject.SetActive(false);
                    playerController.updatePlayerIcon(false);
                }          
            }
        }
    }

    public override void endGame()
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
        base.endGame();
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
        float delay = 2f;
        int currentTime = Mathf.FloorToInt(((this.gameTimer.gameDuration - this.gameTimer.currentTime) / this.gameTimer.gameDuration) * 100);

        for (int i = 0; i < this.playersList.Count; i++)
        {
            var playerController = this.playersList[i].GetComponent<PlayerController>();
            if (playerController != null) {
                playerController.checkAnswer(currentTime);

                if(playerController.scoring.correct && !showCorrect) {
                    showCorrect = true;
                }
            }

        }

        AudioController.Instance?.PlayAudio(showCorrect ? 1 : 2);

        if (showCorrect) { 
            this.setGetScorePopup(true);
            yield return new WaitForSeconds(delay);
            this.setGetScorePopup(false);
        }
        else { 
            this.setWrongPopup(true);
            yield return new WaitForSeconds(delay);
            this.setWrongPopup(false);
        }
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
}
