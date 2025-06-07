using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public int hp = 3;
    public int stage = 1;
    public int wave = 1;
    public int maxStage = 3;
    public int maxWave = 5;
    public float maxTime = 30.0f;
    public GameObject[] defenseMapList;

    public SettingUI settingUI;
    public Texture2D heartFull;
    public Texture2D heartEmpty;

    public enum STEP
    {
        NONE = -1, PLAY = 0, CLEAR, PAUSE, NUM,
    };
    public STEP step = STEP.NONE;
    public STEP nextStep = STEP.NONE;
    public float stepTimer = 0.0f;
    private float clearTime = 0.0f;

    private BlockRoot blockRoot = null;
    private GameObject defenseMap;
    private EnemySpawner spawner;
    private bool waveComplete = false;

    private bool isPaused = false;

    void Start()
    {
        this.blockRoot = this.gameObject.GetComponent<BlockRoot>();
        this.blockRoot.Create(stage, wave, maxWave);
        this.blockRoot.InitialSetUp();
        CreateRandomDefenseMap();
        this.nextStep = STEP.PLAY;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            blockRoot.isPaused = isPaused;
            if (isPaused)
            {
                this.step = STEP.PAUSE;
                Time.timeScale = 0f;
                AudioListener.pause = true;
            }
            else
            {
                this.step = STEP.PLAY;
                Time.timeScale = 1f;
                AudioListener.pause = false;
            }
        }
        if (this.hp <= 0)
        {
            SceneManager.LoadScene("FailScene");
        }

        if (wave > maxWave)
        {
            wave = maxWave;
        }

        if (waveComplete && spawner.activate)
        {
            spawner.activate = false;
        }
        else if (waveComplete && GetEnemyCount() == 0)
        {
            NextStage();
        }

        if (this.stepTimer >= this.maxTime * this.maxWave)
        {
            waveComplete = true;
        }

        this.stepTimer += Time.deltaTime;

        if (!waveComplete && this.stepTimer / this.maxTime >= this.wave)
        {
            this.wave++;
            ResetLevelData();
        }

        switch (this.step)
        {
            case STEP.CLEAR:
                if (Input.GetMouseButtonDown(0))
                {
                    SceneManager.LoadScene("TitleScene");
                }
                break;
        }

        while (this.nextStep != STEP.NONE)
        {
            this.step = this.nextStep;
            this.nextStep = STEP.NONE;
            switch (this.step)
            {
                case STEP.CLEAR:
                    this.blockRoot.enabled = false;
                    this.clearTime = this.stepTimer;
                    break;
            }
            this.stepTimer = 0.0f;
        }
    }

    private void NextStage()
    {
        this.waveComplete = false;
        this.stage++;
        if (this.stage > this.maxStage)
        {
            this.step = STEP.CLEAR;
        }
        else
        {
            this.wave = 1;
            this.stepTimer = 0.0f;
            CreateRandomDefenseMap();
        }
    }

    private int GetEnemyCount()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length;
    }

    void OnGUI()
    {
        GUIStyle guistyle = new GUIStyle();
        switch (this.step)
        {
            case STEP.PLAY:
                PlayingGUI();
                break;
            case STEP.CLEAR:
                ClearGUI();
                break;
            case STEP.PAUSE:
                PauseGUI();
                break;
        }
    }

    private void PlayingGUI()
    {
        // 글씨 스타일 설정
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 30;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.alignment = TextAnchor.UpperLeft;

        float labelX = 20f;
        float labelY = 20f;
        float spacing = 40f;

        // 외곽선: 2px 오프셋만 사용 (상하좌우 총 4방향)
        labelStyle.normal.textColor = Color.black;
        GUI.Label(new Rect(labelX - 2, labelY, 300, 40), $"Stage {this.stage}", labelStyle);
        GUI.Label(new Rect(labelX + 2, labelY, 300, 40), $"Stage {this.stage}", labelStyle);
        GUI.Label(new Rect(labelX, labelY - 2, 300, 40), $"Stage {this.stage}", labelStyle);
        GUI.Label(new Rect(labelX, labelY + 2, 300, 40), $"Stage {this.stage}", labelStyle);

        GUI.Label(new Rect(labelX - 2, labelY + spacing, 300, 40), $"Wave {this.wave}", labelStyle);
        GUI.Label(new Rect(labelX + 2, labelY + spacing, 300, 40), $"Wave {this.wave}", labelStyle);
        GUI.Label(new Rect(labelX, labelY + spacing - 2, 300, 40), $"Wave {this.wave}", labelStyle);
        GUI.Label(new Rect(labelX, labelY + spacing + 2, 300, 40), $"Wave {this.wave}", labelStyle);

        // 본문 흰 글씨
        labelStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(labelX, labelY, 300, 40), $"Stage {this.stage}", labelStyle);
        GUI.Label(new Rect(labelX, labelY + spacing, 300, 40), $"Wave {this.wave}", labelStyle);

        // 타이머
        GUIStyle timeStyle = new GUIStyle();
        timeStyle.fontSize = 30;
        timeStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(labelX, labelY + spacing * 2, 200.0f, 30.0f), $"{Mathf.CeilToInt(this.stepTimer)} 초", timeStyle);

        // HP 하트 세로 표시
        if (heartFull != null && heartEmpty != null)
        {
            int maxHP = 3;
            float heartSize = 40f;
            float heartX = labelX;
            float heartY = labelY + spacing * 2 + 40f;

            for (int i = 0; i < maxHP; i++)
            {
                Texture2D heartTexture = (i < this.hp) ? heartFull : heartEmpty;
                GUI.DrawTexture(new Rect(heartX, heartY + i * (heartSize + 5), heartSize, heartSize), heartTexture);
            }
        }
    }

    private void ClearGUI()
    {
        GUIStyle guistyle = new GUIStyle();
        var background = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
        background.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
        background.Apply();
        guistyle.normal.textColor = Color.white;
        guistyle.normal.background = background;
        guistyle.fontStyle = FontStyle.Bold;
        GUI.Box(new Rect(0f, 0.0f, Screen.width, Screen.height), "", guistyle);
        GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, Screen.height / 2.0f - 70f, 200.0f, 20.0f), "☆클리어-!☆", guistyle);
        GUI.Button(new Rect(Screen.width / 2.0f - 110.0f, Screen.height / 2.0f, 200.0f, 40f), "Go back Title");
    }

    private void PauseGUI()
    {
        var background = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
        background.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
        background.Apply();
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.normal.textColor = Color.white;
        titleStyle.normal.background = background;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 40;
        GUI.Box(new Rect(0f, 0.0f, Screen.width, Screen.height), "", titleStyle);
        GUI.Label(new Rect(Screen.width / 2.0f - 100f, Screen.height / 2.0f - 90f, 200.0f, 20.0f), "일시 정지", titleStyle);
        GUIStyle buttnStyle = new GUIStyle();
        float btnX = Screen.width / 2.0f - 110.0f;
        float btnY = Screen.height / 2.0f;
        bool toPlay = GUI.Button(new Rect(btnX, btnY, 200.0f, 40f), "재개하기");
        bool setting = GUI.Button(new Rect(btnX, btnY + 60f, 200.0f, 40f), "설정");
        bool toTitle = GUI.Button(new Rect(btnX, btnY + 120f, 200.0f, 40f), "타이틀로 돌아가기");
        if (toPlay)
        {
            RePlayTime();
        }
        else if (setting)
        {
            settingUI.onSetting = true;
        }
        else if (toTitle)
        {
            RePlayTime();
            SceneManager.LoadScene("TitleScene");
        }
    }

    private void RePlayTime()
    {
        isPaused = false;
        blockRoot.isPaused = false;
        this.step = STEP.PLAY;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void ResetLevelData()
    {
        this.blockRoot.SetLevelData(stage, wave, maxWave);
        this.spawner.SetSpawnData(stage, wave);
    }

    private void CreateRandomDefenseMap()
    {
        if (this.defenseMap != null)
        {
            Destroy(this.defenseMap);
        }
        int index = this.stage - 1;
        if (index >= defenseMapList.Length)
        {
            index = Random.Range(0, defenseMapList.Length);
        }
        this.defenseMap = Instantiate(defenseMapList[index]);
        this.spawner = this.defenseMap.GetComponentInChildren<EnemySpawner>();
        this.spawner.ClearSpawnedList();
    }
}
