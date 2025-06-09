using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_2 : Unit
{
    private bool isPlayingSelfAudio = false;
    private Coroutine loopEnemySound;

    // Attack All Near Enemies
    protected override IEnumerator Attack()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(this.transform.position, enemy.transform.position);
            if (distance < atkDist)
            {
                if (!isPlayingSelfAudio)
                {
                    isPlayingSelfAudio = true;
                    anim.SetBool("IsAttack", true);
                    selfAudioSource.Play();
                }
                target = enemy.GetComponent<Enemy>();
                if (target.hp - atk < 0) PlayEnemySound();
                target.hp -= atk;
            }
        }
        yield return null;
    }

    protected override void SetReadyAnimation()
    {
        if (target == null)
        {
            anim.SetBool("IsAttack", false);
            isPlayingSelfAudio = false;
        }
    }
}
