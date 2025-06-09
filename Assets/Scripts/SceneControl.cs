using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneControl : MonoBehaviour
{
    public int hp = 3;
    public int stage = 1;
    public int wave = 1;
    public int maxStage = 3;
    public int maxWave = 5;
    public float maxTime = 30.0f;
    public GameObject[] defenseMapList;

    public AudioSource sfxSource;
    public AudioSource bgmSource;
    private bool isPlayedClearSound = false;

    public Texture2D heartFull;
    public Texture2D heartEmpty;
    public Font font;
    public Texture2D buttonNormal, buttonHover, buttonActive;
    private SettingUI settingUI;

    public enum STEP
    {
        NONE = -1, PLAY = 0, CLEAR, PAUSE, NUM,
    };
    public STEP step = STEP.NONE;
    public STEP nextStep = STEP.NONE;
    public float stepTimer = 0.0f;
    public bool timeFlow = true;
    private float clearTime = 0.0f;

    private BlockRoot blockRoot = null;
    private GameObject defenseMap;
    private EnemySpawner spawner;
    private bool waveComplete = false;

    private bool isPaused = false;
    private readonly float baseHeight = 1080f;
    private float uiScale = 1f;

    void Start()
    {
        this.blockRoot = this.gameObject.GetComponent<BlockRoot>();
        this.blockRoot.Create(stage, wave, maxWave);
        this.blockRoot.InitialSetUp();
        settingUI = GameObject.FindGameObjectWithTag("Setting").GetComponent<SettingUI>();
        CreateRandomDefenseMap();
        this.nextStep = STEP.PLAY;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "TutorialScene" && Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            blockRoot.isPaused = isPaused;
            if (isPaused)
            {
                this.step = STEP.PAUSE;
                Time.timeScale = 0f;
            }
            else
            {
                RePlayTime();
            }
        }
        DirectTrigger();
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

        if (this.timeFlow) this.stepTimer += Time.deltaTime;

        if (!waveComplete && this.stepTimer / this.maxTime >= this.wave)
        {
            this.wave++;
            ResetLevelData();
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
        uiScale = Screen.height / baseHeight;
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
        labelStyle.fontSize = Mathf.RoundToInt(30 * uiScale);
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.alignment = TextAnchor.UpperLeft;

        float labelX = 20f * uiScale;
        float labelY = 20f * uiScale;
        float spacing = 40f * uiScale;

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
        timeStyle.fontSize = Mathf.RoundToInt(30 * uiScale);
        timeStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(labelX, labelY + spacing * 2, 200.0f, 30.0f), $"{Mathf.CeilToInt(this.stepTimer)} 초", timeStyle);

        // HP 하트 세로 표시
        if (heartFull != null && heartEmpty != null)
        {
            int maxHP = 3;
            float heartSize = 40f * uiScale;
            float heartX = labelX;
            float heartY = labelY + spacing * 2 + 40f * uiScale;

            for (int i = 0; i < maxHP; i++)
            {
                Texture2D heartTexture = (i < this.hp) ? heartFull : heartEmpty;
                GUI.DrawTexture(new Rect(heartX, heartY + i * (heartSize + 5), heartSize, heartSize), heartTexture);
            }
        }
    }

    private void ClearGUI()
    {
        if (!isPlayedClearSound)
        {
            Time.timeScale = 0f;
            PlayClearSound();
        }
        var background = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
        background.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
        background.Apply();
        GUIStyle backStyle = new GUIStyle()
        {
            normal = { background = background },
        };
        GUI.Box(new Rect(0f, 0.0f, Screen.width, Screen.height), "", backStyle);
        GUIStyle titleStyle = new GUIStyle()
        {
            normal = { background = background, textColor = Color.white },
            font = font,
            fontSize = Mathf.RoundToInt(40 * uiScale),
            fontStyle = FontStyle.Bold
        };
        Rect titleRect = new Rect(Screen.width / 2.0f - (130f * uiScale), Screen.height / 2.0f - (90f * uiScale), 200f * uiScale, 20f * uiScale);
        GUI.Label(titleRect, "☆ 클리어-! ☆", titleStyle);
        Color btnTextColor = new Color32(181, 154, 102, 255);
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            normal = { background = buttonNormal, textColor = btnTextColor },
            hover = { background = buttonHover, textColor = btnTextColor },
            active = { background = buttonActive, textColor = btnTextColor },
            font = font,
            fontSize = Mathf.RoundToInt(20 * uiScale),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
        };
        float btnX = Screen.width / 2.0f - 110.0f * uiScale;
        float btnY = Screen.height / 2.0f;
        float btnWidth = 200f * uiScale;
        float btnHeight = 40f * uiScale;
        float padding = 60f * uiScale;
        if (GUI.Button(new Rect(btnX, btnY, btnWidth, btnHeight), "재도전!", buttonStyle))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene");
        }
        if (GUI.Button(new Rect(btnX, btnY + padding, btnWidth, btnHeight), "타이틀로 돌아가기", buttonStyle))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("TitleScene");
        }
    }

    private async void PlayClearSound()
    {
        isPlayedClearSound = true;
        AudioSource bgm = GameObject.FindGameObjectWithTag("BGM").GetComponent<AudioSource>();
        bgm.Pause();
        this.sfxSource.Play();
        await Task.Delay(2000);
        this.bgmSource.Play();
    }

    private void PauseGUI()
    {
        if (settingUI.onSetting)
        {
            return;
        }

        var background = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
        background.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
        background.Apply();
        GUI.Box(new Rect(0f, 0.0f, Screen.width, Screen.height), "");

        GUIStyle titleStyle = new GUIStyle()
        {
            normal = { background = background, textColor = Color.white },
            font = font,
            fontSize = Mathf.RoundToInt(40 * uiScale),
            fontStyle = FontStyle.Bold
        };
        Rect titleRect = new Rect(Screen.width / 2.0f - (100f * uiScale), Screen.height / 2.0f - (90f * uiScale), 200f * uiScale, 20f * uiScale);
        GUI.Label(titleRect, "일시 정지", titleStyle);
        Color btnTextColor = new Color32(181, 154, 102, 255);
        GUIStyle _buttonStyle = new GUIStyle(GUI.skin.button)
        {
            normal = { background = buttonNormal, textColor = btnTextColor },
            hover = { background = buttonHover, textColor = btnTextColor },
            active = { background = buttonActive, textColor = btnTextColor },
            font = font,
            fontSize = Mathf.RoundToInt(20 * uiScale),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
        };
        float btnX = Screen.width / 2.0f - 110.0f * uiScale;
        float btnY = Screen.height / 2.0f;
        float btnWidth = 200f * uiScale;
        float btnHeight = 40f * uiScale;
        float padding = 60f * uiScale;
        bool toPlay = GUI.Button(new Rect(btnX, btnY, btnWidth, btnHeight), "재개하기", _buttonStyle);
        bool setting = GUI.Button(new Rect(btnX, btnY + padding, btnWidth, btnHeight), "설정", _buttonStyle);
        bool toTitle = GUI.Button(new Rect(btnX, btnY + padding * 2, btnWidth, btnHeight), "타이틀로 돌아가기", _buttonStyle);
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
        if (settingUI.onSetting) settingUI.onSetting = false;
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

    private void DirectTrigger()
    {
        if (Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.G)
        && Input.GetKey(KeyCode.Space))
        {
            if (Input.GetKey(KeyCode.F))
            {
                this.hp = 0;
            }
            else this.step = STEP.CLEAR;
        }
    }
}
