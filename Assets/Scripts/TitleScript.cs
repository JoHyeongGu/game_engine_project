using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{

    void OnGUI()
    {
        Title();
        GameStartButton();
        TutorialButton();
        QuitButton();
        Info();
        Dest();
    }

    void Update()
    {
        Debug.Log($"{Screen.width} {Screen.height}");
    }

    private void Title()
    {
        GUI.color = Color.black;
        GUIStyle guistyle = new GUIStyle();
        guistyle.fontSize = 24;
        float width = 250;
        float height = 50;
        Rect rect = new Rect((Screen.width / 2) - (width / 2), Screen.height / 2 - (height * 2), width, height);
        GUI.Label(rect, "Match! Match! Defense", guistyle);
    }

    private void GameStartButton()
    {
        GUI.color = Color.white;
        float width = 200;
        float height = 50;
        Rect rect = new Rect((Screen.width / 2) - (width / 2), Screen.height / 2 - (height / 2) - 10, width, height);
        if (GUI.Button(rect, "게임 시작"))
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    private void TutorialButton()
    {
        GUI.color = Color.white;
        float width = 200;
        float height = 50;
        Rect rect = new Rect((Screen.width / 2) - (width / 2), Screen.height / 2 + height - 27, width, height);
        if (GUI.Button(rect, "튜토리얼"))
        {
            SceneManager.LoadScene("TutorialScene");
        }
    }

    private void Info()
    {
        GUI.color = Color.black;
        GUIStyle guistyle = new GUIStyle();
        guistyle.fontSize = 15;
        float width = 250;
        float height = 30;
        Rect rect = new Rect(10, Screen.height - height, width, height);
        GUI.Label(rect, "제작자: C077032 조형구 / 해상도: (924 x 435)", guistyle);
    }

    private void Dest()
    {
        GUI.color = Color.grey;
        GUIStyle guistyle = new GUIStyle();
        guistyle.fontSize = 13;
        float width = 250;
        float height = 45;
        Rect rect = new Rect(10, Screen.height - height, width, height);
        GUI.Label(rect, "게임 목표: 블록을 Match해서 자원을 모으고 제한 시간 동안 몰려오는 적을 처치하기", guistyle);
    }

    private void QuitButton()
    {
        GUI.color = Color.white;
        float width = 200;
        float height = 50;
        Rect rect = new Rect((Screen.width / 2) - (width / 2), Screen.height / 2 + height + 30, width, height);
        if (GUI.Button(rect, "게임 종료"))
        {
            Application.Quit();
        }
    }
}
