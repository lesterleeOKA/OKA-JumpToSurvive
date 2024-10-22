using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class NumberCounter : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public int CountFPS = 30;
    public float Duration = 1f;
    public string NumberFormat = "N0";
    public string Unit = "";
    private int _value;
    public AudioSource scoringEffect;
    public Color textColor = default;
    public bool isAnimatedColor = true;

    public int Value
    {
        get { return _value; }
        set { UpdateText(value); _value = value; }
    }

    private Coroutine CountingCoroutine;

    private void Awake()
    {
        if (this.Text == null)
        {
            Text = GetComponent<TextMeshProUGUI>();
            Text.color = this.textColor;
        }
    }

    private void UpdateText(int newValue)
    {
        StopAllCoroutines();
        StartCoroutine(CountText(newValue));
    }

    private IEnumerator CountText(int newValue)
    {
        if (this.isAnimatedColor) Text.color = Color.yellow;

        WaitForSeconds Wait = new WaitForSeconds(1f / CountFPS);

        int previousValue = _value;
        int stepAmount;

        if (newValue - previousValue < 0)
        {
            stepAmount = Mathf.FloorToInt((newValue - previousValue) / (CountFPS * Duration));
            // newValue = -20, previousValue = 0. CountFPS = 30, and Duration = 1; (-20- 0) / (30*1) // -0.66667 (ceiltoint)-> 0
        }
        else
        {
            stepAmount = Mathf.CeilToInt((newValue - previousValue) / (CountFPS * Duration));
            // newValue = 20, previousValue = 0. CountFPS = 30, and Duration = 1; (20- 0) / (30*1) // 0.66667 (floortoint)-> 0
        }

        if (previousValue < newValue)
        {
            while (previousValue < newValue)
            {
                previousValue += stepAmount;
                if (previousValue > newValue)
                {
                    previousValue = newValue;
                }

                Text.SetText(previousValue.ToString(NumberFormat) + this.Unit);
                if (previousValue % 2 == 0)
                {
                    // Play scoring effect for even value
                    if (this.scoringEffect != null && AudioController.Instance.audioStatus) this.scoringEffect.Play();
                }
                yield return Wait;
            }
        }
        else
        {
            while (previousValue > newValue)
            {
                previousValue += stepAmount; // (-20 - 0) / (30 * 1) = -0.66667 -> -1              0 + -1 = -1
                if (previousValue < newValue)
                {
                    previousValue = newValue;
                }

                Text.SetText(previousValue.ToString(NumberFormat) + this.Unit);
                if (previousValue % 2 == 0)
                {
                    // Play scoring effect for even value
                    if (this.scoringEffect != null && AudioController.Instance.audioStatus) this.scoringEffect.Play();
                }
                yield return Wait;
            }
        }

        Text.color = this.textColor;
    }
}
