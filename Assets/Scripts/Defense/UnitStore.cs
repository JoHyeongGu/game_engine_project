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
    public Texture2D activeCard;
    public Texture2D unactiveCard;
    public Texture2D hoverCard;
    public Texture2D clickCard;

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
        for (int i = 0; i < units.Length; i++)
        {
            DrawUnitCard(i);
        }
    }

    private void DrawUnitCard(int index)
    {
        UnitItem unit = units[index];

        float height = Screen.height / 4.5f;
        float width = height * (4.5f / 5f);
        float x = (width / 5) * (index + 1) + width * index;
        Rect rect = new Rect(x, Screen.height - (height * 1.1f), width, height);

        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.normal.background = activeCard;
        style.hover.background = hoverCard;
        style.active.background = clickCard;


        bool isActive = true;
        foreach (Price p in unit.price)
        {
            if (scoreCounter.GetPoint(p.key) < p.value)
            {
                isActive = false;
                break;
            }
        }

        if (isActive)
        {
            GUIContent content = new GUIContent(new GUIContent(unit.image));
            if (GUI.Button(rect, content, style))
            {
                BuyUnit(unit);
            }
        }
        else
        {
            GUIContent content = new GUIContent(new GUIContent(unit.unableImage));
            style.normal.background = unactiveCard;
            style.hover.background = null;
            style.active.background = null;
            GUI.Box(rect, content, style);
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
        GameObject unitObject = Instantiate(unit.prefab, blockRoot.mousePosition, Quaternion.Euler(-90f, 0, 0));
        GameObject map = GameObject.FindGameObjectWithTag("Map");
        unitObject.transform.SetParent(map.transform);
        SelectedUnit = unitObject.GetComponent<Unit>();
        SelectedUnit.price = unit.price;
        SelectedUnit.scoreCounter = this.scoreCounter;
    }
}
