using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MoveByPath : MonoBehaviour {

    public Transform[] trans;
    Vector3[] to;

    void OnEnable()
    {
        to = new Vector3[] { trans[0].position, trans[1].position, trans[2].position, trans[3].position };
    }

    // Use this for initialization
    void Start () {
        transform.DOPath(to, 5);
    }
	
}
