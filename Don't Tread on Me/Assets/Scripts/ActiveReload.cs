using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ActiveReload : MonoBehaviour {

    //public Collider2D marker;
    //public Collider2D sweetSpot;
    //public GameObject activeReload;
    public GameObject marker;
    public GameObject sweetSpot;
    GameObject tank;
    rockets rockets;
    GameObject reloadOn;
    public bool reloading = true;
    private float spotWidth = 20f;
    private float markerWidth = 7f;
	// Use this for initialization
	void Start () {
        tank = GameObject.Find("f");
        rockets = tank.GetComponent<rockets>();
        reloadOn = GameObject.FindGameObjectWithTag("Reloading");
        reloadOn.SetActive(false);

	}

    // Update is called once per frame
    void Update()
    {
        if (reloading)
        {
            marker.transform.Translate(Vector3.right * Time.deltaTime * 50);

            if (Input.GetButtonDown("A")) {

                if (marker.transform.position.x < sweetSpot.transform.position.x + spotWidth &&
                   marker.transform.position.x + markerWidth > sweetSpot.transform.position.x )
                {
                    //print("Good Reload");
                    rockets.rocketTrue = true;
                    marker.transform.position = new Vector3(74f, 68.89f, 0);
                    reloading = false;
                    reloadOn.SetActive(false);
                } else 
                {
                    print("Reload Failed");
                }
            }
            if (marker.transform.position.x >= 220.0f)
            {
                //print("Reload Complete" + marker.transform.position.x + "      " + marker.transform.position.y);
                rockets.rocketTrue = true;
                marker.transform.position = new Vector3(74f, 68.89f, 0);
                reloading = false;
                reloadOn.SetActive(false);
            }
            

        }
        

    }

}
