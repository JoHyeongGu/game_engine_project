using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float speed;
    public Vector3 goal;
    private NavMeshAgent agent;

    void Start()
    {
        InitAgent();
        SetGoal();
    }

    void Update()
    {
    }

    private void InitAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.acceleration = speed;
    }

    private void SetGoal()
    {
        GameObject desObj = GameObject.FindGameObjectWithTag("Destination");
        goal = desObj.transform.position;
        agent.destination = goal;
    }
}
