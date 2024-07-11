using UnityEngine;
using UnityEngine.UI;

public class Word : MonoBehaviour
{
    public float speed = 2.0f; // Speed at which the wheel moves
    public float rotationSpeed = 360.0f; // Speed at which the wheel rotates (degrees per second)
    public RectTransform rectTransform;
    public Text word;
    public RectTransform wheel;
    public Vector2 startPosition;
    public Vector2 endPosition;
    public bool allowMove = false;

    public void setWord(string _word)
    {
        if(this.word != null) {
            rectTransform = GetComponent<RectTransform>();
            this.allowMove = true;
            this.word.text = _word;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(rectTransform == null) return;

        if(this.allowMove) { 
            rectTransform.localPosition = Vector2.MoveTowards(rectTransform.localPosition, endPosition, speed * Time.deltaTime);
            this.wheel.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
            if (Vector2.Distance(rectTransform.localPosition, endPosition) < 0.1f)
            {
                rectTransform.localPosition = this.startPosition;
            }
        }
    }

    public void resetPosition()
    {
        if (rectTransform == null) return;
        rectTransform.localPosition = this.startPosition;
        this.allowMove = false;
    }
}
