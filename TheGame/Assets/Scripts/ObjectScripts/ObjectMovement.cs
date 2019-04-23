using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour {

    [HideInInspector] public ScriptManager scriptManager; //A script that holds all the other scripts, for easy reference.
    [HideInInspector] public SpriteRenderer objSprRnd;
    [HideInInspector] public Rigidbody2D objRB2D;
    [HideInInspector] public Animator objAnim;
    [HideInInspector] public CapsuleCollider2D objCap2D;
    public GameObject[] moveToLocation; // The target location(s).
    [HideInInspector] public int currentTarget;
    public bool multipleTargetLocations;
    public bool xMoveOnly;
    public bool loopPatrol;
    public bool staticIfStationary;
    public float speed;
    public bool canMove; //True when the object can move around.
    [HideInInspector] public float checkX; //These two variables are 'approximations' of how close the object is to it's target: once it's close enouh, it'll stop.
    [HideInInspector] public float checkY;
    public int initialLayer;
    public float xVelocityCap; //Variable that 'caps' velocity once it goes over a certain threshhold: currently only applies to x axis. 

    void Awake()
    {
        scriptManager = GameObject.FindWithTag("ScriptManager").GetComponent<ScriptManager>(); //Gets a reference to ScriptManager based on tag.
        objRB2D = this.GetComponent<Rigidbody2D>();
        objSprRnd = this.GetComponent<SpriteRenderer>();
        objAnim = this.GetComponent<Animator>();
        objCap2D = this.GetComponent<CapsuleCollider2D>();
        initialLayer = this.gameObject.layer;
        StartCoroutine(objectMovement());
        StartCoroutine(checkIfHeld());
    }

    IEnumerator checkIfHeld ()
    {
        yield return new WaitUntil(() => scriptManager.holdObjectMovement.heldObject == this.gameObject);
        canMove = false;
        yield return new WaitUntil(() => scriptManager.holdObjectMovement.heldObject != this.gameObject);
        canMove = true;
        this.gameObject.layer = initialLayer;
        StartCoroutine(checkIfHeld());
    }

	IEnumerator objectMovement ()
    {
        yield return new WaitUntil(() => canMove == true);
        if (xMoveOnly == true && objRB2D.velocity.y > -1)
        {
            objRB2D.AddForce((moveToLocation[currentTarget].transform.position - this.transform.position) * speed);
            objRB2D.velocity = new Vector2(objRB2D.velocity.x, 0);
        }
        else if (xMoveOnly == false)
        {
            objRB2D.AddForce((moveToLocation[currentTarget].transform.position - this.transform.position) * speed);
        }
        if (objRB2D.velocity.x > xVelocityCap && scriptManager.holdObjectMovement.heldObject != this.gameObject) // This caps the velocity, unless the player is holding the object.
        {
            objRB2D.velocity = new Vector2(xVelocityCap, objRB2D.velocity.y);
        }
        else if (objRB2D.velocity.x < -xVelocityCap && scriptManager.holdObjectMovement.heldObject != this.gameObject) // This caps the velocity, unless the player is holding the object.
        {
            objRB2D.velocity = new Vector2(-xVelocityCap, objRB2D.velocity.y);
        }
        if (objRB2D.velocity.x < 0)
        {
            objSprRnd.flipX = true;
        }
        else if (objRB2D.velocity.x > 0)
        {
            objSprRnd.flipX = false;
        }
        objAnim.SetBool("Walking", true);
        checkXY();
        if (checkX < 1f && checkY < 1f || xMoveOnly == true && checkX < 1f)
        {
            if (multipleTargetLocations == false)
            {
                if (staticIfStationary == true)
                {
                    FreezeMovement();
                    objAnim.SetBool("Walking", false);
                }
            }
            else
            {
                if (currentTarget < moveToLocation.Length - 1)
                {
                    Debug.Log("increase target");
                    currentTarget++;
                    StartCoroutine(objectMovement());
                }
                else if (loopPatrol == true)
                {
                    Debug.Log("reset target");
                    currentTarget = 0;
                    StartCoroutine(objectMovement());
                }
            }
        }
        else
        {
            StartCoroutine(objectMovement());
        }
    }

    void UnfreezeMovement ()
    {
        objRB2D.constraints = RigidbodyConstraints2D.None;
        objRB2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        objRB2D.velocity = Vector3.zero;
        objRB2D.isKinematic = false;
    }

    void FreezeMovement ()
    {
        canMove = false;
        objRB2D.constraints = RigidbodyConstraints2D.FreezeAll;
        objRB2D.velocity = Vector3.zero;
        objRB2D.isKinematic = true;
    }

    void checkXY() //This sets the approximate numbers that the location is judged against.
    {
        checkX = Mathf.Abs(moveToLocation[currentTarget].transform.position.x - this.transform.position.x);
        checkY = Mathf.Abs(moveToLocation[currentTarget].transform.position.y - this.transform.position.y);
    }
}
