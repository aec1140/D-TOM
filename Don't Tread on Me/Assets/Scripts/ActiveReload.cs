using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TeamUtility.IO;

public class ActiveReload : MonoBehaviour {

    private PlayerID gunner;
    private bool reloadAttempt = false;
    private float spotWidth = 20f;
    private float markerWidth = 7f;
    private Vector3 initialPos;

    private bool reloading = false;
    public GameObject marker;
    public GameObject sweetSpot;
    public GameObject lineRail;
    private GameObject reloadOn;
    public int reloadSpeed;

	// Use this for initialization
	void Start () {
        reloadOn = GameObject.FindGameObjectWithTag("Reloading");
        reloadOn.SetActive(false);
        reloadSpeed = 50;

        // initialize the icons position
        initialPos = marker.transform.position;

        // this code is for managing player roles
        GameObject inputMngr = GameObject.Find("InputManager");
        gunner = inputMngr.GetComponent<PlayerRoles>().gunner;
	}

    // Update is called once per frame
    void Update()
    {
        if (reloading)
        {
            marker.transform.Translate(Vector3.right * Time.deltaTime * reloadSpeed);

            if (InputManager.GetAxis("Left Trigger", gunner) > 0 && !reloadAttempt)
            {

                if (marker.transform.position.x < sweetSpot.transform.position.x + spotWidth - 10 &&
                   marker.transform.position.x + markerWidth > sweetSpot.transform.position.x)
                {
                    marker.transform.position = initialPos;
                    reloading = false;
                    reloadOn.gameObject.SetActive(false);
                }
                else
                {
                    reloadAttempt = true;
                    print("Reload Failed");
                    reloadSpeed = 25;
                }
            }
            if (marker.transform.position.x >= (lineRail.transform.position.x + 109)) // adding half the width of line rail should make this dynamic
            {
                marker.transform.position = initialPos;
                reloading = false;
                reloadOn.gameObject.SetActive(false);
                reloadAttempt = false;
                reloadSpeed = 50;
            }
        }
    }

    public void Reload()
    {
        reloadOn.gameObject.SetActive(true);
        reloading = true;
    }   

    public bool IsReloading()
    {
        return reloading;
    }

}
