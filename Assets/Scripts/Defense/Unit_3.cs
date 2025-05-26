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
            yield return new WaitForSeconds(1);
            if (targetList.Count == 0) break;
            GameObject firstEnemy = targetList[0];
            if (firstEnemy == null)
            {
                targetList.Remove(targetList[0]);
                continue;
            }
            Enemy enemyClass = firstEnemy.GetComponent<Enemy>();
            if (enemyClass == null)
            {
                targetList.Remove(targetList[0]);
                continue;
            }
            enemyClass.hp -= 10;
            if (enemyClass.hp <= 0)
            {
                targetList.Remove(targetList[0]);
            }
        }
    }
}
