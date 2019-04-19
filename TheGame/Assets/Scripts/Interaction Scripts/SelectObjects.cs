using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObjects : MonoBehaviour {

    [HideInInspector] public Camera cam; //The main camera.
    [HideInInspector] public ScriptManager scriptManager; //A script that holds all the other scripts, for easy reference.
    [HideInInspector] public bool clickTime; //A bool that is set and unset so that the clicking functions don't occur every frame.
    [HideInInspector] public bool holdingSelf; //True when the player is holding themselves. I might move this later. 
    [HideInInspector] public TalkToObject talkToObject;

    void Start () //Grabs necessary references.
    {
        cam = this.GetComponent<Camera>();
        scriptManager = GameObject.FindWithTag("ScriptManager").GetComponent<ScriptManager>();
	}
	
	void Update () //Essentially, this fires a raycast on mouseclick: if it hits an object, if that object is Holdable, or the Player, you'll be able to move that object around with the HoldObjectMovement script. If that object is dialogue, you can talk to it.
    {
        if (Input.GetButton("Fire1") && clickTime == false)
        {
            StartCoroutine(clickTimer());
            RaycastHit2D hit = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Input.mousePosition));
            if (hit.transform.tag == "Player")
            {
                scriptManager.holdObjectMovement.heldObject = hit.transform.gameObject;
                scriptManager.holdObjectMovement.HoldObject();
                holdingSelf = true;
            }
            else if (hit.collider != null)
            {
                foreach (Transform child in hit.transform)
                {
                    if (child.tag == "Holdable")
                    {
                        scriptManager.playerAnimator.StartHolding();
                        scriptManager.holdObjectMovement.heldObject = hit.transform.gameObject;
                        scriptManager.holdObjectMovement.HoldObject();
                    } else if (child.tag == "Dialogue")
                    {
                        //ween
                    }
                }
            } 
        }
    }

    IEnumerator clickTimer () //Sets ClickTime, then unsets it after half a second. 
    {
        clickTime = true;
        yield return new WaitForSeconds(0.1f);
        clickTime = false;
    }

}
