using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance = null;
    private List<int> previousSiblingIndices = new List<int>();
    public List<RectTransform> playersList = new List<RectTransform>();

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }

    public HorizontalLayoutGroup layoutGroup;

    public void RandomlySortChildObjects()
    {
        // Create a list of available sibling indices (0 to playersList.Count - 1)
        List<int> availableSiblingIndices = Enumerable.Range(0, playersList.Count).ToList();

        // Ensure the new order does not match the previous order
        List<int> newSiblingIndices;

        if (previousSiblingIndices.Count == 0)
        {
            // First shuffle, no need to compare with previous indices
            newSiblingIndices = availableSiblingIndices.OrderBy(x => Random.value).ToList();
        }
        else
        {
            do
            {
                // Shuffle the available sibling indices
                newSiblingIndices = availableSiblingIndices.OrderBy(x => Random.value).ToList();
            } while (HasMatchingIndices(newSiblingIndices));
        }

        // Reorder the child GameObjects in the Horizontal Layout Group
        for (int i = 0; i < playersList.Count; i++)
        {
            Debug.Log(newSiblingIndices[i]);
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
}
