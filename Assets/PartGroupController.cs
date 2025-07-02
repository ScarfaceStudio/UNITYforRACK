using UnityEngine;
using System.Collections;

public class PartGroupController : MonoBehaviour
{
    [Header("群組名稱 (Debug用)")]
    public string groupName;

    [Header("飛出設定")]
    public Vector3 flyDirection = Vector3.up;
    public float flySpeed = 1f;
    public float flyDistance = 1f;

    [Header("旋轉 & 縮放設定")]
    public float rotateSpeed = 100f;   // 旋轉靈敏度
    public float zoomSpeed = 1f;     // 縮放靈敏度
    public float minScale = 0.5f;   // 最小縮放
    public float maxScale = 2f;     // 最大縮放

    private PartFlyElement[] parts;
    private Vector3 initialLocalPos;
    private bool isMoving = false;

    // 旋轉用
    private float yaw = 0f;
    private float pitch = 0f;
    // 縮放用
    private float currentScale;

    void Awake()
    {
        Debug.Log($"[PartGroupController] {groupName} Awake");
        parts = GetComponentsInChildren<PartFlyElement>();
        Debug.Log($"[PartGroupController] {groupName} 共蒐集到 {parts.Length} 個 PartFlyElement");
    }

    void Start()
    {
        // 記錄部件初始位置
        foreach (var p in parts)
            p.StoreInitialPosition();

        // 旋轉 & 縮放初值
        var e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;
        currentScale = transform.localScale.x;
    }

    void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    #region Fly 控制（不改動）
    public void ActivateGroup()
    {
        Debug.Log($"[PartGroupController] {groupName} ActivateGroup");
        // 可加入進場動畫
    }

    public void FlyOutGroup()
    {
        Debug.Log($"[PartGroupController] {groupName} FlyOutGroup");
        foreach (var p in parts) p.FlyOut();
    }

    public void ResetGroup()
    {
        Debug.Log($"[PartGroupController] {groupName} ResetGroup");
        foreach (var p in parts) p.ResetPosition();
        // 重置旋轉與縮放
        transform.rotation = Quaternion.Euler(0, yaw, 0);
        transform.localScale = Vector3.one * currentScale;
    }
    #endregion

    #region 旋轉邏輯
    private void HandleRotation()
    {
        // 只有在該物件啟用時才生效
        if (!gameObject.activeSelf) return;

        bool rotating = false;
        float h = 0f, v = 0f;

        // 滑鼠左鍵拖曳
        if (Input.GetMouseButton(0))
        {
            h = Input.GetAxis("Mouse X");
            v = Input.GetAxis("Mouse Y");
            rotating = true;
        }
        // 單指觸控
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
            // （可自行加 pitch 限制）
            // pitch = Mathf.Clamp(pitch, -80f, 80f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
            Debug.Log($"[PartGroupController] {groupName} Rotate Yaw={yaw:F1}, Pitch={pitch:F1}");
        }
    }
    #endregion

    #region 縮放邏輯
    private void HandleZoom()
    {
        if (!gameObject.activeSelf) return;

        // 滑鼠滾輪
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // 雙指捏合
        if (Input.touchCount == 2)
        {
            var t0 = Input.GetTouch(0);
            var t1 = Input.GetTouch(1);
            float prevDist = (t0.position - t0.deltaPosition - (t1.position - t1.deltaPosition)).magnitude;
            float currDist = (t0.position - t1.position).magnitude;
            scroll = (currDist - prevDist) * 0.01f;
        }

        if (Mathf.Abs(scroll) > 0.001f)
        {
            float targetScale = Mathf.Clamp(currentScale * (1 + scroll * zoomSpeed), minScale, maxScale);
            transform.localScale = Vector3.one * targetScale;
            currentScale = targetScale;
            Debug.Log($"[PartGroupController] {groupName} Zoom Scale={currentScale:F2}");
        }
    }
    #endregion
}
