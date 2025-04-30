using System.Collections.Generic;
using UnityEngine;

// List 데이터 구조를 사용하기 위해 필요
public class LevelData
{ // 각 레벨의 레벨 데이터를 저장하는 List 값
    public float[] probability;
    // 블록의 출현빈도를 저장하는 배열
    public float heatTime;
    public LevelData()
    {
        // 연소시간
        // 생성자
        this.probability = new float[(int)Block.COLOR.NORMAL_COLOR_NUM];
        for (int i = 0; i < (int)Block.COLOR.NORMAL_COLOR_NUM; i++)
        {
            this.probability[i] = 1.0f / (float)Block.COLOR.NORMAL_COLOR_NUM;
        }
    }
    public void clear()
    { // 모든 종류의 출현확률을 0으로 리셋하는 메소드
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] = 0.0f;
        }
    }
    public void normalize()
    {// 모든 종류의 출현확률의 합계를 100%(=1.0)로 하는 메소드
        float sum = 0.0f;
        for (int i = 0; i < this.probability.Length; i++)
        {
            sum += this.probability[i];
        }
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] /= sum;
            // 블록의 종류 수와 같은 크기로 데이어 영역을 확보
            // 모든 종류의 출현확률을 우선 균등하게
            // 출현확률의 '임시 합계값'을 계산
            // 각각의 출현확률을 '임시 합계값'으로 나누면, 합계가 100%(=1.0) 딱 떨어짐
            if (float.IsInfinity(this.probability[i]))
            {
                this.clear();
                this.probability[0] = 1.0f;
                break;
            }
        }
    }
}

public class LevelControl
{
    private List<LevelData> levelDatas = null;
    private int selectLevel = 0;
    // 각 레벨의 레벨 데이터
    // 선택된 레벨
    public void initialize() { this.levelDatas = new List<LevelData>(); }
    // List를 초기화
    public void loadLevelData(TextAsset levelDataText)
    { // 텍스트 데이터를 읽어와서 그 내용을 해석하고 데이터를 보관
        string levelTexts = levelDataText.text;
        // 텍스트 데이터를 문자열로서 받아들인다       
        string[] lines = levelTexts.Split('\n');
        foreach (var line in lines)
        {
            if (line == "" || line.Contains("#"))
            {
                continue;
            }
            string[] words = line.Split("\t");
            int n = 0;
            LevelData levelData = new();
            foreach (var word in words)
            {
                if (word == "") { continue; }
                switch (n)
                {
                    case 0: levelData.probability[(int)Block.COLOR.PINK] = float.Parse(word); break;
                    case 1: levelData.probability[(int)Block.COLOR.BLUE] = float.Parse(word); break;
                    case 2: levelData.probability[(int)Block.COLOR.GREEN] = float.Parse(word); break;
                    case 3: levelData.probability[(int)Block.COLOR.ORANGE] = float.Parse(word); break;
                    case 4: levelData.probability[(int)Block.COLOR.YELLOW] = float.Parse(word); break;
                    case 5: levelData.probability[(int)Block.COLOR.MAGENTA] = float.Parse(word); break;
                    case 6: levelData.heatTime = float.Parse(word); break;
                }
                n++;
            }
            if (n >= 7)
            {
                // 8항목(이상)이 제대로 처리되었다면.
                levelData.normalize();
                this.levelDatas.Add(levelData);
            }
            else
            {
                if (n == 0)
                { 
                }
                else
                {
                    Debug.LogError("[LevelData] Out of parameter.\n");
                }
            }
        }
        // levelDatas에 데이터가 하나도 없으면
        if (this.levelDatas.Count == 0)
        {
            // 오류 메시지를 표시
            Debug.LogError("[LevelData] Has no data.\n");
            // levelDatas에 LevelData를 하나 추가
            this.levelDatas.Add(new LevelData());
        }
    }

    public void SelectLevel()
    { // 몇 개의 레벨 패턴에서 지금 사용할 패턴을 선택        
      // 0~패턴 사이의 값을 임의로 선택
        this.selectLevel = Random.Range(0, this.levelDatas.Count);
        Debug.Log("select level = " + this.selectLevel.ToString());
    }

    public LevelData GetCurrentLevelData()
    { // 선택되어 있는 레벨 패턴의 레벨 데이터를 반환
      // 선택된 패턴의 레벨 데이터를 반환
        return (this.levelDatas[this.selectLevel]);
    }

    public float GetVanishTime()
    { // 선택되어 있는 레벨 패턴의 연소 시간을 반환
      // 선택된 패턴의 연소시간을 반환
        return (this.levelDatas[this.selectLevel].heatTime);
    }
}