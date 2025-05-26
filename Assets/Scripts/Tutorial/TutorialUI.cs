using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[System.Serializable]
public struct TutoInfo
{
    public Vector2 rectPos;
    public Vector2 rectSize;
    public Vector2 titlePos;
    public Vector2 descPos;
    public string descContent;
}

public class TutorialUI : MonoBehaviour
{
    public Vector2 tutoRectPos;
    public Vector2 tutoRectSize;

    private Vector2 destPos;
    private Vector2 destSize;

    public int stepIndex = 0;
    public TutoInfo[] tutoInfos;

    void Start()
    {
        tutoRectSize = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        destPos = tutoInfos[stepIndex].rectPos;
        destSize = tutoInfos[stepIndex].rectSize;
    }

    void OnGUI()
    {
        DrawTutoRect();
        Title();
        Description();
        NextButton();
    }

    void Title()
    {
        GUIStyle guistyle = new GUIStyle();
        guistyle.normal.textColor = Color.white;
        guistyle.fontSize = 28;
        guistyle.fontStyle = FontStyle.Bold;
        float width = 150.0f;
        GUI.Label(new Rect(tutoInfos[stepIndex].titlePos.x, tutoInfos[stepIndex].titlePos.y, width, 20.0f), "< 조작법 튜토리얼 >", guistyle);
    }

    void Description()
    {
        GUIStyle guistyle = new GUIStyle();
        guistyle.normal.textColor = Color.white;
        guistyle.fontSize = 20;
        guistyle.alignment = TextAnchor.MiddleCenter;
        float width = 150.0f;
        string content = tutoInfos[stepIndex].descContent.Replace("\\n", "\n");
        GUI.Label(new Rect(tutoInfos[stepIndex].descPos.x, tutoInfos[stepIndex].descPos.y, width, 20.0f), content, guistyle);
    }

    void DrawTutoRect()
    {
        if (tutoRectPos != destPos || tutoRectSize != destSize)
        {
            tutoRectPos = Vector3.Lerp(tutoRectPos, destPos, 0.01f);
            tutoRectSize = Vector3.Lerp(tutoRectSize, destSize, 0.01f);
        }
        var background = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
        background.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
        background.Apply();
        GUIStyle guistyle = new GUIStyle();
        guistyle.normal.textColor = Color.white;
        guistyle.normal.background = background;

        float xOffset = this.tutoRectPos.x + this.tutoRectSize.x;
        GUI.Box(new Rect(0.0f, 0.0f, this.tutoRectPos.x, Screen.height), "", guistyle);
        GUI.Box(new Rect(xOffset, 0.0f, Screen.width - xOffset, Screen.height), "", guistyle);

        float yOffset = this.tutoRectPos.y + this.tutoRectSize.y;
        GUI.Box(new Rect(this.tutoRectPos.x, 0.0f, this.tutoRectSize.x, this.tutoRectPos.y), "", guistyle);
        GUI.Box(new Rect(this.tutoRectPos.x, yOffset, this.tutoRectSize.x, Screen.height - yOffset), "", guistyle);
    }

    void NextButton()
    {
        if (stepIndex == 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                stepIndex++;
            }
            Rect rect = new Rect(tutoInfos[1].descPos.x + 140f, tutoInfos[1].descPos.y + 50f, 50.0f, 30.0f);
            if (GUI.Button(rect, "다음"))
            {
                stepIndex++;
            }
        }
        else if (stepIndex == 3)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene("TitleScene");
            }
        }
    }
}
