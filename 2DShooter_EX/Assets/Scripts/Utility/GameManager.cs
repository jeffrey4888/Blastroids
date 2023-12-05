using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class which manages the game
/// </summary>
public class GameManager : MonoBehaviour
{
    // The script that manages all others
    public static GameManager instance = null;

    [Tooltip("The UIManager component which manages the current scene's UI")]
    public UIManager uiManager = null;

    [Tooltip("Don't change this value.")]
    public int enemiesFromStatic;

    [Tooltip("The player gameobject")]
    public GameObject player = null;

    [Tooltip("Whether or not the game tracks a specific amount of enemies in order to complete a level.")]
    public bool enemyWinCondition;

    [Header("Scores")]
    // The current player score in the game
    [Tooltip("The player's score")]
    [SerializeField] private int gameManagerScore = 0;

    [Header("Game Builder Toggles")]
    public Toggle toggleStrength;
    public Toggle toggleEvolutions;
    public Toggle toggleAdditionals;
    public Toggle toggleLocks;
    public Toggle toggleWeakness;
    public Toggle toggleAntiRespawn;
    public Toggle toggleNoHitMode;
    public Toggle toggleGunOnly;

    private const string ToggleKeyStr = "ToggleValue1";
    private const string ToggleKeyEvo = "ToggleValue2";
    private const string ToggleKeyAdd = "ToggleValue3";
    private const string ToggleKeyLock = "ToggleValue4";
    private const string ToggleKeyWeak = "ToggleValue5";
    private const string ToggleKeyNoRs = "ToggleValue6";
    private const string ToggleKeyNoHP = "ToggleValue7";
    private const string ToggleKeyNoWP = "ToggleValue8";

    [Header("Enemy Layouts")]
    [Tooltip("The enemies to enable when the evolved Box is unchecked")]
    public GameObject[] enemiesUnevolved; // The base amount of enemies without any evolutions.
    [Tooltip("The enemies to enable when the evolved Box is checked")]
    public GameObject[] enemiesEvolved; // The base amount of enemies with some enemies substituted with their evolved forms that are not in the base game.
    [Tooltip("The enemies to enable when more enemies is enabled. No switching with evolutions")]
    public GameObject[] moreEnemies; // Extra enemies to enable on top of the base amount of enemies.
    [Tooltip("The enemies to enable when more enemies is enabled without evolutions enabled")]
    public GameObject[] moreEnemiesUnevolved; // Extra enemies to enable on top of the base amount of enemies, but for enemies that have evolutions.
    [Tooltip("The enemies to enable when more enemies is enabled with evolutions enabled")]
    public GameObject[] moreEnemiesEvolved; // Extra enemies to enable on top of the base amount of enemies with some enemies substituted with their evolved forms.
    public Text EnemiesRemainingText;
    public GameObject EnemiesRemainingTextObject;

    // Static getter/setter for player score (for convenience)
    public static int score
    {
        get
        {
            return instance.gameManagerScore;
        }
        set
        {
            instance.gameManagerScore = value;
        }
    }

    // The highest score obtained by this player
    [Tooltip("The highest score acheived on this device")]
    public int highScore = 0;

    [Header("Game Progress / Victory Settings")]
    [Tooltip("Whether the game is winnable or not \nDefault: true")]
    public bool gameIsWinnable = true;
    [Tooltip("The number of enemies that must be defeated to win the game. Do not change this value. Instead, change the two below to set this value.")]
    public int enemiesToDefeat = 0;

    [Tooltip("The number of enemies that must be defeated to win the game while More Enemies is disabled.")]
    public int enemyCondition = 10;
    [Tooltip("The number of enemies that must be defeated to win the game while More Enemies is enabled. This value should be higher than enemiesToDefeat")]
    public int enemyConditionExtended = 20;
    
    // The number of enemies defeated in game
    public int enemiesDefeated = 0;

