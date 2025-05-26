using System.Collections.Generic;
using UnityEngine;

// 각 레벨의 레벨 데이터를 저장하는 List 값
public class LevelData
{
    public float[] probability; // 블록의 출현빈도를 저장하는 배열
    public float heatTime; // 연소시간
    
    public LevelData()
    {
        this.probability = new float[(int)Block.COLOR.NORMAL_COLOR_NUM];
        for (int i = 0; i < (int)Block.COLOR.NORMAL_COLOR_NUM; i++)
        {
            this.probability[i] = 1.0f / (float)Block.COLOR.NORMAL_COLOR_NUM;
        }
    }

    // 모든 종류의 출현 확률을 0으로 리셋하는 메소드
    public void clear()
    { 
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] = 0.0f;
        }
    }
    
    // 모든 종류의 출현확률의 합계를 100%(=1.0)로 하는 메소드
    public void normalize()
    {
        float sum = 0.0f;
        for (int i = 0; i < this.probability.Length; i++)
        {
            sum += this.probability[i];
        }
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] /= sum;
            /*
              블록의 종류 수와 같은 크기로 데이어 영역을 확보
              모든 종류의 출현확률을 우선 균등하게
              출현확률의 '임시 합계값'을 계산
              각각의 출현확률을 '임시 합계값'으로 나누면, 합계가 100%(=1.0) 딱 떨어짐
            */
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
    public int stage;
    public int wave;

    private int maxWave;
    private List<LevelData> levelDatas = null; // 각 레벨의 레벨 데이터
    private int selectLevel = 0; // 선택된 레벨


    public void initialize(int stage, int wave, int maxWave)
    {
        this.stage = stage;
        this.wave = wave;
        this.maxWave = maxWave;
        this.levelDatas = new List<LevelData>();
    }
    
    // 텍스트 데이터를 읽어와서 그 내용을 해석하고 데이터를 보관
    public void loadLevelData(TextAsset levelDataText)
    { 
        string levelTexts = levelDataText.text;
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
                    case 0: levelData.probability[(int)Block.COLOR.RED] = float.Parse(word); break;
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

    // 몇 개의 레벨 패턴에서 지금 사용할 패턴을 선택        
    public void SelectLevel()
    { 
        this.selectLevel = ((stage - 1) * maxWave) + (wave - 1);
    }

    // 선택되어 있는 레벨 패턴의 레벨 데이터를 반환
    public LevelData GetCurrentLevelData()
    { 
        // 선택된 패턴의 레벨 데이터를 반환
        return (this.levelDatas[this.selectLevel]);
    }

    // 선택되어 있는 레벨 패턴의 연소 시간을 반환
    public float GetVanishTime()
    { 
        // 선택된 패턴의 연소시간을 반환
        return (this.levelDatas[this.selectLevel].heatTime);
    }
}