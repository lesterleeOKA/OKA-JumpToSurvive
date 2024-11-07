using UnityEngine;


public class Bird : Item
{
    private EdgeCollider2D col = null;
    public float checkEndDistance = 200f;
    public float speed = 200f;
    public SpriteRenderer birdSprite;
    public Material birdOutline;
    public int originalSortOrder = 10;

    public void setBirdTexture(string fileName, Sprite sprite = null)
    {
        if(sprite != null && this.birdSprite != null && fileName != this.birdSprite.sprite.name)
        {
            this.birdSprite.sprite = sprite;
            this.birdSprite.GetComponent<Animator>().enabled = false;
        }
    }

    public void setOutline(bool status)
    {
        if(this.birdOutline != null)
        {
            this.birdOutline.SetFloat("_OutlineThickness", status? 6f : 0f);
        }
    }
    public void setWord(string _word, int birdOrder, Material _birdOutline=null)
    {
        if (this.word != null)
        {
            if(_birdOutline != null) this.birdOutline = _birdOutline;
            if(this.birdSprite != null) {
                this.birdSprite.sortingOrder = this.originalSortOrder;
                this.birdSprite.material = this.birdOutline;
                this.birdSprite.sortingOrder += birdOrder;
            }
            this.rectTransform = GetComponent<RectTransform>();
            this.col = GetComponent<EdgeCollider2D>();
            this.rectTransform.localPosition = this.startPosition;
            this.endPosition = new Vector3(this.endPosition.x, this.startPosition.y);
            this.word.text = _word;
            this.allowMove = true;
            this.setAudioEffect(true);
            this.setOutline(false);
        }
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
            this.rectTransform.localPosition = Vector2.MoveTowards(this.rectTransform.localPosition, endPosition, this.speed * Time.smoothDeltaTime);
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
        //this.col.isTrigger = false;
        this.rectTransform.localPosition = this.startPosition;
        this.setAudioEffect(false);
        this.setOutline(false);
        if (this.birdSprite != null) this.birdSprite.sortingOrder = this.originalSortOrder;
    }

    public void reTrigger()
    {
        this.allowMove = true;
    }
}
