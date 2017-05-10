using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;

public class Gunner : MonoBehaviour {
    public GameObject cannon;
    public GameObject cannonPivot;
    private int angleCurrent;

    // for gunner rotation
    float oldRotation;
    public float gunnerRotateSpeed = 1.0f;

    // for main gunner and active reload
    public GameObject tankTop;
    public GameObject launcher;

    private Rockets rockets = null;

    public enum AmmoTypes { Default, HighExplosive, ArmorPiercing };
    private int selectedAmmo = (int)AmmoTypes.Default;

    public GameObject reloadPanel;

    // Weapon Swapping
    public GameObject ammoPanel;

    public GameObject abilityIcon;
    public Sprite[] abilityIcons = new Sprite[3];

    private bool reloading = false;
    private bool attemptedReload = false;
    public Slider activeReload;

    // for ammo
    private float reloadSpeed = 0.01f;
    private List<string> currentCombo = new List<string>();

    private List<string> standard_shot_combo = new List<string> { "Button A", "Button Y", "Button X", "Button X" };
    public GameObject[] standard_shot_buttons = new GameObject[4];

    private List<string> he_shot_combo = new List<string> { "Button Y", "Button B", "Button X", "Button A" };
    public GameObject[] he_shot_buttons = new GameObject[4];

    private List<string> ap_shot_combo = new List<string> { "Button A", "Button A", "Button X", "Button Y" };
    public GameObject[] ap_shot_buttons = new GameObject[4];

    private Dictionary<AmmoTypes, List<string>> ammoCombos = new Dictionary<AmmoTypes, List<string>>();
    private Dictionary<List<string>, GameObject[]> comboButtons = new Dictionary<List<string>, GameObject[]>();

    // for UI
    // http://www.thegamecontriver.com/2014/10/create-sliding-pause-menu-unity-46-gui.html
    public GameObject gunnerPanel;
    private Animator anim;

    private GameObject inputMngr;
    private PlayerRoles playerRoles;
    public PlayerID playerID;

    public GameObject playerIcon;

    // Use this for initialization
    void Start () {
        oldRotation = tankTop.transform.parent.transform.localEulerAngles.y;

        // pull in rockets script
        rockets = GetComponent<Rockets>();

        // init ammo swap
        ammoCombos.Add(AmmoTypes.Default, standard_shot_combo);
        ammoCombos.Add(AmmoTypes.HighExplosive, he_shot_combo);
        ammoCombos.Add(AmmoTypes.ArmorPiercing, ap_shot_combo);

        comboButtons.Add(standard_shot_combo, standard_shot_buttons);
        comboButtons.Add(ap_shot_combo, ap_shot_buttons);
        comboButtons.Add(he_shot_combo, he_shot_buttons);

        // init
        anim = gunnerPanel.GetComponent<Animator>();
        anim.enabled = true;

        inputMngr = GameObject.Find("InputManager");
        playerRoles = inputMngr.GetComponent<PlayerRoles>();

        playerRoles.HidePanel(anim, ammoPanel, reloadPanel);
        playerRoles.SetComboTextures(comboButtons);

        playerID = inputMngr.GetComponent<PlayerRoles>().gunner;

        abilityIcon.GetComponent<Image>().sprite = abilityIcons[selectedAmmo];
    }
	
	// Update is called once per frame
	void Update () {
        playerIcon.GetComponent<Image>().sprite = playerRoles.helmets[(int)playerID];

        #region tank top rotation

        float newRotation = tankTop.transform.parent.transform.localEulerAngles.y;
        float test = oldRotation - newRotation;

        tankTop.transform.Rotate(0f, test, 0f);

        if (InputManager.GetAxis("Right Stick Horizontal", playerID) != 0.0f)
        {
            tankTop.transform.Rotate(0f, (InputManager.GetAxis("Right Stick Horizontal", playerID) * gunnerRotateSpeed), 0f);
        }

        oldRotation = tankTop.transform.parent.transform.localEulerAngles.y;
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
            if (InputManager.GetAxis("Right Trigger", playerID) == 1)
            {
                // ToDo: ammotype needs to be implemented
                rockets.FireProjectile(selectedAmmo);
                reloading = true;

                playerRoles.DisplayPanel(anim, reloadPanel);
                launcher.GetComponent<LineRenderer>().enabled = false;
            }
        }

        else if (reloading)
        {
            activeReload.value += reloadSpeed;

            if (InputManager.GetAxis("Left Trigger", playerID) > 0 && !attemptedReload)
            {

                if (activeReload.value >= 0.45f && activeReload.value <= 0.55f)
                {
                    activeReload.value = 0;
                    reloading = false;

                    playerRoles.HidePanel(anim, reloadPanel);
                    launcher.GetComponent<LineRenderer>().enabled = true;
                }
                else
                {
                    attemptedReload = true;
                }
            }
            if (activeReload.value >= 1) // adding half the width of line rail should make this dynamic
            {
                activeReload.value = 0; ;
                reloading = false;
                attemptedReload = false;

                playerRoles.HidePanel(anim, reloadPanel);
                launcher.GetComponent<LineRenderer>().enabled = true;
            }
        }

        #endregion

        #region weapon swapping
        if (InputManager.GetButtonDown("Left Bumper", playerID))
        {
            // display the options when pushing left bumper
            playerRoles.DisplayPanel(anim, ammoPanel);
            currentCombo = new List<string>();
        }

        else if (InputManager.GetButtonUp("Left Bumper", playerID) || currentCombo.Count == 4)
        {
            if (currentCombo.Count == 4)
            {
                selectedAmmo = playerRoles.SelectAmmo(currentCombo, ammoCombos);
                abilityIcon.GetComponent<Image>().sprite = abilityIcons[selectedAmmo];
                currentCombo = new List<string>();
            }

            // display the options when pushing left bumper
            playerRoles.HidePanel(anim, ammoPanel);
            playerRoles.ResetCombo(comboButtons);
        }

        if (InputManager.GetButton("Left Bumper", playerID))
        {
            // Add buttons to the current combo
            if (InputManager.GetButtonDown("Button A", playerID))
            {
                currentCombo.Add("Button A");
                playerRoles.DisplayCombo(currentCombo, comboButtons);
            }
            else if (InputManager.GetButtonDown("Button B", playerID))
            {
                currentCombo.Add("Button B");
                playerRoles.DisplayCombo(currentCombo, comboButtons);
            }
            else if (InputManager.GetButtonDown("Button X", playerID))
            {
                currentCombo.Add("Button X");
                playerRoles.DisplayCombo(currentCombo, comboButtons);
            }
            else if (InputManager.GetButtonDown("Button Y", playerID))
            {
                currentCombo.Add("Button Y");
                playerRoles.DisplayCombo(currentCombo, comboButtons);
            }
        }

        #endregion

        if (InputManager.GetAxis("DPAD Horizontal", playerID) == 1)
        {
            playerRoles.SwapToEngineer(this);
        }
        else if (InputManager.GetAxis("DPAD Vertical", playerID) == -1)
        {
            playerRoles.SwapToDriver(this);
        }
        else if (InputManager.GetAxis("DPAD Horizontal", playerID) == -1)
        {
            playerRoles.SwapToCommander(this);
        }
    }
}
