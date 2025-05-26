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

    void OnTriggerStay(Collider other)
    {
        if (!parent.isActive) return;
        GameObject otherObj = other.gameObject;
        if (otherObj.CompareTag("Enemy") && !parent.targetList.Contains(otherObj))
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
