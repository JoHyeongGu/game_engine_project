using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_4 : Unit
{
    protected override void Update()
    {
        if (IsNearEnemy()) anim.SetBool("IsAttack", true);
        else anim.SetBool("IsAttack", false);
        LookTarget();
        base.Update();
    }

    protected override IEnumerator Attack()
    {
        isAttacking = true;
        while (true)
        {
            if (targetList.Count == 0) break;
            GameObject firstEnemy = targetList[0];
            if (firstEnemy == null)
            {
                targetList.Remove(targetList[0]);
                continue;
            }
            target = firstEnemy;
            Enemy enemyClass = firstEnemy.GetComponent<Enemy>();
            if (enemyClass == null)
            {
                targetList.Remove(targetList[0]);
                continue;
            }
            yield return new WaitForSeconds(3.0f);
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < 3; i++)
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

    private bool IsNearEnemy()
    {
        if (!isActive) return false;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(this.transform.position, enemy.transform.position);
            if (distance < 4.0f) return true;
        }
        return false;
    }

    public override void LookTarget()
    {
        if (target != null)
        {
            // anim.SetBool("IsAttack", true);
            Vector3 localDir = transform.parent.InverseTransformPoint(target.transform.position) - transform.localPosition;
            localDir.y = 0f;

            if (localDir == Vector3.zero) return;

            Quaternion lookRotation = Quaternion.LookRotation(localDir);
            Quaternion offsetRotation = Quaternion.AngleAxis(-170f, Vector3.up); // 로컬 Y축 기준 -30도

            transform.localRotation = lookRotation * offsetRotation;
        }
    }
}
