using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningGun : MonoBehaviour
{
    public float zRotationIntensity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(0, 0, zRotationIntensity);
    }
}
