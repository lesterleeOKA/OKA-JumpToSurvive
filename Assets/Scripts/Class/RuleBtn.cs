using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RuleBtn : MonoBehaviour
{
    public CanvasGroup ruleBox;
    public EventTrigger ruleBtnTrigger;
    public Button ruleBtnButton;
    public bool isOn = false;

    private void Start()
    {
        this.SetPopup(false);
    }
    public void SetPopup(bool status)
    {
        this.isOn = status;
        if(this.ruleBtnTrigger != null) {
            this.ruleBtnTrigger.enabled = !this.isOn;
        }
        if (this.ruleBtnButton != null)
        {
            this.ruleBtnButton.interactable = !this.isOn;
        }
        SetUI.Set(this.ruleBox, status);
    }
}
