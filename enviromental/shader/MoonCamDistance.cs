using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonCamDistance : MonoBehaviour {

    public Transform master;
    public float distance = 1000;
    public Camera ccam;
    // Update is called once per frame
   void LateUpdate () {
       // if (ccam == null)
        //{
            if (Camera.main != null) ccam = Camera.main;
            else if (Camera.current != null) ccam = Camera.current;
        //}

        if (ccam != null) {
            transform.position = ccam.transform.position + (-master.transform.forward * distance);
            transform.rotation = Quaternion.LookRotation(-master.forward,-master.forward);
        }
    }


}
