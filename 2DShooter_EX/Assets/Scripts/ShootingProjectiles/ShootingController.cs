using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// A class which controlls player aiming and shooting
/// </summary>
public class ShootingController : MonoBehaviour
{
    [Header("GameObject/Component References")]
    [Tooltip("The projectile to be fired.")]
    public GameObject projectilePrefab = null;
    public GameObject playerShield = null;
    public GameObject playerBomb = null;
    public Text modeText;

    [Tooltip("The transform in the heirarchy which holds projectiles if any")]
    public Transform projectileHolder = null;

    [Header("Input")]
    [Tooltip("Whether this shooting controller is controled by the player")]
    public bool isPlayerControlled = false;

    [Header("Firing Settings")]
    [Tooltip("The minimum time between projectiles being fired.")]
    public float fireRate = 0.05f;
    public float fireRateShield = 4f;
    public float fireRateBomb = 5f;
    public float shieldDuration = 1f;
    public float bombDuration = 0.5f;
    private float savedFireRate;

    [Tooltip("The maximum diference between the direction the" +
        " shooting controller is facing and the direction projectiles are launched.")]
    public float projectileSpread = 1.0f;

    // The last time this component was fired
    private float lastFired = Mathf.NegativeInfinity;

    [Tooltip("The amount of time between shots for On/Off toggles. Only affects enemies with the Delayed modifier.")]
    public float waitTime = 3f;

    public bool looksAtPlayer = false;

    [Header("Effects")]
    [Tooltip("The effect to create when this fires")]
    public GameObject fireEffect;
    public GameObject fireEffectShield;
    public GameObject fireEffectBomb;

    //The input manager which manages player input
    private InputManager inputManager = null;

    public int weaponID = 0;

    public bool isPlayer = false;
    private bool shieldSummoned;
    private bool bombSummoned;
    public bool isExplosiveRock = false;

    /// <summary>
    /// Description:
    /// Standard unity function that runs every frame
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void Update()
    {
        ProcessInput();
        if (looksAtPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 dir = player.transform.position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            }
        }
        if (Input.GetKeyDown(KeyCode.Z) && isPlayer && PlayerPrefs.GetInt("ToggleValue8") == 0)
        {
            SwapWeapon();
        }
    }

    /// <summary>
    /// Description:
    /// Standard unity function that runs when the script starts
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void Start()
    {
        SetupInput();
        savedFireRate = fireRate;

        if (isPlayer)
        {
            weaponID = 0;
            modeText.text = "Blaster Mode";
        }
    }

    /// <summary>
    /// Description:
    /// Attempts to set up input if this script is player controlled and input is not already correctly set up 
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void SetupInput()
    {
        if (isPlayerControlled)
        {
            if (inputManager == null)
            {
                inputManager = InputManager.instance;
            }
            if (inputManager == null)
            {
                Debug.LogError("Player Shooting Controller can not find an InputManager in the scene, there needs to be one in the " +
                    "scene for it to run");
            }
        }
    }

    /// <summary>
    /// Description:
    /// Reads input from the input manager
    /// Inputs:
    /// None
    /// Returns:
    /// void (no return)
    /// </summary>
    void ProcessInput()
    {
        if (isPlayerControlled)
        {
            if (inputManager.firePressed || inputManager.fireHeld)
            {
                Fire();
            }
        }   
    }

    /// <summary>
    /// Description:
    /// Fires a projectile if possible
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void Fire()
    {
        if (weaponID == 0 && (Time.timeSinceLevelLoad - lastFired) > fireRate) // Gun
        {
            SpawnProjectile();
            if (fireEffect != null) // Gun Fire Effect
            {
                Instantiate(fireEffect, transform.position, transform.rotation, null);
            }
            lastFired = Time.timeSinceLevelLoad;
        }
        if (weaponID == 1 && (Time.timeSinceLevelLoad - lastFired) > fireRateShield) // Bash Shield
        {
            if (!shieldSummoned)
            {
                SummonShield();
                if (fireEffectShield != null) // Shield Activate Effect
                {
                    Instantiate(fireEffectShield, transform.position, transform.rotation, null);
                }
            }
            lastFired = Time.timeSinceLevelLoad;
        }
        if (weaponID == 2 && (Time.timeSinceLevelLoad - lastFired) > fireRateBomb) // Bomb
        {
            if (!bombSummoned)
            {
                Explode();
                if (fireEffectBomb != null) // Bomb Fire Effect
                {
                    Instantiate(fireEffectBomb, transform.position, transform.rotation, null);
                }
            }
            lastFired = Time.timeSinceLevelLoad;
        }
    }

    public void SwapWeapon()
    {
       if (weaponID == 0)
        {
            weaponID = 1;
            modeText.text = "Shield Mode";
        }
       else if (weaponID == 1)
        {
            weaponID = 2;
            modeText.text = "Explosive Mode";
        }
       else if (weaponID == 2)
        {
            weaponID = 0;
            modeText.text = "Blaster Mode";
        }
    }

    public void ToggleFireOff()
    {
        fireRate = 1000;
        Invoke("ToggleFireOn", waitTime / 2);
    }

    public void ToggleFireOn()
    {
        fireRate = savedFireRate;
    }

    /// <summary>
    /// Description:
    /// Spawns a projectile and sets it up
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void SpawnProjectile()
    {
        // Check that the prefab is valid
        if (projectilePrefab != null)
        {
            // Create the projectile
            GameObject projectileGameObject = Instantiate(projectilePrefab, transform.position, transform.rotation, null);

            // Account for spread
            Vector3 rotationEulerAngles = projectileGameObject.transform.rotation.eulerAngles;
            rotationEulerAngles.z += Random.Range(-projectileSpread, projectileSpread);
            projectileGameObject.transform.rotation = Quaternion.Euler(rotationEulerAngles);

            // Keep the heirarchy organized
            if (projectileHolder != null)
            {
                projectileGameObject.transform.SetParent(projectileHolder);
            }
        }
    }

    public void SummonShield()
    {
        shieldSummoned = true;
        playerShield.SetActive(true);
        GetComponent<Health>().isAlwaysInvincible = true;
        Invoke("DisableShield", shieldDuration);
    }

    public void DisableShield()
    {
        shieldSummoned = false;
        GetComponent<Health>().isAlwaysInvincible = false;
        playerShield.SetActive(false);
    }

    public void Explode()
    {
        bombSummoned = true;
        playerBomb.SetActive(true);
        Invoke("DisableBomb", bombDuration);
    }

    public void DisableBomb()
    {
        bombSummoned = false;
        playerBomb.SetActive(false);
    }
}
