using UnityEngine;
using UnityEngine.SceneManagement;

public class FailUI : MonoBehaviour
{

    void OnGUI()
    {
        Title();
        Button();
    }

    private void Title()
    {
        GUI.color = Color.black;
        GUIStyle guistyle = new GUIStyle();
        guistyle.fontSize = 24;
        float width = 250;
        float height = 50;
        Rect rect = new Rect((Screen.width / 2) - (width / 2), Screen.height / 2 - (height), width, height);
        GUI.Label(rect, "Fail Game...", guistyle);
    }

    private void Button()
    {
        GUI.color = Color.white;
        float width = 200;
        float height = 50;
        Rect rect = new Rect((Screen.width / 2) - (width / 2), Screen.height / 2, width, height);
        if (GUI.Button(rect, "Back to Title"))
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}