    [Tooltip("Whether or not to print debug statements about whether the game can be won or not according to the game manager's" +
        " search at start up")]
    public bool printDebugOfWinnableStatus = true;
    [Tooltip("Page index in the UIManager to go to on winning the game")]
    public int gameVictoryPageIndex = 0;
    [Tooltip("The effect to create upon winning the game")]
    public GameObject victoryEffect;

    //The number of enemies observed by the game manager in this scene at start up"
    private int numberOfEnemiesFoundAtStart;

    private Scene scene;

    /// <summary>
    /// Description:
    /// Standard Unity function called when the script is loaded, called before start
    /// 
    /// When this component is first added or activated, setup the global reference
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first Update
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void Start()
    {
        scene = SceneManager.GetActiveScene();
        HandleStartUp();
        if (scene.name == "MainMenu")
        {
            toggleStrength.isOn = PlayerPrefs.GetInt(ToggleKeyStr, 0) == 1; // Strength is toggled on is equal to true if togglekeystr is 1
            toggleEvolutions.isOn = PlayerPrefs.GetInt(ToggleKeyEvo, 0) == 1;
            toggleAdditionals.isOn = PlayerPrefs.GetInt(ToggleKeyAdd, 0) == 1;
            toggleLocks.isOn = PlayerPrefs.GetInt(ToggleKeyLock, 0) == 1;
            toggleWeakness.isOn = PlayerPrefs.GetInt(ToggleKeyWeak, 0) == 1;
            toggleAntiRespawn.isOn = PlayerPrefs.GetInt(ToggleKeyNoRs, 0) == 1;
            toggleNoHitMode.isOn = PlayerPrefs.GetInt(ToggleKeyNoHP, 0) == 1;
            toggleGunOnly.isOn = PlayerPrefs.GetInt(ToggleKeyNoWP, 0) == 1;

            toggleStrength.onValueChanged.AddListener(ToggleOneStrength);
            toggleEvolutions.onValueChanged.AddListener(ToggleTwoEvolve);
            toggleAdditionals.onValueChanged.AddListener(ToggleThreeAdd);
            toggleLocks.onValueChanged.AddListener(ToggleFourLock);
            toggleWeakness.onValueChanged.AddListener(ToggleFiveWeakness);
            toggleAntiRespawn.onValueChanged.AddListener(ToggleSixRespawn);
            toggleNoHitMode.onValueChanged.AddListener(ToggleSevenOneHP);
            toggleGunOnly.onValueChanged.AddListener(ToggleEightWeapons);
        } else
        {
            if (PlayerPrefs.GetInt("ToggleValue2") == 0 && PlayerPrefs.GetInt("ToggleValue3") == 0) // No evolutions, Base Enemy Amount.
            {
                for (int i = 0; i < enemiesUnevolved.Length; i++)
                {
                    enemiesUnevolved[i].SetActive(true); // Enemies to turn on if evolutions are disabled. Swaps positions with evolved enemies.
                }
                for (int i = 0; i < enemiesEvolved.Length; i++)
                {
                    enemiesEvolved[i].SetActive(false); // Enemies to turn on if evolutions are enabled. Swaps positions with unevolved enemies.
                }
                for (int i = 0; i < moreEnemies.Length; i++)
                {
                    moreEnemies[i].SetActive(false); // Enemies to turn on if extra enemies is enabled. Unaffected by evolutions.
                }
                for (int i = 0; i < moreEnemiesUnevolved.Length; i++)
                {
                    moreEnemiesUnevolved[i].SetActive(false); // Extra enemies to turn on, has same flip-flop with extra + evolutions.
                }
                for (int i = 0; i < moreEnemiesEvolved.Length; i++)
                {
                    moreEnemiesEvolved[i].SetActive(false); // Extra enemies to turn on, has same flip-flip with extra + no evolutions.
                }
            } 
            else if (PlayerPrefs.GetInt("ToggleValue2") == 1 && PlayerPrefs.GetInt("ToggleValue3") == 0) // Evolutions, Base Enemy Amount.
            {
                for (int i = 0; i < enemiesUnevolved.Length; i++)
                {
                    enemiesUnevolved[i].SetActive(false);
                }
                for (int i = 0; i < enemiesEvolved.Length; i++)
                {
                    enemiesEvolved[i].SetActive(true);
                }
                for (int i = 0; i < moreEnemies.Length; i++)
                {
                    moreEnemies[i].SetActive(false);
                }
                for (int i = 0; i < moreEnemiesUnevolved.Length; i++)
                {
                    moreEnemiesUnevolved[i].SetActive(false);
                }
                for (int i = 0; i < moreEnemiesEvolved.Length; i++)
                {
                    moreEnemiesEvolved[i].SetActive(false);
                }
            }
            else if (PlayerPrefs.GetInt("ToggleValue2") == 0 && PlayerPrefs.GetInt("ToggleValue3") == 1) // No evolutions, Extra enemies.
            {
                for (int i = 0; i < enemiesUnevolved.Length; i++)
                {
                    enemiesUnevolved[i].SetActive(true); // Enemies to turn on if evolutions are disabled. Swaps positions with evolved enemies.
                }
                for (int i = 0; i < enemiesEvolved.Length; i++)
                {
                    enemiesEvolved[i].SetActive(false); // Enemies to turn on if evolutions are enabled. Swaps positions with unevolved enemies.
                }
                for (int i = 0; i < moreEnemies.Length; i++)
                {
                    moreEnemies[i].SetActive(true); // Enemies to turn on if extra enemies is enabled. Unaffected by evolutions.
                }
                for (int i = 0; i < moreEnemiesUnevolved.Length; i++)
                {
                    moreEnemiesUnevolved[i].SetActive(true); // Extra enemies to turn on, has same flip-flop with extra + evolutions.
                }
                for (int i = 0; i < moreEnemiesEvolved.Length; i++)
                {
                    moreEnemiesEvolved[i].SetActive(false); // Extra enemies to turn on, has same flip-flip with extra + no evolutions.
                }
            }
            if (PlayerPrefs.GetInt("ToggleValue2") == 1 && PlayerPrefs.GetInt("ToggleValue3") == 1) // Evolutions, Extra Enemies.
            {
                for (int i = 0; i < enemiesUnevolved.Length; i++)
                {
                    enemiesUnevolved[i].SetActive(false); // Enemies to turn on if evolutions are disabled. Swaps positions with evolved enemies.
                }
                for (int i = 0; i < enemiesEvolved.Length; i++)
                {
                    enemiesEvolved[i].SetActive(true); // Enemies to turn on if evolutions are enabled. Swaps positions with unevolved enemies.
                }
                for (int i = 0; i < moreEnemies.Length; i++)
                {
                    moreEnemies[i].SetActive(true); // Enemies to turn on if extra enemies is enabled. Unaffected by evolutions.
                }
                for (int i = 0; i < moreEnemiesUnevolved.Length; i++)
                {
                    moreEnemiesUnevolved[i].SetActive(false); // Extra enemies to turn on, has same flip-flop with extra + evolutions.
                }
                for (int i = 0; i < moreEnemiesEvolved.Length; i++)
                {
                    moreEnemiesEvolved[i].SetActive(true); // Extra enemies to turn on, has same flip-flip with extra + no evolutions.
                }
            }
            if (PlayerPrefs.GetInt("ToggleValue3") == 0) // If More Enemies is disabled
            {
                enemiesToDefeat = enemyCondition;
            }
            else // If More Enemies is enabled
            {
                enemiesToDefeat = enemyConditionExtended;
            }
        }
    }

