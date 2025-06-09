using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections;

[System.Serializable]
public struct TutoInfo
{
    public Vector2 rectPos;     // 기준 해상도(1920x1080) 기준 값
    public Vector2 rectSize;
    public Vector2 titlePos;
    public Vector2 descPos;
    public string descContent;
}

public class TutorialUI : MonoBehaviour
{
    public Font font;
    public EnemySpawner spawner;
    public ScoreCounter scoreCounter;
    public SceneControl sceneControl;
    public UnitStore unitStore;

    public Vector2 tutoRectPos;
    public Vector2 tutoRectSize;

    private Vector2 destPos;
    private Vector2 destSize;

    public int stepIndex = 0;
    public TutoInfo[] tutoInfos;

    private float uiScale;

    void Start()
    {
        sceneControl = GetComponent<SceneControl>();
        sceneControl.timeFlow = false;
        scoreCounter = GetComponent<ScoreCounter>();
        scoreCounter.isLocked = true;
        unitStore = GetComponent<UnitStore>();
        unitStore.isLocked = true;
    }

    void Update()
    {
        InitSpawner();
        TriggerStep();
    }

    void OnGUI()
    {
        GUI.depth = 0;
        uiScale = Screen.height / 1080f;
        destPos = tutoInfos[stepIndex].rectPos * uiScale;
        destSize = tutoInfos[stepIndex].rectSize * uiScale;
        DrawTutoRect();
        Title();
        Description();
    }

    void Title()
    {
        GUIStyle guistyle = new GUIStyle
        {
            normal = { textColor = Color.white },
            fontSize = Mathf.RoundToInt(40 * uiScale),
            fontStyle = FontStyle.Bold,
            font = font,
        };
        float width = 300.0f * uiScale;
        Vector2 pos = tutoInfos[stepIndex].titlePos * uiScale;
        GUI.Label(new Rect(pos.x, pos.y, width, 40.0f * uiScale), "< 조작법 튜토리얼 >", guistyle);
    }

    void Description()
    {
        GUIStyle guistyle = new GUIStyle
        {
            normal = { textColor = Color.white },
            fontSize = Mathf.RoundToInt(30 * uiScale),
            alignment = TextAnchor.MiddleCenter,
            font = font,
            richText = true
        };
        float width = 400.0f * uiScale;
        Vector2 pos = tutoInfos[stepIndex].descPos * uiScale;
        string content = tutoInfos[stepIndex].descContent.Replace("\\n", "\n");
        GUI.Label(new Rect(pos.x, pos.y, width, 80.0f * uiScale), content, guistyle);
    }

    void DrawTutoRect()
    {
        if (tutoRectPos != destPos || tutoRectSize != destSize)
        {
            tutoRectPos = Vector2.Lerp(tutoRectPos, destPos, 0.02f);
            tutoRectSize = Vector2.Lerp(tutoRectSize, destSize, 0.02f);
        }

        Texture2D background = new Texture2D(1, 1);
        background.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
        background.Apply();

        GUIStyle guistyle = new GUIStyle
        {
            normal = { background = background }
        };

        float xOffset = tutoRectPos.x + tutoRectSize.x;
        GUI.Box(new Rect(0f, 0f, tutoRectPos.x, Screen.height), "", guistyle);
        GUI.Box(new Rect(xOffset, 0f, Screen.width - xOffset, Screen.height), "", guistyle);

        float yOffset = tutoRectPos.y + tutoRectSize.y;
        GUI.Box(new Rect(tutoRectPos.x, 0f, tutoRectSize.x, tutoRectPos.y), "", guistyle);
        GUI.Box(new Rect(tutoRectPos.x, yOffset, tutoRectSize.x, Screen.height - yOffset), "", guistyle);
    }

    private void InitSpawner()
    {
        if (spawner == null)
        {
            spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<EnemySpawner>();
            spawner.activate = false;
            spawner.startCooltime = 1f;
        }
    }

    private async void TriggerStep()
    {
        GameObject Unit = GameObject.FindGameObjectWithTag("Unit");
        if (Unit != null && !Unit.GetComponent<Unit>().isActive) return;
        if (Input.GetMouseButtonDown(1))
        {
            SceneManager.LoadScene("TitleScene");
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (stepIndex >= tutoInfos.Length - 1) return;
            if (stepIndex == 0 || stepIndex == 2 ||
                stepIndex >= 3 && stepIndex <= 7)
            {
                if (stepIndex > 1 && scoreCounter.score[Block.COLOR.RED] < 10)
                    scoreCounter.score[Block.COLOR.RED] = 10;
                stepIndex++;
            }
            else if (stepIndex == 1 && scoreCounter.isLocked)
            {
                scoreCounter.isLocked = false;
                scoreCounter.score[Block.COLOR.RED] = 10;
                await Task.Delay(1000);
                stepIndex++;
            }
            else if (stepIndex == 8)
            {
                unitStore.isLocked = false;
                if (Unit != null && Unit.GetComponent<Unit>().isActive)
                {
                    spawner.activate = true;
                    stepIndex++;
                }
            }
            else if (stepIndex >= 8)
            {
                if (Unit != null && Unit.GetComponent<Unit>().isActive)
                    stepIndex++;
            }
        }
    }
}
