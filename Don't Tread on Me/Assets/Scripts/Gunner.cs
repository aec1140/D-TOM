using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

public class Gunner : MonoBehaviour {
    public bool amGunner;

    public GameObject cannon;
    public GameObject cannonPivot;
    private int angleCurrent;

    // for gunner rotation
    Quaternion oldRotation;
    public float gunnerRotateSpeed = 1.0f;

    // for main gunner and active reload
    public GameObject tankTop;
    public GameObject launcher;

    private rockets rockets = null;

    public GameObject marker;
    public GameObject sweetSpot;
    public GameObject lineRail;
    public GameObject reloadPanel;

    private bool reloading = false;
    private bool attemptedReload = false;
    private Vector3 initialPos;

    // for ammo
    private float reloadSpeed = 50.0f;

    // for UI
    // http://www.thegamecontriver.com/2014/10/create-sliding-pause-menu-unity-46-gui.html
    public GameObject gunnerPanel;
    private Animator anim;

    GameObject inputMngr;
    public PlayerID playerID;

    // Use this for initialization
    void Start () {
        oldRotation = tankTop.transform.rotation;

        // pull in rockets script
        rockets = GetComponent<rockets>();

        // init active reload
        initialPos = marker.transform.position;
        reloadPanel.gameObject.SetActive(false);

        // init UI
        anim = gunnerPanel.GetComponent<Animator>();
        anim.enabled = true;

        inputMngr = GameObject.Find("InputManager");
        playerID = inputMngr.GetComponent<PlayerRoles>().gunner;
    }
	
	// Update is called once per frame
	void Update () {

        if (playerID != inputMngr.GetComponent<PlayerRoles>().gunner) return;

        #region tank top rotation
        //turn Top of Tank to the right

        if (InputManager.GetAxis("Right Stick Horizontal", playerID) != 0.0f)
        {
            print("rotating");
            tankTop.transform.Rotate(0f, (InputManager.GetAxis("Right Stick Horizontal", playerID) * gunnerRotateSpeed), 0f);
            oldRotation = tankTop.transform.rotation;
        }
        #endregion

        #region cannon angle
        //point cannon up
        if (InputManager.GetAxis("Left Stick Vertical", playerID) < 0)
        {
            if (angleCurrent < 100)
            {
                cannon.transform.RotateAround(cannonPivot.transform.position, cannonPivot.transform.right, -10 * Time.deltaTime);
                angleCurrent++;
            }
        }

        //point cannon down
        if (InputManager.GetAxis("Left Stick Vertical", playerID) > 0)
        {
            if (angleCurrent > -80)
            {
                cannon.transform.RotateAround(cannonPivot.transform.position, cannonPivot.transform.right, 10 * Time.deltaTime);
                angleCurrent--;
            }
        }
        #endregion

        #region main gun
        if (!reloading)
        {
            print("Ready to Fire");
            // draw line
            // ToDo: this should be a raycast to help see what it is aiming at
            Vector3 forward = launcher.transform.TransformDirection(Vector3.forward) * 20;
            Debug.DrawRay(launcher.transform.position, forward, Color.red);

            if (InputManager.GetAxis("Right Trigger", playerID) == 1)
            {
                print("Firing");
                // ToDo: ammotype needs to be implemented
                rockets.FireProjectile(0);
                reloading = true;
                reloadPanel.gameObject.SetActive(true);

                // display reload UI
                anim.Play("panelSlideIn");
            }

            else
            {
                // hide UI
                anim.Play("panelSlideOut");
            }
        }

        else
        {
            marker.transform.Translate(Vector3.right * Time.deltaTime * reloadSpeed);

            if (InputManager.GetAxis("Left Trigger", playerID) > 0 && !attemptedReload)
            {

                if (marker.transform.position.x < sweetSpot.transform.position.x + 20.0f - 10 &&
                    marker.transform.position.x + 7.0f > sweetSpot.transform.position.x)
                {
                    marker.transform.position = initialPos;
                    reloading = false;
                    reloadPanel.gameObject.SetActive(false);
                }
                else
                {
                    attemptedReload = true;
                    print("Reload Failed");
                    reloadSpeed = 25;
                }
            }
            if (marker.transform.position.x >= (lineRail.transform.position.x + 109)) // adding half the width of line rail should make this dynamic
            {
                marker.transform.position = initialPos;
                reloading = false;
                reloadPanel.gameObject.SetActive(false);
                attemptedReload = false;
                reloadSpeed = 50;
            }
        }
        
        #endregion

        if (InputManager.GetAxis("DPAD Horizontal", playerID) == 1)
        {
            Commander commander = GetComponent<Commander>();
            commander.playerID = playerID;
            playerID = inputMngr.GetComponent<PlayerRoles>().commander;

            inputMngr.GetComponent<PlayerRoles>().commander = commander.playerID;
            inputMngr.GetComponent<PlayerRoles>().gunner = playerID;
        }
       else  if (InputManager.GetAxis("DPAD Vertical", playerID) == -1)
       {
            Driver driver = GetComponent<Driver>();
            driver.playerID = playerID;
            playerID = inputMngr.GetComponent<PlayerRoles>().driver;

            inputMngr.GetComponent<PlayerRoles>().driver = driver.playerID;
            inputMngr.GetComponent<PlayerRoles>().gunner = playerID;
        }
        else if (InputManager.GetAxis("DPAD Horizontal", playerID) == -1)
        {
            Engineer engineer = GetComponent<Engineer>();
            engineer.playerID = playerID;
            playerID = inputMngr.GetComponent<PlayerRoles>().engineer;

            inputMngr.GetComponent<PlayerRoles>().engineer = engineer.playerID;
            inputMngr.GetComponent<PlayerRoles>().gunner = playerID;
        }
    }
}
