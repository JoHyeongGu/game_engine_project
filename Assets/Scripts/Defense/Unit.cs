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

    void Start()
    {
        render = gameObject.GetComponent<Renderer>();
        targetList = new List<GameObject>();
        this.SetOpacity(invisible);
    }

    void Update()
    {
        if (!isActive)
        {
            this.transform.position = mousePosition;
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

    private void CheckCanPlaced()
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
                targetList.Remove(targetList[0]);
                continue;
            }
            Enemy enemyClass = firstEnemy.GetComponent<Enemy>();
            if (enemyClass == null)
            {
                targetList.Remove(targetList[0]);
                continue;
            }
            enemyClass.hp--;
            if (enemyClass.hp <= 0)
            {
                targetList.Remove(targetList[0]);
            }
        }
    }
}
