using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class KeyScript : MonoBehaviour
{
    public int KeysScore;
    public GameObject GM;
    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("KeyDestroyed");
        GM.GetComponent<GameManager>().Keys += KeysScore;
        Destroy(gameObject);
    }
}
