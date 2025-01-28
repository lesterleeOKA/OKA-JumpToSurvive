using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : GameBaseController
{
    public static GameController Instance = null;
    public CharacterSet[] characterSets;
    public GameObject playerPrefab;
    public HorizontalLayoutGroup layoutGroup;
    public List<PlayerController> playerControllers = new List<PlayerController>();
    private List<int> previousSiblingIndices = new List<int>();

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

    void createPlayer()
    {
        for (int i = 0; i < this.maxPlayers; i++)
        {
            if (i < this.playerNumber)
            {
                var playerController = GameObject.Instantiate(this.playerPrefab, this.layoutGroup.transform).GetComponent<PlayerController>();
                playerController.gameObject.name = "Player_" + i;
                playerController.UserId = i;
                this.playerControllers.Add(playerController);
                this.playerControllers[i].Init(this.characterSets[i]);

                if (i == 0 && LoaderConfig.Instance != null && LoaderConfig.Instance.apiManager.peopleIcon != null)
                {
                    var _playerName = LoaderConfig.Instance?.apiManager.loginName;
                    var icon = SetUI.ConvertTextureToSprite(LoaderConfig.Instance.apiManager.peopleIcon as Texture2D);
                    this.playerControllers[i].UserName = _playerName;
                    this.playerControllers[i].updatePlayerIcon(true, _playerName, icon);
                }
                else
                {
                    var icon = SetUI.ConvertTextureToSprite(this.characterSets[i].defaultIcon as Texture2D);
                    this.playerControllers[i].updatePlayerIcon(true, null, icon);
                }
            }
            else
            {
                int notUsedId = i + 1;
                var notUsedPlayerIcon = GameObject.FindGameObjectWithTag("P" + notUsedId + "_Icon");
                if (notUsedPlayerIcon != null)
                {
                    var notUsedIcon = notUsedPlayerIcon.GetComponent<PlayerIcon>();

                    if (notUsedIcon != null)
                    {
                        notUsedIcon.HiddenIcon();
                    }
                }

                var notUsedPlayerController = GameObject.FindGameObjectWithTag("P" + notUsedId + "_controller").GetComponent<CanvasGroup>();
                if (notUsedPlayerController != null)
                {
                   SetUI.Set(notUsedPlayerController, false);
                }
            }
        }
    }

    public override void enterGame()
    {
        base.enterGame();
        this.createPlayer();      
    }

    public override void endGame()
    {
        QuestionController.Instance.killAllWords();
        bool showSuccess = false;
        for (int i = 0; i < this.playerControllers.Count; i++)
        {
            if (i < this.playerNumber)
            {
                var playerController = this.playerControllers[i];
                if (playerController != null)
                {
                    if (playerController.Score >= 30)
                    {
                        showSuccess = true;
                    }
                    this.endGamePage.updateFinalScore(i, playerController.Score);
                }
            }
        }
        this.endGamePage.setStatus(true, showSuccess);
        base.endGame();
    }

    public void showAllCharacterAnswer()
    {
        for (int i = 0; i < this.playerControllers.Count; i++)
        {
            var playerController = this.playerControllers[i];
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

        for (int i = 0; i < this.playerControllers.Count; i++)
        {
            var playerController = this.playerControllers[i];
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

        for (int i = 0; i < this.playerControllers.Count; i++)
        {
            var playerController = this.playerControllers[i];
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
        List<int> availableSiblingIndices = Enumerable.Range(0, this.playerControllers.Count).ToList();

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
        for (int i = 0; i < this.playerControllers.Count; i++)
        {
            //Debug.Log(newSiblingIndices[i]);
            this.playerControllers[i].GetComponent<RectTransform>().SetSiblingIndex(newSiblingIndices[i]);
        }

        // Clear the previousSiblingIndices list
        previousSiblingIndices.Clear();

        // Update the previousSiblingIndices list with the new sibling indices
        foreach (var player in this.playerControllers)
        {
            previousSiblingIndices.Add(player.GetComponent<RectTransform>().GetSiblingIndex());

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
