using UnityEngine;

public class Character : MonoBehaviour
{
    public delegate void CollidedWord(string word);
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
            Debug.Log(obj.tag);
            var wordObj = obj.GetComponent<Word>();
            wordObj.resetPosition();
            this.collidedWord?.Invoke(wordObj.word.text);
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
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
       // Debug.Log(collider.tag);
        if (collider.tag.Contains("Word"))
        {
            Debug.Log(collider.tag);
            var wordObj = collider.GetComponent<Word>();
            wordObj.resetPosition();
            this.collidedWord?.Invoke(wordObj.word.text);
        }
    }
}
