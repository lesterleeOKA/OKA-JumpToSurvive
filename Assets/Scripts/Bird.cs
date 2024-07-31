using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bird : Item
{
    private BoxCollider2D col = null;
    public float checkEndDistance = 200f;
    public float speed = 200f;
    public void setWord(string _word)
    {
        if (this.word != null)
        {
            this.rectTransform = GetComponent<RectTransform>();
            this.col = GetComponent<BoxCollider2D>();
            this.rectTransform.localPosition = this.startPosition;
            this.endPosition = new Vector3(this.endPosition.x, this.startPosition.y);
            this.word.text = _word;
            this.Moving();
            this.setAudioEffect(true);
        }
    }

    void Moving()
    {
        this.allowMove = true;
    }

    void Update()
    {
        if (!allowMove) return;

        var distance = Vector2.Distance(rectTransform.localPosition, endPosition);
        if (distance < this.checkEndDistance)
        {
            this.resetPosition();
        }
        else
        {
            this.rectTransform.localPosition = Vector2.MoveTowards(this.rectTransform.localPosition, endPosition, this.speed * Time.deltaTime);
        }

        if (AudioController.Instance.audioStatus != this.objectEffect.enabled)
        {
            this.setAudioEffect(AudioController.Instance.audioStatus);
        }
    }

    public void resetPosition()
    {
        if (this.rectTransform == null) return;
        this.allowMove = false;
        this.col.isTrigger = false;
        this.rectTransform.localPosition = this.startPosition;
        this.setAudioEffect(false);
    }

    public void reTrigger()
    {
        this.Moving();
    }
}
