using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_3 : Unit
{
    // Get Max HP Enemy
    protected override void SetTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Enemy fullHpEnemy = null;
        float maxHp = 0f;
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(this.transform.position, enemy.transform.position);
            if (distance > atkDist) continue;
            Enemy _class = enemy.GetComponent<Enemy>();
            if (_class.hp > maxHp)
            {
                fullHpEnemy = _class;
                maxHp = _class.hp;
            }
        }
        target = fullHpEnemy;
        SetReadyAnimation();
    }
}
