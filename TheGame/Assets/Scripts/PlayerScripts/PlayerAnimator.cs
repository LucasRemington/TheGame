using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    [HideInInspector] public Animator playerAnim; //The animator attached to the player.
    [HideInInspector] public SpriteRenderer playerSprite; //The sprite renderer attached to the player.
    [HideInInspector] public Rigidbody2D playerRGBD; //The rigidbody2d attached to the player.
    [HideInInspector] public Vector3 playerPosition; //The current position of the player object.
    public float thrust; //Variable that multiplies force applied to the player.
    public float xVelocityCap; //Variable that 'caps' velocity once it goes over a certain threshhold: currently only applies to x axis. 
    [HideInInspector] public ScriptManager scriptManager; //A script that holds all the other scripts, for easy reference.
    public bool playerFalling;
    public bool fallTimerCheck;
    public int fallIncrement;
    public int isFalling;

    private void Start() //Grabs appropriate references.
    {
        playerAnim = this.GetComponent<Animator>();
        playerRGBD= this.GetComponent<Rigidbody2D>();
        playerSprite = this.GetComponent<SpriteRenderer>();
        scriptManager = GameObject.FindWithTag("ScriptManager").GetComponent<ScriptManager>();
    }

    void LateUpdate () //This moves the player, and sets the appropriate bools in the animator. 
    {
		if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") > 0 && scriptManager.selectObjects.holdingSelf == false && scriptManager.selectObjects.isTalking == false)
        {
            playerAnim.SetBool("Walking", true);
            playerSprite.flipX = false;
            playerRGBD.AddRelativeForce(Vector3.right * thrust);
        }
        else if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") < 0 && scriptManager.selectObjects.holdingSelf == false && scriptManager.selectObjects.isTalking == false)
        {
            playerAnim.SetBool("Walking", true);
            playerSprite.flipX = true;
            playerRGBD.AddRelativeForce(Vector3.left * thrust);
        }
        else
        {
            playerAnim.SetBool("Walking", false);
            playerRGBD.velocity = new Vector2(0, playerRGBD.velocity.y);
        }

        if (playerRGBD.velocity.y < 0)
        {
            if (fallTimerCheck == false && playerFalling == false)
            {
                StartCoroutine(fallTimer());
            }
            if (playerFalling == true)
            {
                playerAnim.SetBool("Falling", true);
            }
        }
        else
        {
            playerFalling = false;
            playerAnim.SetBool("Falling", false);
        }

        if (playerRGBD.velocity.x > xVelocityCap && scriptManager.selectObjects.holdingSelf == false) // This caps the velocity, unless the player is holding themselves.
        {
            playerRGBD.velocity = new Vector2 (xVelocityCap, playerRGBD.velocity.y);
        }

        if (playerRGBD.velocity.x < -xVelocityCap && scriptManager.selectObjects.holdingSelf == false) // This caps the velocity, unless the player is holding themselves.
        {
            playerRGBD.velocity = new Vector2(-xVelocityCap, playerRGBD.velocity.y);
        }

        if (Input.GetButton("Jump") && scriptManager.selectObjects.isTalking == false) //Pressing space causes the player to drop the object, unless they are talking.
        {
            scriptManager.holdObjectMovement.heldObjectMove = false;
            StopHolding();
        }

        playerPosition = this.transform.position; //updates the player position on every frame, which other scripts depend on.

    }

    public void StartHolding ()  // This only matters for setting the appropriate bool in the animator.
    {
        playerAnim.SetBool("Holding", true);
    }

    public void StopHolding () // This 'drops' the held object. See HoldObjectMovement.
    {
        scriptManager.selectObjects.holdingSelf = false;
        scriptManager.holdObjectMovement.DropHeldObject();
        playerAnim.SetBool("Holding", false);
    }

    public IEnumerator fallTimer () //currently fucked
    {
        fallTimerCheck = true;
        yield return new WaitForSeconds(0.05f);
        if (playerRGBD.velocity.y < 0)
        {
            fallIncrement++;
        }
        isFalling++;
        if (isFalling > 10)
        {
            if (fallIncrement >= 10)
            {
                playerFalling = true;
                fallTimerCheck = false;
            }
        }
        else
        {
            StartCoroutine(fallTimer());
        }
    }

}
