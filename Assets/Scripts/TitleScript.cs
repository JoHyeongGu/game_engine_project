using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{

    void OnGUI()
    {
        Title();
        GameStartButton();
        TutorialButton();
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
        Rect rect = new Rect((Screen.width / 2) - (width / 2), Screen.height / 2 - (height / 2), width, height);
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
        Rect rect = new Rect((Screen.width / 2) - (width / 2), Screen.height / 2 + height, width, height);
        if (GUI.Button(rect, "튜토리얼"))
        {
            SceneManager.LoadScene("TutorialScene");
        }
    }
}
