using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public GameManager GM;
    public GameObject lockedGate;

    void Start()
    {
        if (PlayerPrefs.GetInt("ToggleValue4") == 1)
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
