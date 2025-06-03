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
        const float baseWidth = 1920f;
        const float baseHeight = 1080f;

        float scaleX = Screen.width / baseWidth;
        float scaleY = Screen.height / baseHeight;

        float panelWidth = 1600f * scaleX;
        float panelHeight = 90f * scaleY;
        float paddingX = 160f * scaleX;
        float paddingY = 20f * scaleY;

        Rect rect = new Rect(paddingX, Screen.height - panelHeight - paddingY, panelWidth, panelHeight);
        GUI.Box(rect, "유닛 상점");

        return new float[] { panelWidth, panelHeight, paddingX, paddingY, scaleX, scaleY };
    }

    private void DrawContent(float[] panelInfo)
    {
        float panelWidth = panelInfo[0];
        float panelHeight = panelInfo[1];
        float paddingX = panelInfo[2];
        float paddingY = panelInfo[3];
        float scaleX = panelInfo[4];
        float scaleY = panelInfo[5];

        float unitBoxWidth = 300f * scaleX;
        float unitBoxHeight = 70f * scaleY;
        float spacing = 10f * scaleX;

        float startX = paddingX + spacing;
        float yPos = Screen.height - panelHeight - paddingY + 10f * scaleY;

        int maxDisplay = Mathf.Min(units.Length, 5);

        for (int i = 0; i < maxDisplay; i++)
        {
            UnitItem unit = units[i];
            float xPos = startX + i * (unitBoxWidth + spacing);
            Rect rect = new Rect(xPos, yPos, unitBoxWidth, unitBoxHeight);

            bool canBuy = true;
            foreach (Price p in unit.price)
            {
                if (scoreCounter.GetPoint(p.key) < p.value)
                {
                    canBuy = false;
                    break;
                }
            }

            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fontSize = Mathf.RoundToInt(14f * scaleY);

            if (!canBuy)
            {
                GUI.Box(rect, new GUIContent(unit.unableImage), style);
            }
            else
            {
                if (GUI.Button(rect, new GUIContent(unit.image), style))
                {
                    BuyUnit(unit);
                    GameObject unitObject = Instantiate(unit.prefab, blockRoot.mousePosition, Quaternion.Euler(-90.0f, 0, 0));
                    SelectedUnit = unitObject.GetComponent<Unit>();
                }
            }

            DrawPrice(unit.price, new Vector2(xPos + 5f * scaleX, yPos + 5f * scaleY), scaleY);
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

    private void DrawPrice(Price[] datas, Vector2 startPosition, float scaleY)
    {
        float lineHeight = 20f * scaleY;
        float boxSize = 16f * scaleY;
        float spacing = 6f * scaleY;

        GUIStyle textStyle = new GUIStyle();
        textStyle.normal.textColor = Color.white;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.fontSize = Mathf.RoundToInt(17f * scaleY);

        for (int i = 0; i < datas.Length; i++)
        {
            Price data = datas[i];

            float y = startPosition.y + i * (lineHeight + 4f * scaleY);
            Rect colorBoxRect = new Rect(startPosition.x, y + 4f * scaleY, boxSize, boxSize);
            GUI.color = GetColorFromBlock(data.key);
            GUI.DrawTexture(colorBoxRect, Texture2D.whiteTexture);
            GUI.color = Color.white;

            string priceText = $"{data.value}";
            Rect textRect = new Rect(startPosition.x + boxSize + spacing, y, 100f * scaleY, lineHeight);
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
