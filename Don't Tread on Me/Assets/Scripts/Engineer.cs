using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;

public class Engineer : MonoBehaviour {
    public bool amEngineer;

    public Rigidbody defaultAmmo = null;
    public Rigidbody mineAmmo = null;
    public Rigidbody magnetAmmo = null;
    public Rigidbody slowAmmo = null;

    // For Grenade aiming
    public GameObject target;
    public GameObject reticle;
    private Vector3 oldTargetPosition;
    public float translateSpeed = 10.0f;
    bool snapBack = true;
    public float aimDistance;

    // Reload Time
    float reloadTime;
    private float timeLast = 0.0f;

    Camera mainCamera;
    private const float SPAWN_DISTANCE = 3.5f;

    public GameObject engineerPanel;
    private Animator anim;

    private GameObject inputMngr;
    private PlayerRoles playerRoles;
    public PlayerID playerID;

    public GameObject abilityIcon;
    public Sprite[] abilityIcons = new Sprite[4];

    public GameObject ammoPanel;

    public enum AmmoTypes { Default, Mine, Magnet, Slow };
    private int selectedAmmo = (int)AmmoTypes.Default;

    // for ammo
    private float reloadSpeed = 50.0f;
    private List<string> currentCombo = new List<string>();
    
    private List<string> standard_grenade_combo = new List<string> { "Button X", "Button A", "Button B", "Button Y" };
    public GameObject[] standard_grenade_buttons = new GameObject[4];
    
    private List<string> mine_grenade_combo = new List<string> { "Button A", "Button A", "Button Y", "Button B" };
    public GameObject[] mine_grenade_buttons = new GameObject[4];
    
    private List<string> magnet_grenade_combo = new List<string> { "Button B", "Button Y", "Button X", "Button Y" };
    public GameObject[] magnet_grenade_buttons = new GameObject[4];
    
    private List<string> slow_grenade_combo = new List<string> { "Button Y", "Button X", "Button Y", "Button X" };
    public GameObject[] slow_grenade_buttons = new GameObject[4];
    
    private Dictionary<AmmoTypes, List<string>> ammoCombos = new Dictionary<AmmoTypes, List<string>>();
    private Dictionary<List<string>, GameObject[]> comboButtons = new Dictionary<List<string>, GameObject[]>();

    public GameObject playerIcon;



    // Use this for initialization
    void Start () {

        aimDistance = 13f;
        
        reloadTime = 2f;

        // init ammo swap
        ammoCombos.Add(AmmoTypes.Default, standard_grenade_combo);
        ammoCombos.Add(AmmoTypes.Mine, mine_grenade_combo);
        ammoCombos.Add(AmmoTypes.Magnet, magnet_grenade_combo);
        ammoCombos.Add(AmmoTypes.Slow, slow_grenade_combo);
        
        comboButtons.Add(standard_grenade_combo, standard_grenade_buttons);
        comboButtons.Add(mine_grenade_combo, mine_grenade_buttons);
        comboButtons.Add(magnet_grenade_combo, magnet_grenade_buttons);
        comboButtons.Add(slow_grenade_combo, slow_grenade_buttons);


        // Init
        anim = engineerPanel.GetComponent<Animator>();
        anim.enabled = true;

        inputMngr = GameObject.Find("InputManager");
        playerRoles = inputMngr.GetComponent<PlayerRoles>();
        playerRoles.HidePanel(anim, ammoPanel);
        //playerRoles.SetComboTextures(comboButtons);

        playerID = inputMngr.GetComponent<PlayerRoles>().engineer;

        abilityIcon.GetComponent<Image>().sprite = abilityIcons[selectedAmmo];
    }
	
