using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingTargetDestroy : MonoBehaviour {

    [HideInInspector] public ScriptManager scriptManager; //A script that holds all the other scripts, for easy reference.
    [HideInInspector] public Vector3 currentMove;

    void Awake () //on start, grabs the appropriate references and grabs the appropriate coroutines.
    {
        DestroyAllOther();
        scriptManager = GameObject.FindWithTag("ScriptManager").GetComponent<ScriptManager>(); //Gets a reference to ScriptManager based on tag.
        currentMove = scriptManager.holdObjectMovement.moveToLocation;
        StartCoroutine(destroyOnTarget());
    }

    public void DestroyAllOther()
    {
        GameObject[] others = GameObject.FindGameObjectsWithTag("FlashingTarget"); //Get array of all objects with this tag
                                                                                
        foreach (GameObject go in others)
        {
            if (go != gameObject) //if the object with the tag isn't this one, destroy it.
            { 
                Destroy(go);
            }
        }
    }

    IEnumerator destroyOnTarget () //this destroys the object when the heldobject finishes moving towards its destination.
    {
        yield return new WaitUntil(() => scriptManager.holdObjectMovement.heldObjectMove == false);
        Destroy(this.gameObject);
    }
}
