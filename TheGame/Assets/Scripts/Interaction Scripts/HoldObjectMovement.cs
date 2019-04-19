using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldObjectMovement : MonoBehaviour {

    [HideInInspector] public GameObject heldObject; //This is the object the script moves: it's added from the SelectObject script. 
    [HideInInspector] public Rigidbody2D heldObjectRB2D; //Rigidbody of the selected object.
    [HideInInspector] public Animator heldObjectAnim; //Animator attached to the held object.
    public float thrust; //Modifier to the force applied to the object.
    [HideInInspector] public ScriptManager scriptManager; //A script that holds all the other scripts, for easy reference.

    void Start ()
    {
        scriptManager = GameObject.FindWithTag("ScriptManager").GetComponent<ScriptManager>(); //Gets a reference to ScriptManager based on tag.
    }
	
	void LateUpdate ()
    {
		if (heldObject != null) //If an object is being held, it checks whether the appropriate keys are being pressed to move it. If it isn't being moved, it sets its own velocity to zero.
        {
            if (((Input.GetButton("HorizontalArrows") && (Input.GetAxisRaw("HorizontalArrows") > 0) || (Input.GetButton("HorizontalLetters") && Input.GetButton("Jump") && (Input.GetAxisRaw("HorizontalLetters") > 0)))))
            {
                unfreezeHeldObject();
                heldObjectRB2D.AddRelativeForce(Vector3.right* thrust);
            }
            else if (((Input.GetButton("HorizontalArrows") && (Input.GetAxisRaw("HorizontalArrows") < 0) || (Input.GetButton("HorizontalLetters") && Input.GetButton("Jump") && (Input.GetAxisRaw("HorizontalLetters") < 0)))))
            {
                unfreezeHeldObject();
                heldObjectRB2D.AddRelativeForce(Vector3.left * thrust); ;
            }
            else if (((Input.GetButton("VerticalArrows") && (Input.GetAxisRaw("VerticalArrows") > 0) || (Input.GetButton("VerticalLetters") && Input.GetButton("Jump") && (Input.GetAxisRaw("VerticalLetters") > 0)))))
            {
                unfreezeHeldObject();
                heldObjectRB2D.AddRelativeForce(Vector3.up * thrust); 
            }
            else if (((Input.GetButton("VerticalArrows") && (Input.GetAxisRaw("VerticalArrows") < 0) || (Input.GetButton("VerticalLetters") && Input.GetButton("Jump") && (Input.GetAxisRaw("VerticalLetters") < 0)))))
            {
                unfreezeHeldObject();
                heldObjectRB2D.AddRelativeForce(Vector3.down * thrust); ;
            }
            else
            {
                heldObjectRB2D.constraints = RigidbodyConstraints2D.FreezeAll;
                heldObjectRB2D.velocity = Vector3.zero;
                heldObjectRB2D.isKinematic = true;
            }
        }
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
