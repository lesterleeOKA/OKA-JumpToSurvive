using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent((typeof(TextMeshProUGUI)))]
public class InstructionText : MonoBehaviour
{
    public TextMeshProUGUI instructionText;
    public int lengthOfPixelX = 20;
    // Start is called before the first frame update

    public void setContent(string _content)
    {
        if(this.instructionText != null)
        {
            this.instructionText.text = _content;
            this.instructionText.GetComponent<RectTransform>().sizeDelta  = new Vector2(_content.Length * this.lengthOfPixelX, this.instructionText.GetComponent<RectTransform>().sizeDelta.y);  
        }
    }
}
