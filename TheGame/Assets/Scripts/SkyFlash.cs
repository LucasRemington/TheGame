using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkyFlash : MonoBehaviour {

    public Image skyDimmer;

	public IEnumerator SkyFlasher ()
    {
        skyDimmer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        skyDimmer.color = Color.black;
    }
}
