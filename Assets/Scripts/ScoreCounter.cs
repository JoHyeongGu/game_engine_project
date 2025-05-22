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

    public Count last; // 마지막(이번) 점수
    public Count best; // 최고점수
    public static int QUOTA_SCORE = 1000; // 클리어하는데필요한점수
    public GUIStyle guistyle; // 폰트스타일

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
    // 연쇄 횟수를 가산
    public void AddIgniteCount(int count)
    {
        this.last.ignite += count; // 연쇄 수에 count를 합산
        this.UpdateScore(); // 점수 계산
    }
    // 연쇄 횟수를 리셋
    public void ClearIgniteCount()
    {
        this.last.ignite = 0; // 연쇄 횟수 리셋
    }
    // 더해야 할 점수를 계산
    private void UpdateScore()
    {
        this.last.score = this.last.ignite * 10; // 점수 갱신
    }
    // 합계 점수를 갱신
    public void UpdateTotalScore()
    {
        this.last.totalSocre += this.last.score;
    }
    // 게임을 클리어 했는지 판정 (SceneControl에서 사용)
    public bool IsGameClear()
    {
        bool isClear = false;
        // 현재 합계 점수가 클리어 기준보다 크면
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
