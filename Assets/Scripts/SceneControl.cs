using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private ScoreCounter scoreCounter = null;
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

    void Start()
    {
        this.blockRoot = this.gameObject.GetComponent<BlockRoot>();
        this.blockRoot.Create();
        this.blockRoot.InitialSetUp();
        this.scoreCounter = this.gameObject.GetComponent<ScoreCounter>(); // ScoreCounter 가져오기
        this.nextStep = STEP.PLAY;
        // 다음 상태를 '플레이 중'으로
        this.guistyle.fontSize = 24;
    }

    void Update()
    {
        this.stepTimer += Time.deltaTime;
        
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
                // 경과 시간을 표시
                GUI.Label(new Rect(40.0f, 10.0f, 200.0f, 20.0f), "시간" + Mathf.CeilToInt(this.stepTimer).ToString() + "초", guistyle);
                GUI.color = Color.white;
                break;
            case STEP.CLEAR:
                GUI.color = Color.black;
                // 「☆클리어-！☆」라는 문자열을 표시
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 20.0f, 200.0f, 20.0f), "☆클리어-!☆", guistyle);
                // 클리어 시간을 표시
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 40.0f, 200.0f, 20.0f), "클리어 시간" + Mathf.CeilToInt(this.clearTime).ToString() + "초", guistyle);
                GUI.color = Color.white;
                break;
        }
    }
}
