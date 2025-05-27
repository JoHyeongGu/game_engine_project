using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float hp;
    public float maxHp;
    public float speed;
    public Vector3 goal;
    public int splitCount = 0;
    public NavMeshAgent agent;

    void Start()
    {
        maxHp = hp;
        InitAgent();
        SetGoal();
    }

    void Update()
    {
        if (hp <= 0)
        {
            if (splitCount > 0)
            {
                SplitBody();
            }
            Destroy(this.gameObject);
        }
        else if (agent.remainingDistance <= 0.01f)
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
        SceneControl scene = GameObject.FindWithTag("Root").GetComponent<SceneControl>();
        if (scene.stepTimer >= 0.1f) scene.hp--;
        Destroy(this.gameObject);
    }

    private void SplitBody()
    {
        int newHp = (int)(this.maxHp / splitCount);
        Vector3 size = transform.localScale;
        Vector3 pos = transform.position;
        Vector3 newSize = new Vector3(size.x / 2, size.y / 2, size.z / 2);
        float newSpeed = this.speed * 1.8f;
        for (int i = 0; i < splitCount; i++)
        {
            GameObject clone = Instantiate(this.gameObject);
            clone.transform.localScale = newSize;
            float posOffset = (i - 1) * Random.Range(0.01f, 0.15f);
            clone.transform.position = new Vector3(pos.x + posOffset, pos.y + posOffset, pos.z + posOffset);
            Enemy _class = clone.GetComponent<Enemy>();
            _class.hp = newHp;
            _class.splitCount = 0;
            _class.speed = newSpeed;
        }
    }
}
