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
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fontSize = 12;
            if (!canBuy)
            {
                GUI.Box(rect, new GUIContent(unit.unableImage), style);
                DrawPrice(unit.price, new Vector2(xPos, yPos));
            }
            else
            {
                if (GUI.Button(rect, new GUIContent(unit.image), style))
                {
                    this.BuyUnit(unit);
                    GameObject unitObject = Instantiate(unit.prefab, blockRoot.mousePosition, Quaternion.Euler(-90.0f, 0, 0));
                    SelectedUnit = unitObject.GetComponent<Unit>();
                }
                DrawPrice(unit.price, new Vector2(xPos, yPos));
            }
        }
    }

    private Color GetColorFromBlock(Block.COLOR color)
    {
        switch (color)
        {
            case Block.COLOR.RED: return Color.red;
            case Block.COLOR.BLUE: return Color.blue;
            case Block.COLOR.GREEN: return Color.green;
            case Block.COLOR.YELLOW: return Color.yellow;
            case Block.COLOR.MAGENTA: return Color.magenta;
            case Block.COLOR.ORANGE: return new Color(1.0f, 0.46f, 0.0f);
            default: return Color.gray;
        }
    }

    private void DrawPrice(Price[] datas, Vector2 startPosition)
    {
        float lineHeight = 20f;
        float boxSize = 16f;
        float spacing = 6f;

        GUIStyle textStyle = new GUIStyle();
        textStyle.normal.textColor = Color.white;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.fontSize = 17;

        for (int i = 0; i < datas.Length; i++)
        {
            Price data = datas[i];

            float y = startPosition.y + i * (lineHeight + 4f);
            Rect colorBoxRect = new Rect(startPosition.x, y + 4, boxSize, boxSize);
            GUI.color = GetColorFromBlock(data.key);
            GUI.DrawTexture(colorBoxRect, Texture2D.whiteTexture);
            GUI.color = Color.white;
            string priceText = $"{data.value}";
            Rect textRect = new Rect(startPosition.x + boxSize + spacing, y, 100, lineHeight);
            GUI.Label(textRect, priceText, textStyle);
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
