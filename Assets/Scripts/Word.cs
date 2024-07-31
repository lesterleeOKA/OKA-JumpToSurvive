using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Word : MonoBehaviour
{
    public float rotationSpeed = 360.0f;
    public float checkEndDistance = 200f;
    public RectTransform rectTransform;
    public TextMeshProUGUI word;
    public RectTransform wheel;
    public Vector2 startPosition;
    public Vector2 endPosition;
    public bool allowMove = false;
    private Collider2D col = null;
    private Rigidbody2D rockRigidbody = null;
    public float forceMagnitude = 500.0f; // Magnitude of the force applied to the rock
    public AudioSource objectEffect;
    public Image rockImg;


    public void setWord(string _word)
    {
        if(this.word != null) {
            this.rectTransform = GetComponent<RectTransform>();
            this.rockRigidbody = GetComponent<Rigidbody2D>();
            this.col = GetComponent<Collider2D>();
            this.rockRigidbody.isKinematic = true;
            this.rectTransform.localPosition = this.startPosition;
            this.word.text = _word;
            this.MoveRock();
            this.setAudioEffect(true);
        }
    }

    public void setAudioEffect(bool status)
    {
        if(this.objectEffect != null)
        {
            if(status)
            {
                this.objectEffect.loop = true;
                this.objectEffect.Play();
            }
            else
            {
                this.objectEffect.Stop();
            }
        }
    }

    void MoveRock()
    {
        if (rockRigidbody == null) return;
        this.allowMove = true;
        this.rockRigidbody.isKinematic = false;
    }

    void FixedUpdate()
    {
        if (rockRigidbody == null || !allowMove) return;

        if (wheel != null)
        {
            wheel.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }

        var distance = Vector2.Distance(rectTransform.localPosition, endPosition);
        if (distance < this.checkEndDistance)
        {
            this.resetPosition();
        }
        else
        {
            this.rectTransform.localPosition = Vector2.MoveTowards(this.rectTransform.localPosition, endPosition, this.forceMagnitude * Time.deltaTime);
        }

        if(AudioController.Instance.audioStatus != this.objectEffect.enabled)
        {
            this.setAudioEffect(AudioController.Instance.audioStatus);
        }
    }

    public void resetPosition()
    {
        if (this.rectTransform == null) return;
        this.allowMove = false;
        this.rockRigidbody.velocity = Vector2.zero;
        this.rockRigidbody.angularVelocity = 0.0f;
        this.rockRigidbody.isKinematic = true;
        this.col.isTrigger = false;
        this.rockRigidbody.constraints = rockRigidbody.constraints & ~RigidbodyConstraints2D.FreezePositionY;
        this.rectTransform.localPosition = this.startPosition;
        this.setAudioEffect(false);
    }

    public void reTrigger()
    {
        this.MoveRock();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag.Contains("Ground"))
        {
            this.col.isTrigger = true;
            this.rockRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | this.rockRigidbody.constraints;
        }
    }
}
