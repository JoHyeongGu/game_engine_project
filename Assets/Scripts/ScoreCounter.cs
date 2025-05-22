using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public struct Count
    {
        public int ignite;
        public int score;
        public int totalSocre;
    };

    public Vector2 posOffset = new Vector2(270.0f, 85.0f);

    public Count last; // ������(�̹�) ����
    public Count best; // �ְ�����
    public static int QUOTA_SCORE = 1000; // Ŭ�����ϴµ��ʿ�������
    public GUIStyle guistyle; // ��Ʈ��Ÿ��

    public Dictionary<Block.COLOR, int> score = new Dictionary<Block.COLOR, int>();

    void Start()
    {
        this.last.score = 0;
        this.last.ignite = 0;
        this.last.totalSocre = 0;
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
    // ���� Ƚ���� ����
    public void AddIgniteCount(int count)
    {
        this.last.ignite += count; // ���� ���� count�� �ջ�
        this.UpdateScore(); // ���� ���
    }
    // ���� Ƚ���� ����
    public void ClearIgniteCount()
    {
        this.last.ignite = 0; // ���� Ƚ�� ����
    }
    // ���ؾ� �� ������ ���
    private void UpdateScore()
    {
        this.last.score = this.last.ignite * 10; // ���� ����
    }
    // �հ� ������ ����
    public void UpdateTotalScore()
    {
        this.last.totalSocre += this.last.score;
    }
    // ������ Ŭ���� �ߴ��� ���� (SceneControl���� ���)
    public bool IsGameClear()
    {
        bool isClear = false;
        // ���� �հ� ������ Ŭ���� ���غ��� ũ��
        if (this.last.totalSocre > QUOTA_SCORE)
        {
            isClear = true;
        }
        return (isClear);
    }

    public void PointUp(Block.COLOR key, int count = 1)
    {
        this.score[key] += count;
    }
}
