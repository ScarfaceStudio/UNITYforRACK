using UnityEngine;

public class PartGroupController : MonoBehaviour
{
    [Header("互動設定")]
    public Transform rotationRoot;       // 拖入同一物件自己或子物件，用於旋轉/縮放
    public float rotationSpeed = 0.5f;
    public float minVertical = -80f, maxVertical = 80f;
    public float minScale = 1f, maxScale = 8f;
    public float scaleSpeed = 0.15f;

    [Header("飛走整組")]
    public Vector3 groupFlyDir = Vector3.back; // 被關閉時整組飛出的方向
    public float groupFlyDist = 5f;
    public float groupFlyTime = 1f;

    PartFlyElement[] parts;
    Vector3 groupOrigin;
    Quaternion rotOrigin;
    Vector3 scaleOrigin;

    // 旋轉用
    bool isDragging = false;
    Vector3 lastMouse;
    float xRot, yRot;
    bool interactionEnabled = false;

    void Awake()
    {
        // 找出所有子 PartFlyElement
        parts = GetComponentsInChildren<PartFlyElement>();

        // 記錄群組原始位置、旋轉、縮放
        groupOrigin = transform.localPosition;
        rotOrigin = rotationRoot.localRotation;
        scaleOrigin = rotationRoot.localScale;
        var e = rotOrigin.eulerAngles;
        xRot = e.x; yRot = e.y;
    }

    // 把整組飛回原位
    public void FlyBackGroup()
    {
        StopAllCoroutines();
        StartCoroutine(FlyGroupTo(groupOrigin));
    }

    // 把整組飛出
    public void FlyAwayGroup()
    {
        Vector3 target = groupOrigin + groupFlyDir.normalized * groupFlyDist;
        StopAllCoroutines();
        StartCoroutine(FlyGroupTo(target));
    }

    IEnumerator FlyGroupTo(Vector3 target)
    {
        Vector3 start = transform.localPosition;
        float t = 0f;
        while (t < groupFlyTime)
        {
            transform.localPosition = Vector3.Lerp(start, target, t / groupFlyTime);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = target;
    }

    // 飛回所有子項、旋轉及縮放歸位
    public void ResetAll()
    {
        // 所有部件飛回原點
        foreach (var p in parts) p.FlyBack();
        // 模型旋轉+縮放回原
        rotationRoot.localRotation = rotOrigin;
        rotationRoot.localScale = scaleOrigin;
    }

    public void EnableInteraction() => interactionEnabled = true;
    public void DisableInteraction() => interactionEnabled = false;

    void Update()
    {
        if (!interactionEnabled) return;

        // 拖曳旋轉
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMouse = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
            isDragging = false;

        if (isDragging)
        {
            var delta = (Vector3)Input.mousePosition - lastMouse;
            lastMouse = Input.mousePosition;

            yRot -= delta.x * rotationSpeed;
            xRot += delta.y * rotationSpeed; // 上下顛倒
            xRot = Mathf.Clamp(xRot, minVertical, maxVertical);

            rotationRoot.localRotation = Quaternion.Euler(xRot, yRot, 0f);
        }

        // 滾輪縮放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float cur = rotationRoot.localScale.x / scaleOrigin.x;
            float nxt = Mathf.Clamp(cur + scroll * scaleSpeed, minScale, maxScale);
            rotationRoot.localScale = scaleOrigin * nxt;
        }
    }
}
