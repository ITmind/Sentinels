using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour {

    Text text;
	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        text.text = "0";
	}
	
    public void ScoreChanged(int score)
    {
        text.text = score.ToString();
    }
	
}
