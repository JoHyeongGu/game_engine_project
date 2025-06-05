using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool isActive = false;
    public bool canPlaced = false;
    public List<GameObject> targetList;

    protected Vector3 mousePosition;
    protected Renderer render;

    protected float invisible = 0.3f;
    protected float visible = 1.0f;
    protected float canOpacity = 0.7f;
    protected bool isAttacking = false;
    protected Coroutine AttackRoutine;
    protected Animator anim;
    protected GameObject target;
    protected Transform prefab;

    public Price[] price;
    public ScoreCounter scoreCounter = null;

    void Start()
    {
        anim = this.GetComponentInChildren<Animator>();
        render = gameObject.GetComponent<Renderer>();
        targetList = new List<GameObject>();
        prefab = transform.GetChild(transform.childCount - 1);
        this.SetOpacity(invisible);
    }

    protected virtual void Update()
    {
        if (!isActive)
        {
            this.transform.position = mousePosition;
            // 구매 취소
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
        }
        else if (targetList.Count > 0)
        {
            if (!isAttacking)
            {
                AttackRoutine = StartCoroutine(Attack());
            }
        }
        else
        {
            if (AttackRoutine != null && isAttacking)
            {
                isAttacking = false;
                StopCoroutine(AttackRoutine);
            }
        }
        LookTarget();
    }

    public void LookTarget()
    {
        if (target != null)
        {
            Vector3 localDir = transform.parent.InverseTransformPoint(target.transform.position) - transform.localPosition;
            localDir.y = 0f;

            if (localDir == Vector3.zero) return;

            Quaternion lookRotation = Quaternion.LookRotation(localDir);
            Quaternion offsetRotation = Quaternion.AngleAxis(-90f, Vector3.up); // 로컬 Y축 기준 -30도

            transform.localRotation = lookRotation * offsetRotation;
        }
    }

    public void SetMousePosition(Vector3 mousePosition)
    {
        this.mousePosition = mousePosition;
    }

    public void Active()
    {
        isActive = true;
        this.SetOpacity(visible);
    }

    private void SetOpacity(float alpha)
    {
        var color = render.material.color;
        var newColor = new Color(color.r, color.g, color.b, alpha);
        render.material.color = newColor;
    }

    protected void CheckCanPlaced()
    {
        Ray ray = new Ray(transform.position, Vector3.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Wall"))
            {
                SetOpacity(canOpacity);
                canPlaced = true;
            }
            else
            {
                SetOpacity(invisible);
                canPlaced = false;
            }
        }
        else
        {
            SetOpacity(invisible);
            canPlaced = false;
        }
    }

    protected virtual IEnumerator Attack()
    {
        isAttacking = true;
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (targetList.Count == 0) break;
            GameObject firstEnemy = targetList[0];
            if (firstEnemy == null)
            {
                target = null;
                targetList.Remove(targetList[0]);
                continue;
            }
            Enemy enemyClass = firstEnemy.GetComponent<Enemy>();
            if (enemyClass == null)
            {
                target = null;
                targetList.Remove(targetList[0]);
                continue;
            }
            target = firstEnemy;
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(0.4f);
            enemyClass.hp -= 0.5f;
            if (enemyClass.hp <= 0)
            {
                target = null;
                targetList.Remove(targetList[0]);
            }
        }
    }
}
