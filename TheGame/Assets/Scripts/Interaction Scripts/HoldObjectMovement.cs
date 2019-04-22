using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldObjectMovement : MonoBehaviour {

    [HideInInspector] public GameObject heldObject; //This is the object the script moves: it's added from the SelectObject script. 
    [HideInInspector] public Rigidbody2D heldObjectRB2D; //Rigidbody of the selected object.
    [HideInInspector] public Animator heldObjectAnim; //Animator attached to the held object.
    [HideInInspector] public SpriteRenderer heldObjectSpriteRenderer; //Sprite Renderer attahced to the held object
    public float speed; //A multiplier for the speed that the object moves towards the target location.
    [HideInInspector] public ScriptManager scriptManager; //A script that holds all the other scripts, for easy reference.
    [HideInInspector] public Vector3 moveToLocation; // The target location.
    [HideInInspector] public bool heldObjectMove; //True when the held object can move around.
    [HideInInspector] public float checkX; //These two variables are 'approximations' of how close the object is to it's target: once it's close enouh, it'll stop.
    [HideInInspector] public float checkY;
    public GameObject flashingTarget; //A prefab instantiated to show the target location.

    void Start ()
    {
        scriptManager = GameObject.FindWithTag("ScriptManager").GetComponent<ScriptManager>(); //Gets a reference to ScriptManager based on tag.
    }

    void LateUpdate ()
    {
		if (heldObject != null) //If the player is holding an object..
        {
            if (Input.GetButton("Fire2") && heldObject != null && scriptManager.selectObjects.clickTime == false && scriptManager.selectObjects.isTalking == false) //...And the timer is right, and they press the right mouse button...
            {
                unfreezeHeldObject(); //...The object that they're holding can move, and the target position gets set.
                StartCoroutine(scriptManager.selectObjects.clickTimer());
                moveToLocation = scriptManager.selectObjects.cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -scriptManager.selectObjects.cam.transform.position.z));
                heldObjectMove = true; 
                Instantiate(flashingTarget, moveToLocation, Quaternion.identity); //This prefab is instantiated to show the target location. See the FlashingTargetDestroy script for more info.
                checkXY();
            }
            if (heldObject.transform.position != moveToLocation && heldObjectMove == true && checkX > 0.01f && checkY > 0.01f) //When these conditions are true, the object moves towards the target location.
            {
                unfreezeHeldObject();
                heldObjectRB2D.AddForce((moveToLocation - heldObject.transform.position) * speed);
                checkXY();
            }
            else //If the condition isn't being set or the object isn't moving, the object freezes in place.
            {
                freezeHeldObject();
            }

            if (moveToLocation.x < heldObject.transform.position.x)
            {
                heldObjectSpriteRenderer.flipX = true;
            }
            else
            {
                heldObjectSpriteRenderer.flipX = false;
            }

        }

    }

    void checkXY () //This sets the approximate numbers that the location is judged against.
    {
        checkX = Mathf.Abs(moveToLocation.x - heldObject.transform.position.x);
        checkY = Mathf.Abs(moveToLocation.y - heldObject.transform.position.y);
    }

    void freezeHeldObject () //Freezes the held object.
    {
        heldObjectMove = false;
        heldObjectRB2D.constraints = RigidbodyConstraints2D.FreezeAll;
        heldObjectRB2D.velocity = Vector3.zero;
        heldObjectRB2D.isKinematic = true;
    }
    
    void unfreezeHeldObject () //Function that allows the held object to move around.
    {
        heldObjectRB2D.constraints = RigidbodyConstraints2D.None;
        heldObjectRB2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        heldObjectRB2D.velocity = Vector3.zero;
        heldObjectRB2D.isKinematic = false;
    }

    public void HoldObject() //Called when an object is first held: grabs the appropriate component, sets gravity to nothing, and sets the objects animator to trigger.
    {
        heldObjectRB2D = heldObject.GetComponent<Rigidbody2D>();
        heldObjectAnim = heldObject.GetComponent<Animator>();
        heldObjectSpriteRenderer = heldObject.GetComponent<SpriteRenderer>();
        if (heldObjectAnim != null)
        {
            heldObjectAnim.SetBool("Flying", true);
        }
        heldObjectRB2D.isKinematic = false;
        heldObjectRB2D.gravityScale = 0;
    }

    public void DropHeldObject() //'Resets' a grabbed object, dumping all references. 
    {
        unfreezeHeldObject();
        heldObjectRB2D.gravityScale = 1;
        heldObjectAnim.SetBool("Flying", false);
        heldObject = null;
        heldObjectRB2D = null;
        heldObjectAnim = null;
    }
}
