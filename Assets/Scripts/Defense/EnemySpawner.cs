using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnInfo
{
    public float minCooltime;
    public float maxCooltime;
    public float[] enemyPercent;
}

[System.Serializable]
public struct EnemyInfo
{
    public GameObject prefab;
    public float percent;
}

public class EnemySpawner : MonoBehaviour
{
    public int stage = 1;
    public float minCooltime = 0.1f;
    public float maxCooltime = 1.0f;
    public EnemyInfo[] enemyList;
    public TextAsset spawnDataText = null;
    public bool activate = true;

    private Dictionary<string, SpawnInfo> spawnData;
    private Coroutine routine;

    void Start()
    {
        spawnData = new Dictionary<string, SpawnInfo>();
        InitSpawnData();
        SetSpawnData(stage, 1);
        if (activate)
        {
            routine = StartCoroutine(SpawnRoutine());
        }
    }

    public void SetSpawnData(int stage, int wave)
    {
        string key = $"{stage}_{wave}";
        if (spawnData == null)
        {
            spawnData = new Dictionary<string, SpawnInfo>();
            InitSpawnData();
        }
        if (!spawnData.ContainsKey(key)) return;
        SpawnInfo data = spawnData[key];
        this.minCooltime = data.minCooltime;
        this.maxCooltime = data.maxCooltime;
        for (int i = 0; i < data.enemyPercent.Length; i++)
        {
            enemyList[i].percent = data.enemyPercent[i];
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            GameObject enemy = GetRandomEnemy();
            GameObject instance = Instantiate(enemy, transform.position, transform.rotation);
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

    private void InitSpawnData()
    {
        string stringData = spawnDataText.text;
        string[] lines = stringData.Split('\n');
        foreach (string line in lines)
        {
            if (line == "" || line.Contains("#"))
            {
                continue;
            }
            string[] words = line.Split("\t");
            string key = $"{words[0]}_{words[1]}";
            var data = new SpawnInfo();
            data.minCooltime = float.Parse(words[2]);
            data.maxCooltime = float.Parse(words[3]);
            int offset = 4;
            data.enemyPercent = new float[words.Length - offset];
            for (int i = offset; i < words.Length; i++)
            {
                data.enemyPercent[i - offset] = float.Parse(words[i]);
            }
            spawnData[key] = data;
        }
    }

    public void ClearSpawnedList()
    {
        GameObject[] spawnedEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] spawnedUnits = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject _enemy in spawnedEnemies)
        {
            Destroy(_enemy);
        }
        foreach (GameObject _unit in spawnedUnits)
        {
            Destroy(_unit);
        }
    }
}
