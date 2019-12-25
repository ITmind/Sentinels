using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowHelper : MonoBehaviour {

    public Image text;
    Renderer r;
	// Use this for initialization
	void Start () {
        r = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (r.isVisible)
        {
            //Debug.Log(gameObject);
        }
        else
        {
            //Debug.Log("invisible");
        }
        var sp = Camera.main.WorldToScreenPoint(transform.position);
        text.transform.position = new Vector3(sp.x,sp.y);
	}
}
