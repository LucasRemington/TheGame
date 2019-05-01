using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkToObject : MonoBehaviour {

    [HideInInspector] public ScriptManager scriptManager; //A script that holds all the other scripts, for easy reference.
    [HideInInspector] public Text text; //the text displayed in the text box.
    public Dialogue[] dialogue; //An array of Dialogue objects, which are the individual 'conversations' that can be held with each character. Each dialogue contains an array of DialogueItems,
                                //which contains the dialogue text the text box will display.
    [HideInInspector] public GameObject textBox; //The 'Container' for the canvas, text box image, and text.
    public GameObject textBoxPrefab; //The prefab the container is pulled from.
    [HideInInspector] public Animator textBoxAnim; //The animator on the text box image.
    [HideInInspector] public Animator selfAnim; //The animator on the object with the script.
    [HideInInspector] public RectTransform textBoxRT; //The RectTransform of the text box.
    [HideInInspector] public int currentDialogue = 0; //This variable keeps track of which dialogue we're pulling from when talking. (Again, the 'conversation.')
    [HideInInspector] public int currentDialogueItem = 0; //This keeps track of what specific part of the text the conversation is actually on.
    private string animateString;
    private bool cantPresstoProceed;
    private bool animatingText;
    public bool playerNear;
    [HideInInspector] public Collider2D talkToCollider;
    public float textBoxFloatDistance; //distance text box floats above object when AboveSelf is set in the dialogue element

    public void Awake()
    {
        scriptManager = GameObject.FindWithTag("ScriptManager").GetComponent<ScriptManager>(); //Gets a reference to ScriptManager based on tag.
        selfAnim = GetComponent<Animator>();
        talkToCollider = this.GetComponent<Collider2D>();
    }

    public void StartTalking (bool calledFromScript) //Called from SelectObject. Bool is true if called from script./
    {
        if (scriptManager.playerAnimator.canMove == true && playerNear == true || calledFromScript == true)
        {
            if (scriptManager.selectObjects.isTalking == false) //Instantiates the text box and then grabs the appropriate references. Also begins necessary coroutines.
            {
                Instantiate(textBoxPrefab);
                textBox = GameObject.FindWithTag("Textbox");
                text = textBox.transform.Find("Canvas").transform.Find("Textbox").transform.Find("Text").GetComponent<Text>();
                textBoxAnim = textBox.transform.Find("Canvas").transform.Find("Textbox").GetComponent<Animator>();
                textBoxRT = textBox.transform.Find("Canvas").transform.Find("Textbox").GetComponent<RectTransform>();
                locateTextBox();
                StartCoroutine(PressToProceed());
                scriptManager.selectObjects.isTalking = true;
                if (selfAnim != null && selfAnim.GetBool("Talking"))
                {
                    selfAnim.SetBool("Talking", true);
                }
            }
            else
            {
                updateTextBox(); //If the conversation has already started, this just calls updatetextbox.
            }
        }
    }

    public void updateTextBox () //Called when the text is going to proceed.
    {
        if (currentDialogueItem >= dialogue[currentDialogue].DialogItems.Count) //First, the text checks if there's more text. if there isn't, it stops the appropriate coroutines and starts the 
                //appropriate animations. The text box will destroy itself with an animation event when needed. 
        {
            if (currentDialogue < dialogue.Length - 1)
            {
                Debug.Log("dialog stay same");
                currentDialogue++;
            }
            currentDialogueItem = 0;
            if (textBoxAnim != null)
            {
                textBoxAnim.SetBool("End", true);
            }
            StopCoroutine(PressToProceed());
            StartCoroutine(turnOnCollider());
            StartCoroutine(waitToStopTalking());
        }
        else //If there's still more text to go, the script pulls the necessary info from the dialogue element, and calls locatetextbox to set the appropriate location.
        {
            text.font = dialogue[currentDialogue].DialogItems[currentDialogueItem].DialogueTextStyle.font;
            text.fontSize = dialogue[currentDialogue].DialogItems[currentDialogueItem].DialogueTextStyle.fontSize;
            text.fontStyle = dialogue[currentDialogue].DialogItems[currentDialogueItem].DialogueTextStyle.fontStyle;
            locateTextBox();
            animateString = "";
            StartCoroutine(AnimateText(dialogue[currentDialogue].DialogItems[currentDialogueItem].DialogueText));
            currentDialogueItem++;
        }
    }

    public void locateTextBox () //Sets the text box above the player, or in the specified location.
    {
        if (dialogue[currentDialogue].DialogItems[currentDialogueItem].isAboveSelf == true && textBoxRT != null)
        {
            textBoxRT.SetPositionAndRotation(new Vector3(this.transform.position.x, this.transform.position.y + textBoxFloatDistance, 0), Quaternion.identity);
        }
        else if (dialogue[currentDialogue].DialogItems[currentDialogueItem].isAboveCharacter == true && textBoxRT != null)
        {
            textBoxRT.SetPositionAndRotation(new Vector3 (scriptManager.playerAnimator.playerPosition.x, scriptManager.playerAnimator.playerPosition.y +5 , 0), Quaternion.identity);
        }
        else if (textBoxRT != null)
        {
            textBoxRT.SetPositionAndRotation(dialogue[currentDialogue].DialogItems[currentDialogueItem].textBoxLocation, Quaternion.identity);
        }
    }

    IEnumerator waitToStopTalking ()
    {
        yield return new WaitForSeconds(0.1f);
        scriptManager.selectObjects.isTalking = false;
        if (selfAnim != null)
        {
            selfAnim.SetBool("Talking", false);
        }
    }

    IEnumerator AnimateText(string strComplete) //This method makes text display one letter at a time. Looks nicer.
    {
        if (animatingText == false)
        {
            int i = 0;
            animateString = "";
            animatingText = true;
            talkToCollider.enabled = false;
            while (i < strComplete.Length)
            {
                animateString += strComplete[i++];
                if (text.text != null)
                {
                    text.text = animateString;
                }
                yield return new WaitForSeconds(0.01f);
            }
            Debug.Log("turn collider on from animate text");
            talkToCollider.enabled = true;
            animatingText = false;
        }
    }
     
    IEnumerator turnOnCollider ()
    {
        yield return new WaitForSeconds(0.5f);
        scriptManager.selectObjects.talkToObject.talkToCollider.enabled = true;
    }

    IEnumerator PressToProceed() //This coroutine allows you to press w or s to advance dialogue. 
    {
        yield return new WaitUntil(() => Input.GetButton("Vertical") && scriptManager.selectObjects.isTalking == true && cantPresstoProceed == false && talkToCollider.enabled == true);
        cantPresstoProceed = true;
        clearText();
        updateTextBox();
        yield return new WaitForSeconds(0.25f);
        cantPresstoProceed = false;
        if (currentDialogueItem <= dialogue[currentDialogue].DialogItems.Count)
        {
            StartCoroutine(PressToProceed());
        }
    }

    public void clearText() //Clears the text.
    {
        text.text = "";
    }

}
