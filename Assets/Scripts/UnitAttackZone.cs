using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackZone : MonoBehaviour
{
    Unit parent;

    void Start()
    {
        parent = transform.parent.GetComponent<Unit>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!parent.isActive) return;
        Debug.Log("자식이 감지한 충돌 대상: " + other.name);
        GameObject otherObj = other.gameObject;
        if (otherObj.CompareTag("Enemy"))
        {
            parent.targetList.Add(otherObj);
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject otherObj = other.gameObject;
        parent.targetList.Remove(otherObj);
    }
}
