using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    public string scene;
    public float focus = 0.1f;
    public SettingUI settingUI;
    private bool isHovering = false;

    void Update()
    {
        RayCheck();
    }

    void RayCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (!isHovering)
                {
                    isHovering = true;
                    OnHover();
                }
            }
            else
            {
                if (isHovering)
                {
                    isHovering = false;
                    OnHoverExit();
                }
            }
        }
        else
        {
            if (isHovering)
            {
                isHovering = false;
                OnHoverExit();
            }
        }

        if (isHovering && Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }

    void OnClick()
    {
        if (scene == "quit") Application.Quit();
        else if (scene == "setting") settingUI.onSetting = true;
        else SceneManager.LoadScene(scene);
    }

    void OnHover()
    {
        Vector3 pos = this.transform.position;
        pos.z = pos.z - focus;
        this.transform.position = pos;
    }

    void OnHoverExit()
    {
        Vector3 pos = this.transform.position;
        pos.z = pos.z + focus;
        this.transform.position = pos;
    }
}
