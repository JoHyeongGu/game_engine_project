using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_2 : Unit
{
    protected override void Update()
    {
        if (IsNearEnemy()) anim.SetBool("Attack", true);
        else anim.SetBool("Attack", false);
        LookTarget();
        base.Update();
    }

    protected override IEnumerator Attack()
    {
        isAttacking = true;
        while (true)
        {
            if (targetList.Count == 0) break;
            yield return new WaitForSeconds(0.3f);
            for (int i = targetList.Count - 1; i >= 0; i--)
            {
                GameObject _enemy = targetList[i];
                if (_enemy == null)
                {
                    targetList.Remove(_enemy);
                    continue;
                }
                Enemy _class = _enemy.GetComponent<Enemy>();
                if (_class == null)
                {
                    targetList.Remove(_enemy);
                    continue;
                }
                if (targetList.Count == 0)
                {
                    targetList.Add(_enemy);
                }
                _class.hp -= 0.1f;
                if (_class.hp <= 0)
                {
                    targetList.Remove(_enemy);
                }
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
            if (distance < 2.0f) return true;
        }
        return false;
    }

    public override void LookTarget()
    {
        if (targetList.Count == 0) return;
        target = targetList[0];
        if (target != null)
        {
            Vector3 localDir = transform.parent.InverseTransformPoint(target.transform.position) - transform.localPosition;
            localDir.y = 0f;

            if (localDir == Vector3.zero) return;

            Quaternion lookRotation = Quaternion.LookRotation(localDir);
            Quaternion offsetRotation = Quaternion.AngleAxis(-100f, Vector3.up); // 로컬 Y축 기준 -30도

            transform.localRotation = lookRotation * offsetRotation;
        }
    }
}
