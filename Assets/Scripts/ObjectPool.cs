using UnityEngine;
using System.Collections;

public class ObjectPool : MonoBehaviour {

    public GameObject CollectCursorPerfab;
    public GameObject EnterCursorPerfab;
    public float TouchRadius = 1f;

    public static ObjectPool instance;

    void Start()
    {
        instance = this;
    }
    
}
