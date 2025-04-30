using System.Collections.Generic;
using UnityEngine;

// List ������ ������ ����ϱ� ���� �ʿ�
public class LevelData
{ // �� ������ ���� �����͸� �����ϴ� List ��
    public float[] probability;
    // ����� �����󵵸� �����ϴ� �迭
    public float heatTime;
    public LevelData()
    {
        // ���ҽð�
        // ������
        this.probability = new float[(int)Block.COLOR.NORMAL_COLOR_NUM];
        for (int i = 0; i < (int)Block.COLOR.NORMAL_COLOR_NUM; i++)
        {
            this.probability[i] = 1.0f / (float)Block.COLOR.NORMAL_COLOR_NUM;
        }
    }
    public void clear()
    { // ��� ������ ����Ȯ���� 0���� �����ϴ� �޼ҵ�
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] = 0.0f;
        }
    }
    public void normalize()
    {// ��� ������ ����Ȯ���� �հ踦 100%(=1.0)�� �ϴ� �޼ҵ�
        float sum = 0.0f;
        for (int i = 0; i < this.probability.Length; i++)
        {
            sum += this.probability[i];
        }
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] /= sum;
            // ����� ���� ���� ���� ũ��� ���̾� ������ Ȯ��
            // ��� ������ ����Ȯ���� �켱 �յ��ϰ�
            // ����Ȯ���� '�ӽ� �հ谪'�� ���
            // ������ ����Ȯ���� '�ӽ� �հ谪'���� ������, �հ谡 100%(=1.0) �� ������
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
    // �� ������ ���� ������
    // ���õ� ����
    public void initialize() { this.levelDatas = new List<LevelData>(); }
    // List�� �ʱ�ȭ
    public void loadLevelData(TextAsset levelDataText)
    { // �ؽ�Ʈ �����͸� �о�ͼ� �� ������ �ؼ��ϰ� �����͸� ����
        string levelTexts = levelDataText.text;
        // �ؽ�Ʈ �����͸� ���ڿ��μ� �޾Ƶ��δ�       
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

    public void SelectLevel()
    { // �� ���� ���� ���Ͽ��� ���� ����� ������ ����        
      // 0~���� ������ ���� ���Ƿ� ����
        this.selectLevel = Random.Range(0, this.levelDatas.Count);
        Debug.Log("select level = " + this.selectLevel.ToString());
    }

    public LevelData GetCurrentLevelData()
    { // ���õǾ� �ִ� ���� ������ ���� �����͸� ��ȯ
      // ���õ� ������ ���� �����͸� ��ȯ
        return (this.levelDatas[this.selectLevel]);
    }

    public float GetVanishTime()
    { // ���õǾ� �ִ� ���� ������ ���� �ð��� ��ȯ
      // ���õ� ������ ���ҽð��� ��ȯ
        return (this.levelDatas[this.selectLevel].heatTime);
    }
}