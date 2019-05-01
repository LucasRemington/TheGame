using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningCutscene : MonoBehaviour {

    [HideInInspector] public ScriptManager scriptManager; //A script that holds all the other scripts, for easy reference.
    public ObjectMovement startingCameraPan;
    public Image blackScreen;
    public Image skyDimmer;
    public GameObject deathOBJ;
    public Animator graveAnim;
    [HideInInspector] public TalkToObject deathSpeak;
    [HideInInspector] public ObjectMovement deathMove;
    public Text titleText;
    public Image titleImage;

    void Start()
    {
        blackScreen.canvasRenderer.SetAlpha(1f);
        scriptManager = GameObject.FindWithTag("ScriptManager").GetComponent<ScriptManager>();
        deathSpeak = deathOBJ.GetComponent<TalkToObject>();
        deathMove = deathOBJ.GetComponent<ObjectMovement>();
        StartCoroutine(cutsceneFade());
        StartCoroutine(deathSpeaks());
        titleText.canvasRenderer.SetAlpha(0f);
        titleImage.canvasRenderer.SetAlpha(0f);
        scriptManager.selectObjects.talkToObject = deathSpeak;
    }

    IEnumerator cutsceneFade ()
    {
        blackScreen.CrossFadeAlpha(0, 3.0f, false);
        yield return new WaitForSeconds(6f);
        titleText.CrossFadeAlpha(1, 1.0f, false);
        yield return new WaitForSeconds(5f);
        titleText.CrossFadeAlpha(0, 1.0f, false);
        yield return new WaitForSeconds(5f);
        titleImage.CrossFadeAlpha(1, 2.0f, false);
        yield return new WaitForSeconds(6f);
        titleImage.CrossFadeAlpha(0, 1.0f, false);
    }

    IEnumerator deathSpeaks ()
    {
        yield return new WaitUntil(() => startingCameraPan.movementFlag == true);
        yield return new WaitForSeconds(1f);
        deathSpeak.StartTalking(true);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Input.GetButton("Fire1") || Input.GetButton("Jump"));
        deathSpeak.updateTextBox();
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Input.GetButton("Fire1") || Input.GetButton("Jump"));
        deathSpeak.updateTextBox();
        yield return new WaitForSeconds(1.5f);
        deathMove.canMove = true;
        yield return new WaitUntil(() => deathMove.movementFlag == true);
        Debug.Log("death end");
        deathOBJ.SetActive(false);
        skyDimmer.CrossFadeAlpha(0.5f, 2.0f, false);
        yield return new WaitForSeconds(2.5f);
        graveAnim.SetTrigger("Grave");
        yield return new WaitForSeconds(5f);
        graveAnim.SetTrigger("Grave");
        scriptManager.playerAnimator.playerAnim.SetTrigger("Start");
        skyDimmer.enabled = false;
    }

}
