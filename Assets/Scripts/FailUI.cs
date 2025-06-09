using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FailUI : MonoBehaviour
{
    public Font font;
    public Texture2D buttonNormal, buttonHover, buttonActive;
    private float uiScale;

    private GameObject mainCamera;
    public float moveDistance = -3f;
    public float duration = 1f;

    void Start()
    {
        this.mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        StartCoroutine(MoveCamera());
    }

    void OnGUI()
    {
        uiScale = Screen.height / 1080f;
        Title();
        Button();
    }

    private void Title()
    {
        GUIStyle titleStyle = new GUIStyle()
        {
            normal = { textColor = Color.white },
            font = font,
            fontSize = Mathf.RoundToInt(60 * uiScale),
            fontStyle = FontStyle.Bold
        };
        Rect titleRect = new Rect(Screen.width / 2.0f - (150f * uiScale), Screen.height / 2.0f - (90f * uiScale), 200f * uiScale, 20f * uiScale);
        GUI.Label(titleRect, "FAIL GAME..", titleStyle);
    }

    private void Button()
    {
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

    private IEnumerator MoveCamera()
    {
        Vector3 startPos = mainCamera.transform.localPosition;
        Vector3 targetPos = startPos + new Vector3(0, 0, moveDistance);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            mainCamera.transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.localPosition = targetPos;
    }
}
