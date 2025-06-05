using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_2 : Unit
{
    protected override void Update()
    {
        if (GetTargetCount() > 0)
        {
            anim.SetBool("Attack", true);
        }
        else
        {
            anim.SetBool("Attack", false);
        }
        LookTarget();
        base.Update();
    }

    protected override IEnumerator Attack()
    {
        isAttacking = true;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (targetList.Count == 0) break;
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
                _class.hp -= 0.3f;
                if (_class.hp <= 0)
                {
                    targetList.Remove(_enemy);
                }
            }
        }
    }

    private int GetTargetCount()
    {
        int count = 0;
        foreach (GameObject target in targetList)
        {
            count++;
            if (target == null) continue;
            Enemy _class = target.GetComponent<Enemy>();
            if (_class == null)
            {
                count--;
                targetList.Remove(target);
            }
        }
        return count;
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
