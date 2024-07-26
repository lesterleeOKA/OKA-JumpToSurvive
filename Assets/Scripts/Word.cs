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
    public AudioSource rockEffect;
    public Image rockImg;


    public void setWord(string _word)
    {
        if(this.word != null) {
            //if(this.rockImg != null) this.rockImg.color = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 255);
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
        if(this.rockEffect != null)
        {
            if(status)
            {
                this.rockEffect.loop = true;
                this.rockEffect.Play();
            }
            else
            {
                this.rockEffect.Stop();
            }
        }
    }

    void MoveRock()
    {
        if (rockRigidbody == null) return;
        this.allowMove = true;
        this.rockRigidbody.isKinematic = false;
        /*Vector2 direction = (endPosition - startPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x);
        // Increase the angle of the projectile
        angle += Mathf.Deg2Rad * 30f; // Increase the angle by 30 degrees

        // Calculate the force magnitude with a lower value
        this.forceMagnitude = Random.Range(300f, 400f); // Decrease the force magnitude

        // Calculate the force vector
        Vector2 force = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * this.forceMagnitude;
        this.rockRigidbody.AddRelativeForce(force);*/

    }

    void FixedUpdate()
    {
        if (rockRigidbody == null || !allowMove) return;

        if (wheel != null)
        {
            wheel.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }

        var distance = Vector2.Distance(rectTransform.localPosition, endPosition);
        //Debug.Log(distance);

        if (distance < this.checkEndDistance)
        {
            this.resetPosition();
        }
        else
        {
            this.rectTransform.localPosition = Vector2.MoveTowards(this.rectTransform.localPosition, endPosition, this.forceMagnitude * Time.deltaTime);
        }

        if(AudioController.Instance.audioStatus != this.rockEffect.enabled)
        {
            this.setAudioEffect(AudioController.Instance.audioStatus);
        }
    }

    public void resetPosition()
    {
        if (this.rectTransform == null) return;
        //if (this.rockImg != null) this.rockImg.color = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 255);
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
        //Debug.Log(collision.collider.tag);
        if (collision.collider.tag.Contains("Ground"))
        {
            this.col.isTrigger = true;
            this.rockRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | this.rockRigidbody.constraints;
        }
    }
}
