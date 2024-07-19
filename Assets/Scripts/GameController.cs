using System;
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
    public CanvasGroup GameUILayer, TopUILayer, EndGameLayer;

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }

    private void Start()
    {
        SetUI.Set(this.GameUILayer, true, 0f);
        SetUI.Set(this.TopUILayer, false, 0f);
        SetUI.Set(this.EndGameLayer, false, 0f);
        this.RandomlySortChildObjects();
    }

    public void enterGame(bool status)
    {
        if(!status) { 
            SetUI.Set(this.GameUILayer, false, 0f);
            if (AudioController.Instance != null) AudioController.Instance.changeBGMStatus(false);
        }
        SetUI.Set(this.TopUILayer, status, status ? 0.5f: 0f);
        SetUI.Set(this.EndGameLayer, !status, !status ? 0.5f : 0f);
    }

    public void resetPlayers()
    {
        StartCoroutine(randomlySortChildObjects());
    }

    public System.Collections.IEnumerator randomlySortChildObjects()
    {
        for (int i = 0; i < this.playersList.Count; i++)
        {
            var playerController = this.playersList[i].GetComponent<PlayerController>();
            if (playerController != null) {
                playerController.checkAnswer();
            }
        }
        yield return new WaitForSeconds(1f);
        this.RandomlySortChildObjects();
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

    public string GetCurrentDomainName
    {
        get
        {
            string absoluteUrl = Application.absoluteURL;
            Uri url = new Uri(absoluteUrl);
            Debug.Log("Host Name:" + url.Host);
            return url.Host;
        }
    }

    public void BackToWebpage()
    {
#if !UNITY_EDITOR
        string hostname = this.GetCurrentDomainName;

        if (hostname.Contains("dev.openknowledge.hk"))
        {
            string baseUrl = this.GetCurrentDomainName;
            string newUrl = $"https://{baseUrl}/RainbowOne/webapp/OKAGames/SelectGames/";
            Debug.Log("full url:" + newUrl);
            Application.ExternalEval($"location.href = '{newUrl}', '_self'");
        }
        else if (hostname.Contains("www.rainbowone.app"))
        {
            string Production = "https://www.starwishparty.com/";
            Application.ExternalEval($"location.href = '{Production}', '_self'");
        }
#endif
    }
}
