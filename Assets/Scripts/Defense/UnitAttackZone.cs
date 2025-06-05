using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackZone : MonoBehaviour
{
    bool isPlaced = false;
    private Renderer render;
    private Unit parent;

    void Start()
    {
        render = this.GetComponent<Renderer>();
        parent = transform.parent.GetComponent<Unit>();
    }

    void Update()
    {
        if (!isPlaced && parent.isActive)
        {
            isPlaced = true;
            var color = render.material.color;
            var newColor = new Color(color.r, color.g, color.b, 0.1f);
            render.material.color = newColor;
        }
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
