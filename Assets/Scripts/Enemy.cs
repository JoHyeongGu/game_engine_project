using System.Collections;
using System.Threading.Tasks;
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
        if (agent.remainingDistance <= 0.5f)
        {
            ArriveToGoal();
        }
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

    private void ArriveToGoal()
    {
        Destroy(this.gameObject);
    }
}
