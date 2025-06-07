using UnityEngine;

public class SettingUI : MonoBehaviour
{
    public bool onSetting = false;

    private GUIStyle windowStyle;
    private GUIStyle buttonStyle;
    private GUIStyle labelStyle;
    private GUIStyle sliderStyle;
    private GUIStyle popupStyle;

    private Rect windowRect;
    private float volume = 1.0f;
    private bool previousAudioListenerPaused = false;
    private bool isDraggingSlider = false;

    private int selectedResolutionIndex = 0;
    private bool isStyleInitialized = false;
    
    private readonly Vector2Int[] resolutions = new Vector2Int[]
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440)
    };

    private void OnGUI()
    {
        if (!isStyleInitialized)
        {
            InitStyles();
            isStyleInitialized = true;
        }
        if (onSetting)
        {
            float windowWidth = Screen.width * 0.6f;
            float windowHeight = Screen.height * 0.7f;
            windowRect = new Rect(
                (Screen.width - windowWidth) / 2f,
                (Screen.height - windowHeight) / 2f,
                windowWidth,
                windowHeight
            );

            // 창 외부 클릭 시 닫기
            if (Event.current.type == EventType.MouseDown && !windowRect.Contains(Event.current.mousePosition))
            {
                onSetting = false;
                if (isDraggingSlider)
                {
                    AudioListener.pause = previousAudioListenerPaused;
                    isDraggingSlider = false;
                }
                return;
            }

            GUI.Window(10001, windowRect, DrawSettingWindow, "⚙ 설정", windowStyle);
        }
    }

    private void DrawSettingWindow(int windowID)
    {
        GUILayout.Space(30);
        GUILayout.Label("🎵 배경음악 볼륨", labelStyle);

        float newVolume = GUILayout.HorizontalSlider(volume, 0f, 1f, sliderStyle, GUI.skin.horizontalSliderThumb);

        if (newVolume != volume)
        {
            if (!isDraggingSlider)
            {
                previousAudioListenerPaused = AudioListener.pause;
                isDraggingSlider = true;
            }

            AudioListener.pause = false;
            volume = newVolume;

            AudioSource bgm = GameObject.Find("BGM")?.GetComponent<AudioSource>();
            if (bgm != null)
                bgm.volume = volume;
        }

        if (isDraggingSlider && (Event.current.type == EventType.MouseUp || Event.current.rawType == EventType.MouseUp))
        {
            AudioListener.pause = previousAudioListenerPaused;
            isDraggingSlider = false;
        }

        GUILayout.Space(30);
        GUILayout.Label("🖥 해상도 설정", labelStyle);

        string[] resOptions = new string[resolutions.Length];
        for (int i = 0; i < resolutions.Length; i++)
        {
            resOptions[i] = $"{resolutions[i].x} x {resolutions[i].y}";
        }

        selectedResolutionIndex = GUILayout.SelectionGrid(selectedResolutionIndex, resOptions, 1, popupStyle);

        GUILayout.Space(20);
        if (GUILayout.Button("📐 적용하기", buttonStyle, GUILayout.Height(60)))
        {
            Vector2Int res = resolutions[selectedResolutionIndex];
            Screen.SetResolution(res.x, res.y, FullScreenMode.Windowed);
        }

        GUILayout.Space(20);
        if (GUILayout.Button("닫기", buttonStyle, GUILayout.Height(60)))
        {
            onSetting = false;
            if (isDraggingSlider)
            {
                AudioListener.pause = previousAudioListenerPaused;
                isDraggingSlider = false;
            }
        }
    }

    private void InitStyles()
    {
        Color beige = new Color(0.96f, 0.91f, 0.84f);
        Color darkBrown = new Color(0.36f, 0.24f, 0.2f);
        Color lightBeige = new Color(0.91f, 0.85f, 0.76f);

        Texture2D bgTex = new Texture2D(1, 1);
        bgTex.SetPixel(0, 0, beige);
        bgTex.Apply();

        Texture2D btnTex = new Texture2D(1, 1);
        btnTex.SetPixel(0, 0, lightBeige);
        btnTex.Apply();

        windowStyle = new GUIStyle(GUI.skin.window)
        {
            normal = { background = bgTex, textColor = darkBrown },
            fontSize = Mathf.RoundToInt(Screen.height * 0.03f),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter,
            padding = new RectOffset(40, 40, 30, 30)
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            normal = { background = btnTex, textColor = darkBrown },
            fontSize = Mathf.RoundToInt(Screen.height * 0.025f),
            fontStyle = FontStyle.Bold,
            padding = new RectOffset(20, 20, 10, 10),
            alignment = TextAnchor.MiddleCenter
        };

        labelStyle = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = darkBrown },
            fontSize = Mathf.RoundToInt(Screen.height * 0.025f),
            fontStyle = FontStyle.Bold,
            margin = new RectOffset(0, 0, 10, 10)
        };

        sliderStyle = new GUIStyle(GUI.skin.horizontalSlider)
        {
            fixedHeight = Screen.height * 0.02f
        };

        popupStyle = new GUIStyle(GUI.skin.button)
        {
            normal = { background = btnTex, textColor = darkBrown },
            fontSize = Mathf.RoundToInt(Screen.height * 0.022f),
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(20, 10, 10, 10),
            margin = new RectOffset(0, 0, 5, 5)
        };
    }
}
