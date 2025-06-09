using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct EnemySound
{
    public AudioClip attacked;
    public AudioClip destroy;
    public AudioClip splited;
}

public class Enemy : MonoBehaviour
{
    public float hp;
    public float maxHp;
    public float speed;
    public EnemySound sounds;
    public Vector3 goal;
    public List<Vector3> wayPoints;
    public int splitCount = 0;
    public NavMeshAgent agent;
    public bool isSplited = false;

    private bool completeWayPoints = false;

    void Start()
    {
        maxHp = hp;
        InitAgent();
        InitWayPoints();
        StartCoroutine(FollowGoals());
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
        else if (completeWayPoints && agent.remainingDistance <= 0.01f)
        {
            Destroy(this.gameObject);
        }
    }

    public void Attacked(float atk)
    {
        hp -= atk;
    }

    private void Play(AudioClip clip)
    {
        // AudioSource.PlayClipAtPoint(clip, transform.position);
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    private async void Destory()
    {
        GetComponent<Collider>().enabled = false;
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
        await Task.Delay(1000);
        Destroy(this.gameObject);
    }

    private void InitAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.acceleration = speed;
    }

    private void InitWayPoints()
    {
        this.goal = GameObject.FindGameObjectWithTag("Goal").transform.position;
        if (isSplited) return;
        GameObject[] points = GameObject.FindGameObjectsWithTag("WayPoint");
        foreach (GameObject point in points)
        {
            this.wayPoints.Add(point.transform.position);
        }
    }

    IEnumerator FollowGoals()
    {
        if (isSplited) wayPoints.Clear();
        if (wayPoints.Count > 0)
        {
            int index = Random.Range(0, wayPoints.Count);
            Vector3 point = wayPoints[index];
            agent.SetDestination(point);
            while (true)
            {
                yield return null;
                if (!agent.isActiveAndEnabled || agent.remainingDistance <= 1f) break;
            }
        }
        if (agent.isActiveAndEnabled)
        {
            completeWayPoints = true;
            agent.SetDestination(this.goal);
        }
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
            Enemy _class = clone.GetComponent<Enemy>();
            clone.transform.localScale = newSize;
            float posOffset = (i - 1) * Random.Range(0.01f, 0.15f);
            clone.transform.position = new Vector3(pos.x + posOffset, pos.y + posOffset, pos.z + posOffset);
            _class.hp = newHp;
            _class.splitCount = 0;
            _class.speed = newSpeed;
            _class.isSplited = true;
        }
    }

    public void ActiveEffect(string effect, bool active = true)
    {
        foreach (Transform child in this.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag(effect))
            {
                child.gameObject.SetActive(active);
                return;
            }
        }
    }
}
