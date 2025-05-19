using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
     // ���� ������ ����ü
    public struct Count
    {
        public int ignite; // �����
        public int score; // ����
        public int totalSocre; // �հ�����
    };

    public Count last; // ������(�̹�) ����
    public Count best; // �ְ�����
    public static int QUOTA_SCORE = 1000; // Ŭ�����ϴµ��ʿ�������
    public GUIStyle guistyle; // ��Ʈ��Ÿ��

    void Start()
    {
        this.last.ignite = 0;
        this.last.score = 0;
        this.last.totalSocre = 0;
        this.guistyle.fontSize = 16;
    }

    void OnGUI()
    {
        int x = 20;
        int y = 50;
        GUI.color = Color.black;
        this.printValue(x + 20, y, "���� ī��Ʈ", this.last.ignite);
        y += 30;
        this.printValue(x + 20, y, "���� ���ھ�", this.last.score);
        y += 30;
        this.printValue(x + 20, y, "�հ� ���ھ�", this.last.totalSocre);
        y += 30;
    }

    // ������ �� ���� �����͸� �� ���� �࿡ ���� ǥ��.
    public void printValue(int x, int y, string label, int value)
    {
        GUI.Label(new Rect(x, y, 100, 20), label, guistyle); // label�� ǥ��
        y += 15;
        GUI.Label(new Rect(x + 20, y, 100, 20), value.ToString(), guistyle); // ���� �࿡ value�� ǥ��
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
}
