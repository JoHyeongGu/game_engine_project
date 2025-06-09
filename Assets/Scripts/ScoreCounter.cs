using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public GUIStyle guistyle; // 폰트스타일
    public Dictionary<Block.COLOR, int> score = new Dictionary<Block.COLOR, int>();
    public Dictionary<Block.COLOR, Texture2D> imageDict;
    public bool isLocked = false;

    void Start()
    {
        this.guistyle.fontSize = 16;
        InitScore();
    }

    void InitScore()
    {
        for (int i = 0; i <= (int)Block.COLOR.LAST; i++)
        {
            score[(Block.COLOR)i] = 0;
        }
    }

    void OnGUI()
    {
        float ratio = Screen.height / 1080f;
        float x = Screen.width / 2 - 15f * ratio;
        float y = Screen.height / 10;
        GUI.color = Color.black;
        foreach (KeyValuePair<Block.COLOR, int> data in score)
        {
            PrintScore(x, y, data);
            y += Screen.height / 16;
        }
    }

    public void PrintScore(float x, float y, KeyValuePair<Block.COLOR, int> data)
    {
        float scaleY = Screen.width / 500f;

        float lineHeight = 10f * scaleY;
        float boxSize = 12f * scaleY;
        float spacing = 5f * scaleY;
        float outlineOffset = 0.5f * scaleY;

        GUIStyle textStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
            fontSize = Mathf.RoundToInt(10f * scaleY)
        };

        Texture2D image = imageDict[data.Key];
        float ratio = (float)image.height / image.width;

        Vector2 imagePos = new Vector2(x, y + 4f * scaleY);
        if (data.Key == Block.COLOR.YELLOW) imagePos.y += 13f;
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
        string priceText = $"{data.Value}";
        Rect textRect = new Rect(x + boxSize * 0.7f + spacing, y + boxSize / 3, 30f * scaleY, lineHeight);
        DrawOutlinedLabel(textRect, priceText, textStyle, Color.white, Color.black, 2f);
        GUI.color = Color.white;
    }

    public void PointUp(Block.COLOR key, int count = 1)
    {
        if (isLocked) return;
        this.score[key] += count;
    }

    public float GetPoint(Block.COLOR key)
    {
        return this.score[key];
    }

    public void SetImageDict(PriceImage[] priceImages)
    {
        imageDict = new Dictionary<Block.COLOR, Texture2D>();
        foreach (PriceImage data in priceImages)
        {
            imageDict[data.key] = data.image;
        }
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
}
