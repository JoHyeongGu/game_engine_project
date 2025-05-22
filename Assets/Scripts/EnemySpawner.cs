using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyInfo
{
    public GameObject prefab;
    public float percent;
}


public class EnemySpawner : MonoBehaviour
{
    public float minCooltime = 0.1f;
    public float maxCooltime = 1.0f;
    public EnemyInfo[] enemyList;

    private Coroutine routine;

    void Start()
    {
        routine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            GameObject enemy = GetRandomEnemy();
            Instantiate(enemy, transform.position, transform.rotation);
            yield return new WaitForSeconds(Random.Range(minCooltime, maxCooltime));
        }
    }

    private GameObject GetRandomEnemy()
    {
        float select = (float)Random.value;
        float percent = 0.0f;
        foreach (EnemyInfo enemy in enemyList)
        {
            percent += enemy.percent;
            if (select < percent)
            {
                return enemy.prefab;
            }
        }
        return enemyList[0].prefab;
    }
}
