using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObjects : MonoBehaviour {

    [HideInInspector] public Camera cam; //The main camera.
    [HideInInspector] public ScriptManager scriptManager; //A script that holds all the other scripts, for easy reference.
    [HideInInspector] public bool clickTime; //A bool that is set and unset so that the clicking functions don't occur every frame.
    [HideInInspector] public bool holdingSelf; //True when the player is holding themselves. I might move this later. 
     public TalkToObject talkToObject; // the current dialogue script being referenced
    [HideInInspector] public bool isTalking; //True when a conversation is ongoing.

    void Start () //Grabs necessary references.
    {
        scriptManager = GameObject.FindWithTag("ScriptManager").GetComponent<ScriptManager>();
        cam = scriptManager.GetComponentInChildren<Camera>();
        StartCoroutine(castRay());
    }
	
	IEnumerator castRay () //Essentially, this fires a raycast on mouseclick: if it hits an object, if that object is Holdable, or the Player, you'll be able to move that object around with the HoldObjectMovement script. If that object is dialogue, you can talk to it.
    {
        yield return new WaitUntil(() => Input.GetButton("Fire1") && clickTime == false && scriptManager.playerAnimator.canMove == true);
        StartCoroutine(clickTimer());
        RaycastHit2D hit = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Input.mousePosition));
        StartCoroutine(castRay());
        if (hit.transform.tag == "Player" && isTalking == false)
        {
            scriptManager.holdObjectMovement.heldObject = hit.transform.gameObject;
            scriptManager.holdObjectMovement.HoldObject();
            holdingSelf = true;
        }
        else if (hit.collider != null)
        {
            foreach (Transform child in hit.transform)
            {
                if (child.tag == "Holdable" && isTalking == false && holdingSelf == false)
                {
                    scriptManager.playerAnimator.StartHolding();
                    scriptManager.holdObjectMovement.heldObject = hit.transform.gameObject;
                    scriptManager.holdObjectMovement.HoldObject();
                }
                else if (child.tag == "Dialogue" && holdingSelf == false)
                {
                    talkToObject = hit.collider.GetComponent<TalkToObject>();
                    talkToObject.StartTalking(false);
                }
                else if (child.tag == "DialogueContinue" && talkToObject.talkToCollider.enabled == true && holdingSelf == false) //These objects can continue existing dialogue, but not start new ones. (i.e. text boxes)
                {
                    talkToObject.updateTextBox();
                }
                }
            }
    }

    public IEnumerator clickTimer () //Sets ClickTime, then unsets it after half a second. 
    {
        clickTime = true;
        yield return new WaitForSeconds(0.1f);
        clickTime = false;
    }

}
