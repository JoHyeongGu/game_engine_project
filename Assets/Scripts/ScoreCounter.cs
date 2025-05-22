using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public Vector2 posOffset = new Vector2(270.0f, 85.0f);
    public GUIStyle guistyle; // 폰트스타일
    public Dictionary<Block.COLOR, int> score = new Dictionary<Block.COLOR, int>();

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
        float x = posOffset.x;
        float y = posOffset.y;
        GUI.color = Color.black;
        foreach (KeyValuePair<Block.COLOR, int> data in score)
        {
            this.printValue(x, y, $"{data.Key}", data.Value);
            y += 30;
        }
    }

    // 지정된 두 개의 데이터를 두 개의 행에 나눠 표시.
    public void printValue(float x, float y, string label, int value)
    {
        float labelWidth = 100.0f;
        float _x = (Screen.width - labelWidth) / 2f + x;
        GUI.Label(new Rect(_x, y, labelWidth, 20), label, guistyle); // label을 표시
        y += 15;
        GUI.Label(new Rect(_x, y, labelWidth, 20), value.ToString(), guistyle); // 다음 행에 value를 표시
        y += 15;
    }

    public void PointUp(Block.COLOR key, int count = 1)
    {
        this.score[key] += count;
    }
}
