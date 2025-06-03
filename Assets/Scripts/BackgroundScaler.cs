using UnityEngine;

[ExecuteAlways]
public class FitPlaneToCamera : MonoBehaviour
{
    public Camera cam;
    public float planeZ = 10f;

    void Update()
    {
        if (cam == null) cam = Camera.main;
        float distance = planeZ - cam.transform.position.z;
        float height = 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float width = height * cam.aspect;
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, planeZ);
        transform.localScale = new Vector3(width / 10f, 1f, height / 10f);
    }
}
