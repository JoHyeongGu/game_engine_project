using System.Collections.Generic;
using UnityEngine;

// �� ������ ���� �����͸� �����ϴ� List ��
public class LevelData
{
    public float[] probability; // ����� �����󵵸� �����ϴ� �迭
    public float heatTime; // ���ҽð�
    
    public LevelData()
    {
        this.probability = new float[(int)Block.COLOR.NORMAL_COLOR_NUM];
        for (int i = 0; i < (int)Block.COLOR.NORMAL_COLOR_NUM; i++)
        {
            this.probability[i] = 1.0f / (float)Block.COLOR.NORMAL_COLOR_NUM;
        }
    }

    // ��� ������ ���� Ȯ���� 0���� �����ϴ� �޼ҵ�
    public void clear()
    { 
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] = 0.0f;
        }
    }
    
    // ��� ������ ����Ȯ���� �հ踦 100%(=1.0)�� �ϴ� �޼ҵ�
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
              ����� ���� ���� ���� ũ��� ���̾� ������ Ȯ��
              ��� ������ ����Ȯ���� �켱 �յ��ϰ�
              ����Ȯ���� '�ӽ� �հ谪'�� ���
              ������ ����Ȯ���� '�ӽ� �հ谪'���� ������, �հ谡 100%(=1.0) �� ������
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
    private List<LevelData> levelDatas = null; // �� ������ ���� ������
    private int selectLevel = 0; // ���õ� ����


    public void initialize(int stage, int wave, int maxWave)
    {
        this.stage = stage;
        this.wave = wave;
        this.maxWave = maxWave;
        this.levelDatas = new List<LevelData>();
    }
    
    // �ؽ�Ʈ �����͸� �о�ͼ� �� ������ �ؼ��ϰ� �����͸� ����
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
                // 8�׸�(�̻�)�� ����� ó���Ǿ��ٸ�.
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
        // levelDatas�� �����Ͱ� �ϳ��� ������
        if (this.levelDatas.Count == 0)
        {
            // ���� �޽����� ǥ��
            Debug.LogError("[LevelData] Has no data.\n");
            // levelDatas�� LevelData�� �ϳ� �߰�
            this.levelDatas.Add(new LevelData());
        }
    }

    // �� ���� ���� ���Ͽ��� ���� ����� ������ ����        
    public void SelectLevel()
    { 
        this.selectLevel = ((stage - 1) * maxWave) + (wave - 1);
    }

    // ���õǾ� �ִ� ���� ������ ���� �����͸� ��ȯ
    public LevelData GetCurrentLevelData()
    { 
        // ���õ� ������ ���� �����͸� ��ȯ
        return (this.levelDatas[this.selectLevel]);
    }

    // ���õǾ� �ִ� ���� ������ ���� �ð��� ��ȯ
    public float GetVanishTime()
    { 
        // ���õ� ������ ���ҽð��� ��ȯ
        return (this.levelDatas[this.selectLevel].heatTime);
    }
}