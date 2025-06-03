using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBread : MonoBehaviour
{
    public Vector3 start;
    public Vector3 destination;
    public float speed = 10.0f;

    private Vector3 lookAt;

    public bool isArrive = false;

    void Start()
    {
        lookAt = (destination - start).normalized;
        this.transform.position = start;
    }

    void Update()
    {
        if (!isArrive)
        {
            this.transform.position += lookAt * speed * Time.deltaTime;
        }
        if (this.transform.position.z <= this.destination.z)
        {
            isArrive = true;
        }
    }
}
