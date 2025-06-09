using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float atk;
    public float atkDist;
    public float cooltime;
    public float animCooltime;

    public float rotateOffset = -90f;
    public bool isActive = false;
    public bool canPlaced = false;

    public AudioSource selfAudioSource;
    public AudioSource enemyAudioSource;

    public ScoreCounter scoreCounter = null;
    protected Vector3 mousePosition;
    protected Animator anim;
    public Price[] price;

    protected Enemy target;

    void Start()
    {
        anim = this.GetComponentInChildren<Animator>();
        StartCoroutine(AttackRoutine());
    }

    protected virtual void Update()
    {
        if (!isActive)
        {
            this.transform.position = mousePosition;
            if (Input.GetMouseButtonDown(1)) CancelBuy(); // 구매 취소
            CheckCanPlaced();
        }
        else
        {
            SetTarget();
            SetReadyAnimation();
        }
        LookTarget();
    }

    private void CancelBuy()
    {
        foreach (Price p in price)
        {
            scoreCounter.PointUp(p.key, p.value);
        }
        Destroy(this.gameObject);
        Destroy(this);
    }

    protected virtual IEnumerator AttackRoutine()
    {
        while (true)
        {
            while (!isActive || target == null)
            {
                yield return null;
            }
            yield return new WaitForSeconds(cooltime);
            if (HasAnimParam("Attack")) anim.SetTrigger("Attack");
            if (target != null) yield return Attack();
        }
    }

    protected virtual IEnumerator Attack()
    {
        selfAudioSource.Play();
        yield return new WaitForSeconds(animCooltime);
        if (target != null)
        {
            PlayEnemySound();
            target.Attacked(atk);
        }
        yield return null;
    }

    protected void PlayEnemySound()
    {
        if (target.hp - atk <= 0)
        {
            if (target.isSplited) PlayEnemyClip(target.sounds.splited);
            else PlayEnemyClip(target.sounds.destroy);
        }
        else PlayEnemyClip(target.sounds.attacked);
    }

    private void PlayEnemyClip(AudioClip audio)
    {
        enemyAudioSource.clip = audio;
        enemyAudioSource.Play();
    }

    protected virtual void SetTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(this.transform.position, enemy.transform.position);
            if (distance > atkDist) continue;
            target = enemy.GetComponent<Enemy>();
            return;
        }
        target = null;
    }

    protected virtual void SetReadyAnimation()
    {
        if (!HasAnimParam("IsAttack")) return;
        anim.SetBool("IsAttack", target != null);
    }

    public virtual void LookTarget()
    {
        Vector3 _target = Vector3.zero;
        if (!isActive)
        {
            _target = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        }
        else if (target != null)
        {
            _target = target.transform.position;
        }
        if (_target == Vector3.zero) return;

        Vector3 localDir = transform.parent.InverseTransformPoint(_target) - transform.localPosition;
        localDir.y = 0f;
        if (localDir == Vector3.zero) return;
        Quaternion lookRotation = Quaternion.LookRotation(localDir);
        Quaternion offsetRotation = Quaternion.AngleAxis(rotateOffset, Vector3.up);
        transform.localRotation = lookRotation * offsetRotation;
    }

    public void SetMousePosition(Vector3 mousePosition)
    {
        this.mousePosition = mousePosition;
    }

    protected void CheckCanPlaced()
    {
        Ray ray = new Ray(transform.position, Vector3.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Wall") && !IsNearUnit())
            {
                canPlaced = true;
            }
            else canPlaced = false;
        }
        else canPlaced = false;
    }

    private bool IsNearUnit()
    {
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject unit in units)
        {
            if (unit == this.gameObject) continue;
            float distance = Vector3.Distance(this.transform.position, unit.transform.position);
            if (distance < 0.5f) return true;
        }
        return false;
    }

    public bool HasAnimParam(string paramName)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }
}
