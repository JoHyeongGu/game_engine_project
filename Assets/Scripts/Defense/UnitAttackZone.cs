using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackZone : MonoBehaviour
{
    bool canChangeColor = true;
    private Renderer render;
    private Unit parent;
    private Color color;

    void Start()
    {
        render = this.GetComponent<Renderer>();
        parent = this.transform.parent.GetComponent<Unit>();
        color = render.material.color;
        SetScale();
    }

    void Update()
    {
        if (!canChangeColor) return;
        if (parent.isActive)
        {
            canChangeColor = false;
            ChangeColor(new Color(color.r, color.g, color.b, 0.1f));
            return;
        }
        if (parent.canPlaced) ChangeColor(new Color(color.r, color.g * 2, color.b, color.a * 0.7f));
        else ChangeColor(new Color(color.r * 2, color.g, color.b, color.a));
    }

    private void SetScale()
    {
        float diameter = parent.atkDist * 2f;
        this.transform.localScale = new Vector3(diameter, 0.1f, diameter);
    }

    private void ChangeColor(Color newColor)
    {
        render.material.color = newColor;
    }
}
