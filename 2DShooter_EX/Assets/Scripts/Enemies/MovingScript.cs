using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingScript : MonoBehaviour
{
    public Transform[] waypoints;
    public int startingOffset;
    public float speed;

    public bool usesStartingPoint;
    public int i = 0;

    public bool stops;
    public int stoppingPoint;

    // Start is called before the first frame update
    void Start()
    {
        if (usesStartingPoint)
        {
            transform.position = waypoints[startingOffset].position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoints != null)
        {
            if (!stops)
            {
                if (Vector2.Distance(transform.position, waypoints[i].position) < 0.02f)
                {
                    i++;
                    if (i == waypoints.Length)
                    {
                        i = 0;
                    }
                }
                transform.position = Vector2.MoveTowards(transform.position, waypoints[i].position, speed * Time.deltaTime);
            }
            else
            {
                if (Vector2.Distance(transform.position, waypoints[i].position) < 0.02f)
                {
                    i++;
                    if (i == waypoints.Length)
                    {
                        i = 0;
                    }
                    if (i == stoppingPoint)
                    {
                        speed = 0;
                    }
                }
                transform.position = Vector2.MoveTowards(transform.position, waypoints[i].position, speed * Time.deltaTime);
            }
        }
    }
}
