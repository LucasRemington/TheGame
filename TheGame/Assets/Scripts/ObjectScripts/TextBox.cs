using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBox : MonoBehaviour {

    [HideInInspector] public ScriptManager scriptManager; //A script that holds all the other scripts, for easy reference.
    [HideInInspector] public GameObject textBox;

    void Awake () //Grabs necessary references
    {
        scriptManager = GameObject.FindWithTag("ScriptManager").GetComponent<ScriptManager>();
        textBox = GameObject.FindWithTag("Textbox");
    }

    void stopCollider () //turns off collider of the TalkToObject.
    {
        scriptManager.selectObjects.talkToObject.talkToCollider.enabled = false;
    }

    void TextBegin () //Called from animation when the text is supposed to appear.
    {
        scriptManager.selectObjects.talkToObject.updateTextBox();
        scriptManager.selectObjects.talkToObject.talkToCollider.enabled = true;
    }

    void TextEnd () //Called from animation when the text is supposed to end.
    {
        scriptManager.selectObjects.talkToObject.clearText();
        scriptManager.selectObjects.talkToObject.talkToCollider.enabled = false;
    }

    void DestroyTextBox () //Called from animation when the text box disappears.
    {
        Destroy(textBox);
    }
}
