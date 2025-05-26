using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreTutorial : MonoBehaviour
{
    public Dictionary<Block.COLOR, int> datas = new Dictionary<Block.COLOR, int>();

    private Vector2 posOffset = new Vector2(270.0f, 85.0f);
    private GUIStyle guistyle = new GUIStyle();
    private float time = 17.0f;

    void Start()
    {
        for (int i = 0; i <= (int)Block.COLOR.LAST; i++)
        {
            datas[(Block.COLOR)i] = 0;
        }
    }

    void Update()
    {
        // time += Time.deltaTime;
    }

    void OnGUI()
    {
        guistyle.fontSize = 16;
        float x = 0.0f;
        float y = posOffset.y;
        GUI.color = Color.black;
        foreach (KeyValuePair<Block.COLOR, int> data in datas)
        {
            this.printValue(x, y, $"{data.Key}", data.Value);
            y += 30;
        }

        guistyle.fontSize = 24;
        GUI.color = Color.black;
        GUI.Label(new Rect(10.0f, 10.0f, 200.0f, 20.0f), $"Stage 0", guistyle);
        GUI.Label(new Rect(10.0f, 40.0f, 200.0f, 20.0f), $"Wave 0", guistyle);
        GUI.Label(new Rect(10.0f, 70.0f, 200.0f, 20.0f), $"{time} 초", guistyle);
        GUI.color = Color.white;
    }

    public void printValue(float x, float y, string label, int value)
    {
        float labelWidth = 100.0f;
        float _x = (Screen.width - labelWidth) / 2f + x;
        GUI.Label(new Rect(_x, y, labelWidth, 20), label, guistyle); // label을 표시
        y += 15;
        GUI.Label(new Rect(_x, y, labelWidth, 20), value.ToString(), guistyle); // 다음 행에 value를 표시
        y += 15;
    }
}
