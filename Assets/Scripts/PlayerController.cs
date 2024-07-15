using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int playerId = 0;
    public Sprite[] characterSprites;
    public Character character;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float gravityScale = 20f;
    private Rigidbody2D rb;

    void Start()
    {
        if(this.character != null) {  
            rb = this.character.GetComponent<Rigidbody2D>();
            this.character.enterGround += this.SetCharacterSprite;
            this.character.exitGround += this.SetCharacterSprite;
            this.character.collidedWord += this.TriggerWord;
        }
        this.SetCharacterSprite(0);
    }

    void FixedUpdate()
    {
        if(this.character == null || rb == null) return;
        // Get input from the player
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move the character
        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        movement = this.character.transform.TransformDirection(movement);
        this.character.transform.position += movement * moveSpeed * Time.deltaTime;

        // Jump if the player presses the jump button and is grounded
        if (Input.GetKeyDown("space"))
        {
            //Debug.Log("Jumped");
            Jump();
        }

        // Apply gravity
        rb.AddForce(Vector2.down * gravityScale);
    }

    public void Jump()
    {
        if(this.character.isGrounded && StartGame.Instance.startedGame) 
            this.rb.AddForce(Vector2.up * jumpForce);
    }

    


    void SetCharacterSprite(int id)
    {
        //Debug.Log(id);
        if(this.characterSprites[id] != null)
            this.character.gameObject.GetComponent<Image>().sprite = this.characterSprites[id];
    }

    void TriggerWord()
    {
        Debug.Log("word belong to player:" + this.playerId);
        QuestionController.Instance.randAnswer();
    }
}
