using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_2 : Unit
{
    protected override IEnumerator Attack()
    {
        isAttacking = true;
        while (true)
        {
            yield return new WaitForSeconds(3f);
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
                _class.hp--;
                if (_class.hp <= 0)
                {
                    targetList.Remove(_enemy);
                }
            }
        }
    }
}
