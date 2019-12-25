using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Earth : MonoBehaviour, IPointerClickHandler
{
    
    // Use this for initialization
    void Start () {
	
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        //print("look");
        //Camera.main.transform.LookAt(eventData.pointerPressRaycast.worldPosition);
        //throw new NotImplementedException();
    }

}
