using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_4 : Unit
{
    protected override IEnumerator Attack()
    {
        selfAudioSource.Play();
        for (int i = 0; i < 3; i++)
        {
            PlayEnemySound();
            target.hp -= atk;
            if (target.hp <= 0)
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
