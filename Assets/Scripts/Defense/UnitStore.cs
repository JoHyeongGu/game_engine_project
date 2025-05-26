using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Price
{
    public Block.COLOR key;
    public int value;
}

[System.Serializable]
public struct UnitItem
{
    public GameObject prefab;
    public Texture2D image;
    public Texture2D unableImage;
    public Price[] price;
}

public class UnitStore : MonoBehaviour
{
    public UnitItem[] units;
    public Enemy target;

    private BlockRoot blockRoot;
    private ScoreCounter scoreCounter;
    private Unit SelectedUnit;

    void Start()
    {
        blockRoot = this.gameObject.GetComponent<BlockRoot>();
        scoreCounter = this.gameObject.GetComponent<ScoreCounter>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && SelectedUnit != null && SelectedUnit.canPlaced)
        {
            SelectedUnit.Active();
            SelectedUnit = null;
        }
        if (SelectedUnit != null)
        {
            SelectedUnit.SetMousePosition(blockRoot.mousePosition);
        }
    }

    void OnGUI()
    {
        float[] panelInfo = DrawStorePanel();
        DrawContent(panelInfo);
    }

    private float[] DrawStorePanel()
    {
        float width = Screen.width / 2f;
        float height = 90f;
        float padding = 10;
        var rect = new Rect(padding, Screen.height - height - padding, width, height);
        GUI.Box(rect, "유닛 상점");
        return new float[] { width, height, padding };
    }

    private void DrawContent(float[] panelInfo)
    {
        float padding = 10;
        float width = panelInfo[0] / 5;
        float height = panelInfo[1] - (padding * 2);

        float yPos = Screen.height - (height + padding) - panelInfo[2];

        for (int i = 0; i < units.Length; i++)
        {
            UnitItem unit = units[i];
            float xPos = panelInfo[2] + padding + (i * (width + padding));
            var rect = new Rect(xPos, yPos, width, height);
            bool canBuy = true;
            foreach (Price p in unit.price)
            {
                if (scoreCounter.GetPoint(p.key) < p.value)
                {
                    canBuy = false;
                }
            }
            if (!canBuy)
            {
                GUI.Box(rect, new GUIContent(unit.unableImage));
            }
            else
            {
                if (GUI.Button(rect, new GUIContent(unit.image)))
                {
                    this.BuyUnit(unit);
                    GameObject unitObject = Instantiate(unit.prefab, blockRoot.mousePosition, Quaternion.Euler(-90.0f, 0, 0));
                    SelectedUnit = unitObject.GetComponent<Unit>();
                }
            }
        }
    }

    private void BuyUnit(UnitItem unit)
    {
        foreach (Price p in unit.price)
        {
            // 포인트 감소
            scoreCounter.PointUp(p.key, -p.value);
        }
    }
}
