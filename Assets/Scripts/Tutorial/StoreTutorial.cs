using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StoreTutorial : MonoBehaviour
{
    public UnitItem[] units;
    public EnemySpawner spawner;
    public TutorialUI tutorialUI;
    public Dictionary<Block.COLOR, int> datas = new Dictionary<Block.COLOR, int>();

    private Vector2 posOffset = new Vector2(270.0f, 85.0f);
    private GUIStyle guistyle = new GUIStyle();
    private float time = 17.0f;
    private Vector3 mousePosition;
    private GameObject mainCamera;
    private Unit SelectedUnit;
    private int wave = 1;

    void Start()
    {
        this.mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        for (int i = 0; i <= (int)Block.COLOR.LAST; i++)
        {
            datas[(Block.COLOR)i] = 0;
        }
    }

    void Update()
    {
        this.UnprojectMousePosition(out mousePosition, Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && SelectedUnit != null && SelectedUnit.canPlaced)
        {
            SelectedUnit.Active();
            SelectedUnit = null;
            spawner.activate = true;
            tutorialUI.stepIndex++;
        }
        if (SelectedUnit != null)
        {
            SelectedUnit.SetMousePosition(this.mousePosition);
        }
        if (tutorialUI.stepIndex >= 3)
        {
            time += Time.deltaTime;
            if (time >= 20)
            {
                wave++;
                time = 0;
                if (wave >= 3)
                {
                    SceneManager.LoadScene("TitleScene");
                }
            }
        }
    }

    void OnGUI()
    {
        guistyle.fontSize = 16;
        float x = 0.0f;
        float y = posOffset.y;
        GUI.color = Color.black;
        foreach (KeyValuePair<Block.COLOR, int> data in datas)
        {
            this.printValue(x, y, $"{data.Key}", data.Value);
            y += 30;
        }

        guistyle.fontSize = 24;
        GUI.color = Color.black;
        GUI.Label(new Rect(10.0f, 10.0f, 200.0f, 20.0f), $"Stage 1", guistyle);
        GUI.Label(new Rect(10.0f, 40.0f, 200.0f, 20.0f), $"Wave {wave}", guistyle);
        GUI.Label(new Rect(10.0f, 70.0f, 200.0f, 20.0f), $"{(int)time} 초", guistyle);
        GUI.color = Color.white;

        float[] panelInfo = DrawStorePanel();
        DrawStoreContent(panelInfo);
    }

    public void printValue(float x, float y, string label, int value)
    {
        float labelWidth = 100.0f;
        float _x = (Screen.width - labelWidth) / 2f + x;
        GUI.Label(new Rect(_x, y, labelWidth, 20), label, guistyle);
        y += 15;
        GUI.Label(new Rect(_x, y, labelWidth, 20), value.ToString(), guistyle);
        y += 15;
    }


    private float[] DrawStorePanel()
    {
        float width = Screen.width / 2f;
        float height = 90f;
        float padding = 10;
        var rect = new Rect(padding, Screen.height - height - padding, width, height);
        GUI.Box(rect, "유닛 상점");
        return new float[] { width, height, padding };
    }

    private void DrawStoreContent(float[] panelInfo)
    {
        float padding = 10;
        float width = panelInfo[0] / 5;
        float height = panelInfo[1] - (padding * 2);

        float yPos = Screen.height - (height + padding) - panelInfo[2];

        for (int i = 0; i < 1; i++)
        {
            UnitItem unit = units[i];
            float xPos = panelInfo[2] + padding + (i * (width + padding));
            var rect = new Rect(xPos, yPos, width, height);
            if (GUI.Button(rect, new GUIContent(unit.image)))
            {
                this.datas[Block.COLOR.RED] -= 10;
                GameObject unitObject = Instantiate(unit.prefab, this.mousePosition, Quaternion.Euler(-90.0f, 0, 0));
                SelectedUnit = unitObject.GetComponent<Unit>();
            }
        }
    }

    public bool UnprojectMousePosition(out Vector3 worldPosition, Vector3 mousePosition)
    {
        bool ret;
        Plane plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));
        Ray ray = this.mainCamera.GetComponent<Camera>().ScreenPointToRay(mousePosition);
        float depth;
        if (plane.Raycast(ray, out depth))
        {
            worldPosition = ray.origin + ray.direction * depth;
            ret = true;
        }
        else
        {
            worldPosition = Vector3.zero;
            ret = false;
        }
        return (ret);
    }
}
