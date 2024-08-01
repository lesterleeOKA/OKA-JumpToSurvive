using UnityEngine;

public class Character : MonoBehaviour
{
    public delegate void CollidedWord(Bird bird);
    public delegate void EnterGround(int id);
    public delegate void ExitGround(int id);
    public event CollidedWord collidedWord;
    public event EnterGround enterGround;
    public event ExitGround exitGround;
    public bool isGrounded = false;
    // Start is called before the first frame update
    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.collider.tag);
        if (collision.collider.tag.Contains("Ground"))
        {
            isGrounded = true;
            this.enterGround?.Invoke(0);
        }

        if (collision.collider.tag.Contains("Word"))
        {
            var obj = collision.collider;
            if (LogController.Instance != null) LogController.Instance.debug("collision:" + obj.tag);
            var wordObj = obj.GetComponent<Bird>();
            wordObj.setOutline(true);
            //wordObj.resetPosition();
            this.collidedWord?.Invoke(wordObj);
            this.enabled = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag.Contains("Ground"))
        {
            this.exitGround?.Invoke(1);
            isGrounded = false;
        }

        if (collision.collider.tag.Contains("Word"))
        {
            var obj = collision.collider;
            if (LogController.Instance != null) LogController.Instance.debug("collision:" + obj.tag);
            var wordObj = obj.GetComponent<Bird>();
            wordObj.setOutline(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
       // Debug.Log(collider.tag);
        if (collider.tag.Contains("Word"))
        {
            if (LogController.Instance != null) LogController.Instance.debug("collide:" + collider.tag);
            var wordObj = collider.GetComponent<Bird>();
            wordObj.setOutline(true);
            //wordObj.resetPosition();
            this.collidedWord?.Invoke(wordObj);
        }
    }
}
