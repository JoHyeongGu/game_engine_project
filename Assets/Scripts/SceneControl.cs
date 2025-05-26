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

    public enum STEP
    {
        NONE = -1, PLAY = 0, CLEAR, NUM,
    };
    public STEP step = STEP.NONE;
    public STEP nextStep = STEP.NONE;
    public float stepTimer = 0.0f;
    private float clearTime = 0.0f;
    public GUIStyle guistyle;

    private BlockRoot blockRoot = null;
    private GameObject defenseMap;
    private EnemySpawner spawner;

    void Start()
    {
        this.blockRoot = this.gameObject.GetComponent<BlockRoot>();
        this.blockRoot.Create(stage, wave, maxWave);
        this.blockRoot.InitialSetUp();
        CreateRandomDefenseMap();
        this.nextStep = STEP.PLAY;
        this.guistyle.fontSize = 24;
    }

    void Update()
    {
        if (this.hp <= 0)
        {
            SceneManager.LoadScene("FailScene");
        }

        this.stepTimer += Time.deltaTime;

        if (this.stepTimer / this.maxTime >= this.wave)
        {
            this.wave++;
            if (this.wave > this.maxWave)
            {
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

    // 화면에 클리어한 시간과 메시지를 표시
    void OnGUI()
    {
        switch (this.step)
        {
            case STEP.PLAY:
                GUI.color = Color.black;
                GUI.Label(new Rect(10.0f, 10.0f, 200.0f, 20.0f), $"Stage {this.stage}", guistyle);
                GUI.Label(new Rect(10.0f, 40.0f, 200.0f, 20.0f), $"Wave {this.wave}", guistyle);
                GUI.Label(new Rect(10.0f, 70.0f, 200.0f, 20.0f), Mathf.CeilToInt(this.stepTimer).ToString() + " 초", guistyle);
                GUI.Label(new Rect(10.0f, 100.0f, 200.0f, 20.0f), $"HP: {this.hp}", guistyle);
                GUI.color = Color.white;
                break;
            case STEP.CLEAR:
                var background = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
                background.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
                background.Apply();
                guistyle.normal.textColor = Color.white;
                guistyle.normal.background = background;
                guistyle.fontStyle = FontStyle.Bold;
                GUI.Box(new Rect(0f, 0.0f, Screen.width, Screen.height), "", guistyle);
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, Screen.height / 2.0f - 70f, 200.0f, 20.0f), "☆클리어-!☆", guistyle);
                GUI.Button(new Rect(Screen.width / 2.0f - 110.0f, Screen.height / 2.0f, 200.0f, 40f), "Go back Title");
                break;
        }
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
