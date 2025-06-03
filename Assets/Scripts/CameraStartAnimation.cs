using System.Collections;
using UnityEngine;

public class CameraStartLookUp : MonoBehaviour
{
    public float duration = 2f; // 올라오는 데 걸리는 시간

    void Start()
    {
        // 시작 시 밑으로 60도 보기 (x축 60도 회전)
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        StartCoroutine(RotateUp());
    }

    IEnumerator RotateUp()
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(0f, 0f, 0f);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, endRot, time / duration);
            yield return null;
        }

        transform.rotation = endRot; // 정확히 0도로 맞춤
    }
}