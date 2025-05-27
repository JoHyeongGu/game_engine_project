using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_4 : Unit
{
    protected override IEnumerator Attack()
    {
        isAttacking = true;
        while (true)
        {
            yield return new WaitForSeconds(3);
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
            for (int i = 0; i < 10; i++)
            {
                enemyClass.hp -= 0.5f;
                if (enemyClass.hp <= 0)
                {
                    targetList.Remove(targetList[0]);
                    break;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