	// Update is called once per frame
	void Update () {

        if (playerID != inputMngr.GetComponent<PlayerRoles>().engineer) return;

        playerIcon.GetComponent<Image>().sprite = playerRoles.helmets[(int)playerID];

        if (amEngineer)
        {
            // Reticle follows the tank
            Vector3 follow = target.transform.position - oldTargetPosition;
            reticle.transform.Translate(follow, Space.World);

            oldTargetPosition = target.transform.position;

            // Hold to allow for Grenade aiming
            if(InputManager.GetAxis("Left Trigger", playerID) > 0)
            {
                reticle.SetActive(true);

                // sapBcks the reticle to tank position after pulling let tigger
                if (snapBack)
                {
                    reticle.transform.position = (target.transform.position + new Vector3(0,5.11f,0));
                    snapBack = false;
                }
               // if (Vector3.Distance(reticle.transform.position, target.transform.position) < aimDistance)
                
                    if (InputManager.GetAxis("Left Stick Vertical", playerID) != 0.0f)
                    {
                        reticle.transform.Translate(new Vector3(1, 0, 1) * -InputManager.GetAxis("Left Stick Vertical", playerID) * translateSpeed * Time.deltaTime, Space.World);

                        // Prevents player from moving reticle past a certain distance from tank. There is probably a better way to do this 
                        if (Vector3.Distance(reticle.transform.position, target.transform.position) >= aimDistance)
                        {
                            reticle.transform.Translate(new Vector3(1, 0, 1) * InputManager.GetAxis("Left Stick Vertical", playerID) * translateSpeed * Time.deltaTime, Space.World);
                        }

                    }
                    if (InputManager.GetAxis("Left Stick Horizontal", playerID) != 0.0f)
                    {
                        reticle.transform.Translate(new Vector3(-1, 0, 1) * -InputManager.GetAxis("Left Stick Horizontal", playerID) * translateSpeed * Time.deltaTime, Space.World);
                        
                        // Prevents player from moving reticle past a certain distance from tank. There is probably a better way to do this 
                        if (Vector3.Distance(reticle.transform.position, target.transform.position) >= aimDistance)
                        {
                            reticle.transform.Translate(new Vector3(-1, 0, 1) * InputManager.GetAxis("Left Stick Horizontal", playerID) * translateSpeed * Time.deltaTime, Space.World);
                        }
                    }

                if (InputManager.GetAxis("Right Trigger", playerID) > 0)
                {

                    if(Time.time - timeLast > reloadTime)
                    {
                        ThrowGrenade();
                        timeLast = Time.time;
                    }
                } 

            }

            else
            {
                reticle.SetActive(false);
                snapBack = true;
            } 
        }


        #region ammo swapping
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


        if (InputManager.GetAxis("DPAD Vertical", playerID) == 1)
        {
            playerRoles.SwapToGunner(this);
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


    void ThrowGrenade()
    {

        // Gets the direction from tank to reticle. May not be the most optimal way.
        Vector3 reticleTowards = reticle.transform.position - target.transform.position;
        float reticleDistance = reticleTowards.magnitude;
        Vector3 reticleDirection = reticleTowards / reticleDistance;

        if (selectedAmmo == (int)AmmoTypes.Default)
        {
            Rigidbody clone = Instantiate(defaultAmmo, target.transform.position + (SPAWN_DISTANCE * reticleDirection), target.transform.rotation) as Rigidbody;
            // sets velocity based on reticle distance from tank. May not be the most optimal way.
            clone.velocity = reticleDirection * (reticleDistance - 3.1f);
        }
        if (selectedAmmo == (int)AmmoTypes.Mine)
        {
            Rigidbody clone = Instantiate(mineAmmo, target.transform.position + (SPAWN_DISTANCE * reticleDirection) + new Vector3(0,-2,0), target.transform.rotation) as Rigidbody;
            
        }
        if (selectedAmmo == (int)AmmoTypes.Magnet)
        {
            Rigidbody clone = Instantiate(magnetAmmo, target.transform.position + (SPAWN_DISTANCE * reticleDirection), target.transform.rotation) as Rigidbody;
            clone.velocity = reticleDirection * (reticleDistance - 3.1f);
        }
        if (selectedAmmo == (int)AmmoTypes.Slow)
        {
            Rigidbody clone = Instantiate(slowAmmo, target.transform.position + (SPAWN_DISTANCE * reticleDirection), target.transform.rotation) as Rigidbody;
            clone.velocity = reticleDirection * (reticleDistance - 3.1f);
        }



        
    }


}
