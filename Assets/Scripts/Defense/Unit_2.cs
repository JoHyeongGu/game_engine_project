using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_2 : Unit
{
    // Attack All Near Enemies
    protected override IEnumerator Attack()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(this.transform.position, enemy.transform.position);
            if (distance < atkDist)
            {
                target = enemy.GetComponent<Enemy>();
                target.hp -= atk;
            }
        }
        yield return null;
    }
}
