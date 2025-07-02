using UnityEngine;

public class PartGroupController : MonoBehaviour
{
    [Header("旋轉 & 縮放")]
    public float rotateSpeed = 100f;   // 拖曳靈敏度
    public float zoomSpeed = 1f;     // 滾輪/捏合倍數
    public float minScale = 0.5f;   // 最小縮放
    public float maxScale = 2f;     // 最大縮放

    private float yaw, pitch;
    private float currentScale;

    void Start()
    {
        // 初始角度與縮放
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;
        currentScale = transform.localScale.x;
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        HandleRotation();
        HandleZoom();
    }

    // 滑鼠拖曳 & 單指觸控旋轉
    void HandleRotation()
    {
        bool rotating = false;
        float h = 0f, v = 0f;

        if (Input.GetMouseButton(0))
        {
            h = Input.GetAxis("Mouse X");
            v = Input.GetAxis("Mouse Y");
            rotating = true;
        }
        else if (Input.touchCount == 1)
        {
            var t = Input.GetTouch(0);
            h = t.deltaPosition.x * 0.1f;
            v = t.deltaPosition.y * 0.1f;
            rotating = true;
        }

        if (rotating)
        {
            yaw += h * rotateSpeed * Time.deltaTime;
            pitch -= v * rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            Debug.Log($"[PartGroupController] {name} Rotate Yaw={yaw:F1}, Pitch={pitch:F1}");
        }
    }

    // 滾輪捲動 & 雙指捏合縮放
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Input.touchCount == 2)
        {
            var t0 = Input.GetTouch(0);
            var t1 = Input.GetTouch(1);
            float prev = (t0.position - t0.deltaPosition - (t1.position - t1.deltaPosition)).magnitude;
            float curr = (t0.position - t1.position).magnitude;
            scroll = (curr - prev) * 0.01f;
        }

        if (Mathf.Abs(scroll) > 0.001f)
        {
            currentScale = Mathf.Clamp(currentScale * (1 + scroll * zoomSpeed), minScale, maxScale);
            transform.localScale = Vector3.one * currentScale;
            Debug.Log($"[PartGroupController] {name} Zoom Scale={currentScale:F2}");
        }
    }
}
