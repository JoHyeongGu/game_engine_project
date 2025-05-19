using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
     // 점수 관리용 구조체
    public struct Count
    {
        public int ignite; // 연쇄수
        public int score; // 점수
        public int totalSocre; // 합계점수
    };

    public Count last; // 마지막(이번) 점수
    public Count best; // 최고점수
    public static int QUOTA_SCORE = 1000; // 클리어하는데필요한점수
    public GUIStyle guistyle; // 폰트스타일

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
        this.printValue(x + 20, y, "연쇄 카운트", this.last.ignite);
        y += 30;
        this.printValue(x + 20, y, "가산 스코어", this.last.score);
        y += 30;
        this.printValue(x + 20, y, "합계 스코어", this.last.totalSocre);
        y += 30;
    }

    // 지정된 두 개의 데이터를 두 개의 행에 나눠 표시.
    public void printValue(int x, int y, string label, int value)
    {
        GUI.Label(new Rect(x, y, 100, 20), label, guistyle); // label을 표시
        y += 15;
        GUI.Label(new Rect(x + 20, y, 100, 20), value.ToString(), guistyle); // 다음 행에 value를 표시
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
}
