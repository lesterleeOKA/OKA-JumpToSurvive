using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Word : MonoBehaviour
{
    public float rotationSpeed = 360.0f; // Speed at which the wheel rotates (degrees per second)
    public float checkEndDistance = 200f;
    public RectTransform rectTransform;
    public TextMeshProUGUI word;
    public RectTransform wheel;
    public Vector2 startPosition;
    public Vector2 endPosition;
    public bool allowMove = false;
    private Rigidbody2D rockRigidbody;
    public float forceMagnitude = 500.0f; // Magnitude of the force applied to the rock


    public void setWord(string _word)
    {
        if(this.word != null) {
            rectTransform = GetComponent<RectTransform>();
            rockRigidbody = GetComponent<Rigidbody2D>();
            rockRigidbody.isKinematic = true;
            rectTransform.localPosition = this.startPosition;
            this.word.text = _word;
            this.MoveRock();
        }
    }

    void MoveRock()
    {
        if (rockRigidbody == null) return;
        this.allowMove = true;
        rockRigidbody.isKinematic = false;
        Vector2 direction = (endPosition - startPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x);
        //this.forceMagnitude = Random.Range(forceRange.x, forceRange.y);
        Vector2 force = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * this.forceMagnitude;
        // Apply the force in the object's local space
        rockRigidbody.AddRelativeForce(force);
    }

    void FixedUpdate()
    {
        if (rockRigidbody == null || !allowMove) return;
        /*float targetRotation = 0f;
        float rotationDamping = 3f; // Adjust this value to control the smoothness
        float currentRotation = rockRigidbody.rotation;
        float newRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * rotationDamping);
        rockRigidbody.MoveRotation(newRotation);*/

        if (wheel != null)
        {
            wheel.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }

        var distance = Vector2.Distance(rectTransform.localPosition, endPosition);
        //Debug.Log(distance);
        if (distance < this.checkEndDistance)
        {
            resetPosition();
        }
        else
        {
            rectTransform.localPosition = Vector2.MoveTowards(rectTransform.localPosition, endPosition, this.forceMagnitude * Random.Range(4f, 8f) * Time.deltaTime);
        }
    }

    public void resetPosition()
    {
        if (rectTransform == null) return;

        this.allowMove = false;
        rockRigidbody.velocity = Vector2.zero;
        rockRigidbody.angularVelocity = 0.0f;
        rockRigidbody.isKinematic = true; // Disable physics movement
        rectTransform.localPosition = this.startPosition;
    }

    public void reTrigger()
    {
        this.MoveRock();
    }
}
