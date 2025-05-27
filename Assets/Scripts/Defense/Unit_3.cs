using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_3 : Unit
{
    private float attackTime = 0.0f;

    protected override void Update()
    {
        if (!isActive)
        {
            this.transform.position = mousePosition;
            CheckCanPlaced();
        }
        attackTime += Time.deltaTime;
        if (attackTime >= 10.0f)
        {
            AttackEnemy();
            attackTime = 0.0f;
        }
    }

    private void AttackEnemy()
    {
        if (targetList.Count == 0) return;
        GameObject _enemy = GetFullHpEnemy();
        if (_enemy == null)
        {
            targetList.Remove(_enemy);
            return;
        }
        Enemy enemyClass = _enemy.GetComponent<Enemy>();
        if (enemyClass == null)
        {
            targetList.Remove(_enemy);
            return;
        }
        enemyClass.hp -= 10;
        if (enemyClass.hp <= 0)
        {
            targetList.Remove(_enemy);
        }
    }

    private GameObject GetFullHpEnemy()
    {
        GameObject fullHpEnemy = null;
        float hp = 0;
        foreach (GameObject _enemy in targetList)
        {
            if (_enemy == null) continue;
            Enemy _class = _enemy.GetComponent<Enemy>();
            if (_class == null) continue;
            if (_class.hp >= hp)
            {
                fullHpEnemy = _enemy;
                hp = _class.hp;
            }
        }
        return fullHpEnemy;
    }
}
