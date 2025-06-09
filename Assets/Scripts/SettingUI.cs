using UnityEngine;
using UnityEngine.Audio;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SettingUI : MonoBehaviour
{
    public bool onSetting = false;

    public Font customFont;
    public Texture2D windowBackground;
    public Texture2D buttonNormal, buttonHover, buttonActive;
    public Texture2D sliderBackground;
    public Texture2D sliderThumb;
    public AudioMixer audioMixer;

    private GUIStyle windowStyle;
    private GUIStyle buttonStyle;
    private GUIStyle resButtonStyle;
    private GUIStyle labelStyle;
    private GUIStyle sliderStyle;
    private GUIStyle sliderThumbStyle;

    private Rect windowRect;
    private static float bgmVolume = 0.45f;
    private static float sfxVolume = 0.5f;
    private bool isDraggingSlider = false;

    public static SettingUI Instance { get; private set; }
    private static readonly object _lock = new object();

    protected virtual void Awake()
    {
        SetInstance();
    }

    void Start()
    {
        SetBgmVolume();
        SetSFXVolume();
    }

    protected void SetInstance()
    {
        lock (_lock)
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private readonly Vector2Int[] resolutions =
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160)
    };
    private readonly float baseHeight = 1080f;
    private float uiScale = 1f;

    private void SetBgmVolume()
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(bgmVolume) * 20);
    }

    private void SetSFXVolume()
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(sfxVolume) * 20);
    }

    private void OnGUI()
    {
        if (!onSetting)
        {
            if (SceneManager.GetActiveScene().name != "TutorialScene") MadeByNamed();
            return;
        }
        GUI.depth = 0;
        uiScale = Screen.height / baseHeight;
        InitStyles();

        // 배경 검정
        var background = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
        background.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
        background.Apply();
        GUI.Box(new Rect(0f, 0.0f, Screen.width, Screen.height), "");

        float windowWidth = Screen.width * 0.6f;
        float windowHeight = Screen.height * 0.7f;
        windowRect = new Rect(
            (Screen.width - windowWidth) / 2f,
            (Screen.height - windowHeight) / 2f,
            windowWidth,
            windowHeight
        );

        // 창 밖 클릭 시 닫기
        if (Event.current.type == EventType.MouseDown && !windowRect.Contains(Event.current.mousePosition))
        {
            onSetting = false;
            return;
        }

        // 배경 창
        GUI.Box(windowRect, "설정", windowStyle);

        // 내부 UI 그리기
        Rect contentRect = new Rect(
            windowRect.x + windowStyle.padding.left,
            windowRect.y + windowStyle.padding.top,
            windowRect.width - windowStyle.padding.horizontal,
            windowRect.height - windowStyle.padding.vertical
        );

        GUILayout.BeginArea(contentRect);
        DrawSettingWindow();
        GUILayout.EndArea();
    }

    private void InitStyles()
    {
        Color btnTextColor = new Color32(181, 154, 102, 255);

        int fontSizeLarge = Mathf.RoundToInt(36 * uiScale);  // 1080 기준 약 36
        int fontSizeMedium = Mathf.RoundToInt(28 * uiScale);
        int fontSizeSmall = Mathf.RoundToInt(25 * uiScale);

        int paddingH = Mathf.RoundToInt(40 * uiScale);
        int paddingV = Mathf.RoundToInt(50 * uiScale);
        int buttonHeight = Mathf.RoundToInt(60 * uiScale);
        int sliderHeight = Mathf.RoundToInt(20 * uiScale);

        windowStyle = new GUIStyle
        {
            normal = { background = windowBackground, textColor = Color.white },
            font = customFont,
            fontSize = fontSizeLarge,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter,
            padding = new RectOffset(paddingH, paddingH, paddingV, paddingV)
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            normal = { background = buttonNormal, textColor = btnTextColor },
            hover = { background = buttonHover, textColor = btnTextColor },
            active = { background = buttonActive, textColor = btnTextColor },
            font = customFont,
            fontSize = fontSizeSmall,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(paddingH / 2, paddingH / 2, paddingV / 3, paddingV / 3)
        };
        resButtonStyle = new GUIStyle(buttonStyle)
        {
            fontSize = fontSizeSmall,
            padding = new RectOffset(0, 0, 0, 0)
        };

        labelStyle = new GUIStyle()
        {
            font = customFont,
            fontSize = fontSizeMedium,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            margin = new RectOffset(0, 0, Mathf.RoundToInt(10 * uiScale), Mathf.RoundToInt(10 * uiScale))
        };

        sliderStyle = new GUIStyle(GUI.skin.horizontalSlider)
        {
            normal = { background = sliderBackground },
            fixedHeight = sliderHeight
        };

        sliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb)
        {
            normal = { background = sliderThumb },
            fixedWidth = sliderHeight,
            fixedHeight = sliderHeight
        };
    }

    private void DrawSettingWindow()
    {
        GUILayout.Space(Mathf.RoundToInt(50 * uiScale));
        GUILayout.Label("배경음악 볼륨", labelStyle);

        float newBgm = GUILayout.HorizontalSlider(bgmVolume, 0.0001f, 1f, sliderStyle, sliderThumbStyle);
        if (newBgm != bgmVolume)
        {
            if (!isDraggingSlider)
            {
                isDraggingSlider = true;
            }
            bgmVolume = newBgm;
            SetBgmVolume();
        }

        if (isDraggingSlider && Event.current.type == EventType.MouseUp)
        {
            isDraggingSlider = false;
        }

        GUILayout.Space(Mathf.RoundToInt(10 * uiScale));
        GUILayout.Label("효과음 볼륨", labelStyle);

        float newSfx = GUILayout.HorizontalSlider(sfxVolume, 0.0001f, 1f, sliderStyle, sliderThumbStyle);
        if (newSfx != sfxVolume)
        {
            if (!isDraggingSlider)
            {
                isDraggingSlider = true;
            }
            sfxVolume = newSfx;
            PlaySFXSound();
            SetSFXVolume();
        }

        if (isDraggingSlider && Event.current.type == EventType.MouseUp)
        {
            isDraggingSlider = false;
        }

        GUILayout.Space(Mathf.RoundToInt(10 * uiScale));
        GUILayout.Label("해상도 설정", labelStyle);
        GUILayout.Space(Mathf.RoundToInt(10 * uiScale));

        int buttonHeight = Mathf.RoundToInt(45 * uiScale);
        foreach (Vector2Int res in resolutions)
        {
            bool nowRes = Screen.width == res.x && Screen.height == res.y;
            if (nowRes)
            {
                GUIStyle _style = new GUIStyle(resButtonStyle);
                _style.normal.background = buttonActive;
                int offset = Mathf.RoundToInt(4 * uiScale);
                _style.padding = new RectOffset(
                    resButtonStyle.padding.left,
                    resButtonStyle.padding.right,
                    resButtonStyle.padding.top + offset,
                    resButtonStyle.padding.bottom - offset
                );
                GUILayout.Box($"{res.x} x {res.y}", _style, GUILayout.Height(buttonHeight));
            }
            else if (GUILayout.Button($"{res.x} x {res.y}", resButtonStyle, GUILayout.Height(buttonHeight)))
            {
                Screen.SetResolution(res.x, res.y, FullScreenMode.Windowed);
            }
            GUILayout.Space(Mathf.RoundToInt(10 * uiScale));
        }


        GUILayout.FlexibleSpace();
        if (GUILayout.Button("완료", buttonStyle, GUILayout.Height(Mathf.RoundToInt(60 * uiScale))))
        {
            onSetting = false;
            if (isDraggingSlider)
            {
                isDraggingSlider = false;
            }
        }
    }

    private void PlaySFXSound()
    {
        this.GetComponent<AudioSource>().Play();
    }

    private void MadeByNamed()
    {
        GUIStyle nameStyle = new GUIStyle()
        {
            normal = { textColor = new Color32(255, 255, 255, 150) },
            font = customFont,
            fontSize = Mathf.RoundToInt(25 * uiScale),
        };
        float width = 290f * uiScale;
        float height = 30f * uiScale;
        Rect rect = new Rect(Screen.width - width, Screen.height - height, width, height);
        GUI.Label(rect, "Made by C077032 조형구", nameStyle);
    }
}
