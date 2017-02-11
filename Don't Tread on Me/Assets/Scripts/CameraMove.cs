using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    public int cameraSpeed = 10;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.I) || Input.GetButton("Y"))
        {
            transform.Translate(new Vector3(1, 0, 1) * cameraSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.K) || Input.GetButton("A"))
        {
            transform.Translate(new Vector3(-1, 0, -1) * cameraSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.J) || Input.GetButton("X"))
        {
            transform.Translate(new Vector3(-1, 0, 1) * cameraSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.L) || Input.GetButton("B"))
        {
            transform.Translate(new Vector3(1, 0, -1) * cameraSpeed * Time.deltaTime);
        }

    }
}
