using UnityEngine;
using System.Collections;

public class CameraFacing : MonoBehaviour {

    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back,
             Camera.main.transform.rotation * Vector3.up);
     
        //Debug.Log(transform.rotation.eulerAngles);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);//.SetEulerAngles(transform.rotation.eulerAngles);

        //transform.rotation = new Quaternion(old.x, transform.rotation.y, old.z,1f);
    }
}
