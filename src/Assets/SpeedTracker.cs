using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTracker : MonoBehaviour
{
    private Vector3 previousPosition;
    public Vector3 Direction { get; private set; }
    public float Speed { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Direction = transform.position - previousPosition;
        Speed = Direction.magnitude / Time.deltaTime;
        previousPosition = transform.position;
    }
}
