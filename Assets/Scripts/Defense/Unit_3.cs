using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_3 : Unit
{
    protected override IEnumerator Attack()
    {
        isAttacking = true;
        while (true)
        {
            yield return new WaitForSeconds(10);
            if (targetList.Count == 0) break;
            GameObject _enemy = GetFullHpEnemy();
            if (_enemy == null)
            {
                targetList.Remove(_enemy);
                continue;
            }
            Enemy enemyClass = _enemy.GetComponent<Enemy>();
            if (enemyClass == null)
            {
                targetList.Remove(_enemy);
                continue;
            }
            enemyClass.hp -= 10;
            if (enemyClass.hp <= 0)
            {
                targetList.Remove(_enemy);
            }
        }
    }

    private GameObject GetFullHpEnemy()
    {
        GameObject fullHpEnemy = null;
        int hp = 0;
        foreach (GameObject _enemy in targetList)
        {
            Enemy _class = _enemy.GetComponent<Enemy>();
            if (_class.hp >= hp)
            {
                fullHpEnemy = _enemy;
                hp = _class.hp;
            }
        }
        Debug.Log($"{hp}");
        return fullHpEnemy;
    }
}
