using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatStrength = 0.5f;   // 둥실둥실 진폭
    public float floatSpeed = 1f;        // 둥실둥실 속도

    [Header("Rise Settings")]
    public float startTime = 1f;         // 몇 초 뒤에 올라올지
    public float riseDuration = 1f;      // 올라오는 데 걸리는 시간

    private Vector3 originalPosition;
    private Vector3 loweredPosition;

    private float timer;
    private bool isRising = true;

    void Awake()
    {
        originalPosition = transform.position;
        loweredPosition = originalPosition;
        loweredPosition.y -= 100f;

        transform.position = loweredPosition;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (isRising)
        {
            if (timer >= startTime)
            {
                float t = (timer - startTime) / riseDuration;

                if (t >= 1f)
                {
                    transform.position = originalPosition;
                    isRising = false;
                    timer = 0f;
                    return;
                }

                // 커브 느낌을 위한 EaseOut Cubic 적용
                float easedT = 1f - (1f - t) * (1f - t) * (1f - t);

                float newY = Mathf.LerpUnclamped(loweredPosition.y, originalPosition.y, easedT);
                transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);
            }
        }
        else
        {
            float offsetY = Mathf.Sin(Time.time * floatSpeed) * floatStrength;
            transform.position = new Vector3(originalPosition.x, originalPosition.y + offsetY, originalPosition.z);
        }
    }
}