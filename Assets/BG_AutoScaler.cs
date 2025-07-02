using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BG_AutoScaler : MonoBehaviour
{
    [Tooltip("要跟著填滿畫面的相機")]
    public Camera cam;
    [Tooltip("Quad 離相機的距離——越小越貼近鏡頭")]
    public float bgZOffset = 30f;

    void LateUpdate()
    {
        if (cam == null) return;
        // 1) 位置+旋轉 跟相機一致
        transform.position = cam.transform.position + cam.transform.forward * bgZOffset;
        transform.rotation = cam.transform.rotation;

        // 2) 計算畫面高寬
        float halfFOV = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float height = 2f * bgZOffset * Mathf.Tan(halfFOV);
        float width = height * cam.aspect;

        // 3) 直接把 Quad 縮放到 (width, height)
        transform.localScale = new Vector3(width, height, 1f);
    }
}
