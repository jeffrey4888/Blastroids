using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// This class handles the health state of a game object.
/// 
/// Implementation Notes: 2D Rigidbodies must be set to never sleep for this to interact with trigger stay damage
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Team Settings")]
    [Tooltip("The team associated with this damage")]
    public int teamId = 0;

    [Header("Health Settings")]
    [Tooltip("The default health value")]
    public int defaultHealth = 1;
    [Tooltip("The maximum health value")]
    public int maximumHealth = 1;
    [Tooltip("The current in game health value")]
    public int currentHealth = 1;
    [Tooltip("Invulnerability duration, in seconds, after taking damage")]
    public float invincibilityTime = 3f;
    [Tooltip("Whether or not this health is always invincible")]
    public bool isAlwaysInvincible = false;
    [Tooltip("Whether or not this health drains over time")]
    public bool doesHealthDrain = false;
    [Tooltip("The intensity of health drain")]
    public int healthDrainAmount = 1;
    [Tooltip("How often health is drained")]
    public float drainTime;

    [Header("Lives settings")]
    [Tooltip("Whether or not to use lives")]
    public bool useLives = false;
    [Tooltip("Current number of lives this health has")]
    public int currentLives = 3;
    [Tooltip("The maximum number of lives this health can have")]
    public int maximumLives = 5;

    public bool isExplosive = false;
    public GameObject bomb;

    public bool isEnemy = false;
    public int enemyID;

    /// <summary>
    /// Description:
    /// Standard unity funciton called before the first frame update
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void Start()
    {
        SetRespawnPoint(transform.position);
        if (doesHealthDrain)
        {
            InvokeRepeating("HealthDrain", drainTime, drainTime);
        }
        if (teamId == 0 && PlayerPrefs.GetInt("ToggleValue7") == 1)
        {
            currentHealth = 1;
            maximumHealth = 1;
            defaultHealth = 1;
        }
        else
        {
            currentHealth = 3;
            maximumHealth = 3;
            defaultHealth = 3;
        }
        if (isEnemy)
        {
            if (PlayerPrefs.GetInt("ToggleValue1") == 1) // If the Strength checkbox is checked, strength will be true (1). If not, it is false (0).
            {
                // Unevovled Enemies
                if (enemyID == 0) //Single Shooter
                {
                    currentHealth = 10;
                    maximumHealth = 10;
                    defaultHealth = 10;
                }
                else if (enemyID == 1) //Diagonal Shooter
                {
                    currentHealth = 10;
                    maximumHealth = 10;
                    defaultHealth = 10;
                }
                else if (enemyID == 2) //Blink Shooter
                {
                    currentHealth = 6;
                    maximumHealth = 6;
                    defaultHealth = 6;
                }
                else if (enemyID == 3) //Missile Enemy
                {
                    currentHealth = 6;
                    maximumHealth = 6;
                    defaultHealth = 6;
                }
                else if (enemyID == 4) //Gunner Missle Enemy
                {
                    currentHealth = 10;
                    maximumHealth = 10;
                    defaultHealth = 10;
                }
                // Evolved Enemies
                else if (enemyID == 5) //Laser Shooter
                {
                    currentHealth = 10;
                    maximumHealth = 10;
                    defaultHealth = 10;
                }
                else if (enemyID == 6) //Quadrupler
                {
                    currentHealth = 10;
                    maximumHealth = 10;
                    defaultHealth = 10;
                }
                else if (enemyID == 7) //Circle Enemy
                {
                    currentHealth = 20;
                    maximumHealth = 20;
                    defaultHealth = 20;
                }
                else if (enemyID == 8) //Tower Enemy
                {
                    currentHealth = 20;
                    maximumHealth = 20;
                    defaultHealth = 20;
                }
                else if (enemyID == 9) //Summoner Enemy
                {
                    currentHealth = 30;
                    maximumHealth = 30;
                    defaultHealth = 30;
                }
                else if (enemyID == 10) //Black Hole
                {
                    currentHealth = 2500;
                    maximumHealth = 2500;
                    defaultHealth = 2500;
                }
            }
            else // Sets the enemy HP back to normal if the checkbox is checked, then unchecked by the player.
            {
                // Unevovled Enemies
                if (enemyID == 0) //Single Shooter
                {
                    currentHealth = 3;
                    maximumHealth = 3;
                    defaultHealth = 3;
                }
                else if (enemyID == 1) //Diagonal Shooter
                {
                    currentHealth = 3;
                    maximumHealth = 3;
                    defaultHealth = 3;
                }
                else if (enemyID == 2) //Blink Shooter
                {
                    currentHealth = 1;
                    maximumHealth = 1;
                    defaultHealth = 1;
                }
                else if (enemyID == 3) //Missile Enemy
                {
                    currentHealth = 1;
                    maximumHealth = 1;
                    defaultHealth = 1;
                }
                else if (enemyID == 4) //Gunner Missle Enemy
                {
                    currentHealth = 3;
                    maximumHealth = 3;
                    defaultHealth = 3;
                }
                // Evolved Enemies
                else if (enemyID == 5) //Laser Shooter
                {
                    currentHealth = 3;
                    maximumHealth = 3;
                    defaultHealth = 3;
                }
                else if (enemyID == 6) //Quadrupler
                {
                    currentHealth = 3;
                    maximumHealth = 3;
                    defaultHealth = 3;
                }
                else if (enemyID == 7) //Circle Enemy
                {
                    currentHealth = 5;
                    maximumHealth = 5;
                    defaultHealth = 5;
                }
                else if (enemyID == 8) //Tower Enemy
                {
                    currentHealth = 5;
                    maximumHealth = 5;
                    defaultHealth = 5;
                }
                else if (enemyID == 9) //Summoner Enemy
                {
                    currentHealth = 5;
                    maximumHealth = 5;
                    defaultHealth = 5;
                }
                else if (enemyID == 10) //Black Hole
                {
                    currentHealth = 5000;
                    maximumHealth = 5000;
                    defaultHealth = 5000;
                }
            }
        }
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once per frame
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void Update()
    {
        InvincibilityCheck();
    }

    // The specific game time when the health can be damged again
    private float timeToBecomeDamagableAgain = 0;
    // Whether or not the health is invincible
    private bool isInvincableFromDamage = false;

    /// <summary>
    /// Description:
    /// Checks against the current time and the time when the health can be damaged again.
    /// Removes invicibility if the time frame has passed
    /// Inputs:
    /// None
    /// Returns:
    /// void (no return)
    /// </summary>
    private void InvincibilityCheck()
    {
        if (timeToBecomeDamagableAgain <= Time.time)
        {
            isInvincableFromDamage = false;
        }
    }

    // The position that the health's gameobject will respawn at if lives are being used
    private Vector3 respawnPosition;
    /// <summary>
    /// Description:
    /// Changes the respawn position to a new position
    /// Inputs:
    /// Vector3 newRespawnPosition
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="newRespawnPosition">The new position to respawn at</param>
    public void SetRespawnPoint(Vector3 newRespawnPosition)
    {
        respawnPosition = newRespawnPosition;
    }

    /// <summary>
    /// Description:
    /// Repositions the health's game object to the respawn position and resets the health to the default value
    /// Inputs:
    /// None
    /// Returns:
    /// void (no return)
    /// </summary>
    void Respawn()
    {
        transform.position = respawnPosition;
        currentHealth = defaultHealth;
    }

    /// <summary>
    /// Description:
    /// Applies damage to the health unless the health is invincible.
    /// Inputs:
    /// int damageAmount
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="damageAmount">The amount of damage to take</param>
    public void TakeDamage(int damageAmount)
    {
        if (isInvincableFromDamage || isAlwaysInvincible)
        {
            return;
        }
        else
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, transform.rotation, null);
            }
            timeToBecomeDamagableAgain = Time.time + invincibilityTime;
            isInvincableFromDamage = true;
            currentHealth -= damageAmount;
            CheckDeath();
        }
    }

    /// <summary>
    /// Description:
    /// Applies healing to the health, capped out at the maximum health.
    /// Inputs:
    /// int healingAmount
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="healingAmount">How much healing to apply</param>
    public void ReceiveHealing(int healingAmount)
    {
        currentHealth += healingAmount;
        if (currentHealth > maximumHealth)
        {
            currentHealth = maximumHealth;
        }
        CheckDeath();
    }

    [Header("Effects & Polish")]
    [Tooltip("The effect to create when this health dies")]
    public GameObject deathEffect;
    [Tooltip("The effect to create when this health is damaged")]
    public GameObject hitEffect;

    /// <summary>
    /// Description:
    /// Checks if the health is dead or not. If it is, true is returned, false otherwise.
    /// Calls Die() if the health is dead.
    /// Inputs:
    /// none
    /// Returns:
    /// bool
    /// </summary>
    /// <returns>Bool: true or false value representing if the health has died or not (true for dead)</returns>
    bool CheckDeath()
    {
        if (currentHealth <= 0)
        {
            if (isExplosive)
            {
                bomb.GetComponent<ShootingController>().Fire();
            }
            Die();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Description:
    /// Handles the death of the health. If a death effect is set, it is created. If lives are being used, the health is respawned.
    /// If lives are not being used or the lives are 0 then the health's game object is destroyed.
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    public void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation, null);
        }

        if (useLives)
        {
            HandleDeathWithLives();
        }
        else
        {
            HandleDeathWithoutLives();
        }      
    }

    /// <summary>
    /// Description:
    /// Handles the death of the health when lives are being used
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void HandleDeathWithLives()
    {
        currentLives -= 1;
        if (currentLives > 0 && PlayerPrefs.GetInt("ToggleValue6") == 0 && teamId == 0)
        {
            Respawn();
        }
        else
        {
            if (gameObject.tag == "Player" && GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
            if (gameObject.GetComponent<Enemy>() != null)
            {
                gameObject.GetComponent<Enemy>().DoBeforeDestroy();
            }
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Description:
    /// Handles death when lives are not being used
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void HandleDeathWithoutLives()
    {
        if (gameObject.tag == "Player" && GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }
        if (gameObject.GetComponent<Enemy>() != null)
        {
            gameObject.GetComponent<Enemy>().DoBeforeDestroy();
        }
        Destroy(this.gameObject);
    }

    void HealthDrain()
    {
        currentHealth = currentHealth - healthDrainAmount;
        CheckDeath();
    }
}
