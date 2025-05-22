using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public Vector2 posOffset = new Vector2(270.0f, 85.0f);
    public GUIStyle guistyle; // ��Ʈ��Ÿ��
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

    // ������ �� ���� �����͸� �� ���� �࿡ ���� ǥ��.
    public void printValue(float x, float y, string label, int value)
    {
        float labelWidth = 100.0f;
        float _x = (Screen.width - labelWidth) / 2f + x;
        GUI.Label(new Rect(_x, y, labelWidth, 20), label, guistyle); // label�� ǥ��
        y += 15;
        GUI.Label(new Rect(_x, y, labelWidth, 20), value.ToString(), guistyle); // ���� �࿡ value�� ǥ��
        y += 15;
    }

    public void PointUp(Block.COLOR key, int count = 1)
    {
        this.score[key] += count;
    }
}
