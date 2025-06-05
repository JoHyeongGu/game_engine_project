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
            if (Input.GetMouseButtonDown(1))
            {
                foreach (Price p in price)
                {
                    // 포인트 복구
                    scoreCounter.PointUp(p.key, p.value);
                }
                Destroy(this.gameObject);
                Destroy(this);
            }
            CheckCanPlaced();
            return;
        }
        attackTime += Time.deltaTime;
        LookTarget();
        if (attackTime >= 5.0f)
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
        anim.SetTrigger("Attack");
        enemyClass.hp -= 30;
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

    public override void LookTarget()
    {
        target = GetFullHpEnemy();
        if (target != null)
        {
            Vector3 localDir = transform.parent.InverseTransformPoint(target.transform.position) - transform.localPosition;
            localDir.y = 0f;

            if (localDir == Vector3.zero) return;

            Quaternion lookRotation = Quaternion.LookRotation(localDir);
            Quaternion offsetRotation = Quaternion.AngleAxis(-170f, Vector3.up); // 로컬 Y축 기준 -30도

            transform.localRotation = lookRotation * offsetRotation;
        }
    }
}
