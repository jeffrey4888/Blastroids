using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour
{
    public GameManager GM;
    public GameObject lockedGate;

    void Start()
    {
        if (PlayerPrefs.GetInt("ToggleValue4") == 1 || SceneManager.GetActiveScene().name == "Level2")
        {
            lockedGate.SetActive(true);
        }
        else
        {
            lockedGate.SetActive(false);
        }

        
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("ToggleValue4") == 1 && lockedGate.activeSelf)
        {
            if (GM.enemiesDefeated >= GM.enemiesToDefeat)
            {
                lockedGate.SetActive(false);
            }
        }
        if (SceneManager.GetActiveScene().name == "Level2" && GM.GetComponent<GameManager>().KeysTextObject != null && GM.GetComponent<GameManager>().Keys < 4)
        {
            lockedGate.SetActive(true);
        }
        else
        {
            lockedGate.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GM.LevelCleared();
            Debug.Log("Win");
        }
    }
}
