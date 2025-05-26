using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    public Enemy enemy;
    public GameObject hpBar;

    void Start()
    {
        GameObject enemyObj = this.transform.parent.gameObject;
        enemy = enemyObj.GetComponent<Enemy>();
        hpBar = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (enemy != null && hpBar != null)
        {
            hpBar.transform.localScale = new Vector3(1.0f, (float)enemy.hp / (float)enemy.maxHp, 1.0f);
        }
    }
}