    void Update()
    {
        if (scene.name != "MainMenu")
        {
            if (PlayerPrefs.GetInt("ToggleValue4") == 1)
            {
                int enemiesRemaining;
                EnemiesRemainingTextObject.SetActive(true);
                enemiesRemaining = enemiesToDefeat - enemiesDefeated;
                EnemiesRemainingText.text = "Enemies Required: " + enemiesRemaining;
                if (enemiesRemaining <= 0)
                {
                    EnemiesRemainingText.text = "Exit Is Open!";
                }
            }
            else
            {
                EnemiesRemainingTextObject.SetActive(false);
            }
        }
    }

    public void ToggleOneStrength(bool isOn)
    // A method toggled using toggle keys and toggle objects that acts as a boolean for settings.
    {
        if (isOn) // Checks if the toggle is toggled
        {
            PlayerPrefs.SetInt(ToggleKeyStr, 1);
        }
        else // Sets the bool to match the toggle toggled off
        {
            PlayerPrefs.SetInt(ToggleKeyStr, 0);
        }
        // If is on is a value other than 1, it becomes a zero.
        // ? checks for null values in a condensed if statement, and the colon creates an output if the value is null.
        PlayerPrefs.Save();
        // Save saves the player preferences to use in between sessions.
    }

    public void ToggleTwoEvolve(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt(ToggleKeyEvo, 1);
        }
        else
        {
            PlayerPrefs.SetInt(ToggleKeyEvo, 0);
        }
        PlayerPrefs.Save();
    }

    public void ToggleThreeAdd(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt(ToggleKeyAdd, 1);
        }
        else
        {
            PlayerPrefs.SetInt(ToggleKeyAdd, 0);
        }
        PlayerPrefs.Save();
    }

    public void ToggleFourLock(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt(ToggleKeyLock, 1);
        }
        else
        {
            PlayerPrefs.SetInt(ToggleKeyLock, 0);
        }
        PlayerPrefs.Save();
    }

    public void ToggleFiveWeakness(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt(ToggleKeyWeak, 1);
        }
        else
        {
            PlayerPrefs.SetInt(ToggleKeyWeak, 0);
        }
        PlayerPrefs.Save();
    }

    public void ToggleSixRespawn(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt(ToggleKeyNoRs, 1);
        }
        else
        {
            PlayerPrefs.SetInt(ToggleKeyNoRs, 0);
        }
        PlayerPrefs.Save();
    }

    public void ToggleSevenOneHP(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt(ToggleKeyNoHP, 1);
        }
        else
        {
            PlayerPrefs.SetInt(ToggleKeyNoHP, 0);
        }
        PlayerPrefs.Save();
    }

    public void ToggleEightWeapons(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt(ToggleKeyNoWP, 1);
        }
        else
        {
            PlayerPrefs.SetInt(ToggleKeyNoWP, 0);
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Description:
    /// Handles necessary activities on start up such as getting the highscore and score, updating UI elements, 
    /// and checking the number of enemies
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void HandleStartUp()
    {
        if (PlayerPrefs.HasKey("highscore"))
        {
            highScore = PlayerPrefs.GetInt("highscore");
        }
        if (PlayerPrefs.HasKey("score"))
        {
            score = PlayerPrefs.GetInt("score");
        }
        UpdateUIElements();
        if (printDebugOfWinnableStatus)
        {
            FigureOutHowManyEnemiesExist();
        }
    }

    /// <summary>
    /// Description:
    /// Searches the level for all spawners and static enemies.
    /// Only produces debug messages / warnings if the game is set to be winnable
    /// If there are any infinite spawners a debug message will say so,
    /// If there are more enemies than the number of enemies to defeat to win
    /// then a debug message will say so
    /// If there are too few enemies to defeat to win then a debug warning will say so
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void FigureOutHowManyEnemiesExist()
    {
            List<EnemySpawner> enemySpawners = FindObjectsOfType<EnemySpawner>().ToList();
            List<Enemy> staticEnemies = FindObjectsOfType<Enemy>().ToList();

            int numberOfInfiniteSpawners = 0;
            int enemiesFromSpawners = 0;
            enemiesFromStatic = staticEnemies.Count;
            foreach (EnemySpawner enemySpawner in enemySpawners)
            {
                if (enemySpawner.spawnInfinite)
                {
                    numberOfInfiniteSpawners += 1;
                }
                else
                {
                    enemiesFromSpawners += enemySpawner.maxSpawn;
                }
            }
            numberOfEnemiesFoundAtStart = enemiesFromSpawners + enemiesFromStatic;

            if (gameIsWinnable)
            {
                if (numberOfInfiniteSpawners > 0)
                {
                    Debug.Log("There are " + numberOfInfiniteSpawners + " infinite spawners " + " so the level will always be winnable, "
                        + "\nhowever you sshould still playtest for timely completion");
                }
                else if (enemiesToDefeat > numberOfEnemiesFoundAtStart)
                {
                    Debug.LogWarning("There are " + enemiesToDefeat + " enemies to defeat but only " + numberOfEnemiesFoundAtStart +
                        " enemies found at start \nThe level can not be completed!");
                }
                else
                {
                    Debug.Log("There are " + enemiesToDefeat + " enemies to defeat and " + numberOfEnemiesFoundAtStart +
                        " enemies found at start \nThe level can completed");
                }
            }
    }

    /// <summary>
    /// Description:
    /// Increments the number of enemies defeated by 1
    /// Input:
    /// none
    /// Return:
    /// void (no returned value)
    /// </summary>
    public void IncrementEnemiesDefeated()
    {
        enemiesDefeated++;
        
        if (enemyWinCondition)
        {
            if (enemiesDefeated >= enemiesToDefeat && gameIsWinnable)
            {
                LevelCleared();
            }
        }
        
    }

    /// <summary>
    /// Description:
    /// Standard Unity function that gets called when the application (or playmode) ends
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveHighScore();
        ResetScore();
    }

    /// <summary>
    /// Description:
    /// Adds a number to the player's score stored in the gameManager
    /// Input: 
    /// int scoreAmount
    /// Returns: 
    /// void (no return)
    /// </summary>
    /// <param name="scoreAmount">The amount to add to the score</param>
    public static void AddScore(int scoreAmount)
    {
        score += scoreAmount;
        if (score > instance.highScore)
        {
            SaveHighScore();
        }
        UpdateUIElements();
    }
    
    /// <summary>
    /// Description:
    /// Resets the current player score
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public static void ResetScore()
    {
        PlayerPrefs.SetInt("score", 0);
        score = 0;
    }

    /// <summary>
    /// Description:
    /// Saves the player's highscore
    /// Input: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public static void SaveHighScore()
    {
        if (score > instance.highScore)
        {
            PlayerPrefs.SetInt("highscore", score);
            instance.highScore = score;
        }
        UpdateUIElements();
    }

    /// <summary>
    /// Description:
    /// Resets the high score in player preferences
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public static void ResetHighScore()
    {
        PlayerPrefs.SetInt("highscore", 0);
        if (instance != null)
        {
            instance.highScore = 0;
        }
        UpdateUIElements();
    }

    /// <summary>
    /// Description:
    /// Sends out a message to UI elements to update
    /// Input: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public static void UpdateUIElements()
    {
        if (instance != null && instance.uiManager != null)
        {
            instance.uiManager.UpdateUI();
        }
    }

    /// <summary>
    /// Description:
    /// Ends the level, meant to be called when the level is complete (enough enemies have been defeated)
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void LevelCleared()
    {
        PlayerPrefs.SetInt("score", score);
        Debug.Log("Yes");
        if (uiManager != null)
        {
            player.SetActive(false);
            uiManager.allowPause = false;
            uiManager.GoToPage(gameVictoryPageIndex);
            if (victoryEffect != null)
            {
                Instantiate(victoryEffect, transform.position, transform.rotation, null);
            }
        }     
    }

    [Header("Game Over Settings:")]
    [Tooltip("The index in the UI manager of the game over page")]
    public int gameOverPageIndex = 0;
    [Tooltip("The game over effect to create when the game is lost")]
    public GameObject gameOverEffect;

    // Whether or not the game is over
    [HideInInspector]
    public bool gameIsOver = false;

    /// <summary>
    /// Description:
    /// Displays game over screen
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    public void GameOver()
    {
        gameIsOver = true;
        if (gameOverEffect != null)
        {
            Instantiate(gameOverEffect, transform.position, transform.rotation, null);
        }
        if (uiManager != null)
        {
            uiManager.allowPause = false;
            uiManager.GoToPage(gameOverPageIndex);
        }
    }
}
