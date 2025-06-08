using UnityEngine;

[System.Serializable]
public struct PriceImage
{
    public Block.COLOR key;
    public Texture2D image;
}

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
    public PriceImage[] priceImages;

    public UnitItem[] units;
    public Enemy target;

    private BlockRoot blockRoot;
    private ScoreCounter scoreCounter;
    private Unit SelectedUnit;

    void Awake()
    {
        blockRoot = this.gameObject.GetComponent<BlockRoot>();
        scoreCounter = this.gameObject.GetComponent<ScoreCounter>();
        scoreCounter.SetImageDict(priceImages);
    }

    void Update()
    {
        if (blockRoot.isPaused) return;
        if (Input.GetMouseButtonDown(0) && SelectedUnit != null && SelectedUnit.canPlaced)
        {
            SelectedUnit.isActive = true;
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
        float y = Screen.height - (height * 1.1f);
        Rect rect = new Rect(x, y, width, height);

        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.normal.background = activeCard;
        style.hover.background = blockRoot.isPaused ? activeCard : hoverCard;
        style.active.background = blockRoot.isPaused ? activeCard : clickCard;

        bool isActive = true;
        foreach (Price p in unit.price)
        {
            if (scoreCounter.GetPoint(p.key) < p.value)
            {
                isActive = false;
                break;
            }
        }

        Color priceColor = Color.white;
        if (isActive)
        {
            GUIContent content = new GUIContent(new GUIContent(unit.image));
            if (GUI.Button(rect, content, style) && !blockRoot.isPaused)
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
            priceColor = Color.black;
        }
        float scaleY = Screen.width / 550f;
        DrawPrice(unit.price, new Vector2(x + 10, y + 5), scaleY, priceColor);
    }

    private Texture2D GetImageFromBlockColor(Block.COLOR color)
    {
        foreach (PriceImage data in priceImages)
        {
            if (color == data.key) return data.image;
        }
        return null;
    }

    private void DrawOutlinedLabel(Rect rect, string text, GUIStyle style, Color mainColor, Color outlineColor, float offset)
    {
        Vector2[] directions = {
            new Vector2(-offset, 0), new Vector2(offset, 0),
            new Vector2(0, -offset), new Vector2(0, offset),
            new Vector2(-offset, -offset), new Vector2(-offset, offset),
            new Vector2(offset, -offset), new Vector2(offset, offset)
        };

        Color originalColor = style.normal.textColor;

        style.normal.textColor = outlineColor;
        foreach (Vector2 dir in directions)
        {
            Rect offsetRect = new Rect(rect.x + dir.x, rect.y + dir.y, rect.width, rect.height);
            GUI.Label(offsetRect, text, style);
        }

        style.normal.textColor = mainColor;
        GUI.Label(rect, text, style);

        style.normal.textColor = originalColor;
    }
    private void DrawPrice(Price[] datas, Vector2 startPos, float scaleY, Color priceColor)
    {
        float lineHeight = 10f * scaleY;
        float boxSize = 12f * scaleY;
        float spacing = 5f * scaleY;
        float outlineOffset = 0.5f * scaleY;

        GUIStyle textStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
            fontSize = Mathf.RoundToInt(10f * scaleY)
        };

        for (int i = 0; i < datas.Length; i++)
        {
            Price data = datas[i];

            float y = startPos.y + i * (lineHeight + 4f * scaleY);
            Texture2D image = GetImageFromBlockColor(data.key);
            float ratio = (float)image.height / image.width;

            Vector2 imagePos = new Vector2(startPos.x, y + 4f * scaleY);
            if (data.key == Block.COLOR.YELLOW) imagePos.y += 13f;
            Rect imageRect = new Rect(imagePos.x, imagePos.y, boxSize, boxSize * ratio);
            Rect outlineRect = new Rect(
                imageRect.x - outlineOffset,
                imageRect.y - outlineOffset,
                imageRect.width + outlineOffset * 2,
                imageRect.height + outlineOffset * 2
            );

            // 이미지 윤곽선 + 원본
            GUI.color = Color.black;
            GUI.DrawTexture(outlineRect, image, ScaleMode.ScaleToFit, true);
            GUI.color = Color.white;
            GUI.DrawTexture(imageRect, image, ScaleMode.ScaleToFit, true);

            // 텍스트 윤곽선 + 텍스트
            string priceText = $"{data.value}";
            Rect textRect = new Rect(startPos.x + boxSize * 0.7f + spacing, y + boxSize / 3, 30f * scaleY, lineHeight);
            DrawOutlinedLabel(textRect, priceText, textStyle, Color.white, Color.black, 2f);
        }

        GUI.color = Color.white;
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
