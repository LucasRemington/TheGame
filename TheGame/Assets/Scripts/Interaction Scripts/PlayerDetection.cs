using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour {

    [HideInInspector] public TalkToObject talkToObject; //Simple script. Passes the trigger references to the main talkToObject in order to not interfere with dialogue. Always put on child component.

	void Awake ()
    {
        talkToObject = GetComponentInParent<TalkToObject>();
	}

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            talkToObject.playerNear = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            talkToObject.playerNear = false;
        }
    }

}
