using UnityEngine;

public class CameraZoomRotate : MonoBehaviour
{
    [Header("旋轉參數")]
    public float rotateSpeed = 5f;
    public float minPitch = -20f, maxPitch = 80f;

    [Header("縮放參數")]
    public float zoomSpeed = 2f;
    public float minDistance = 2f, maxDistance = 20f;

    public Transform target;

    private float yaw, pitch, distance;

    void Start()
    {
        if (target == null)
        {
            target = new GameObject("CameraPivot").transform;
            target.position = Vector3.zero;
        }
        distance = Vector3.Distance(transform.position, target.position);
        var e = transform.eulerAngles;
        yaw = e.y; pitch = e.x;
    }

    void Update()
    {
        // 旋轉 (滑鼠左鍵或單指觸控)
        if (Input.GetMouseButton(0) || Input.touchCount == 1)
        {
            float h = Input.GetMouseButton(0)
                ? Input.GetAxis("Mouse X")
                : Input.GetTouch(0).deltaPosition.x * 0.01f;
            float v = Input.GetMouseButton(0)
                ? Input.GetAxis("Mouse Y")
                : Input.GetTouch(0).deltaPosition.y * 0.01f;

            yaw += h * rotateSpeed;
            pitch = Mathf.Clamp(pitch - v * rotateSpeed, minPitch, maxPitch);

            float ry = Mathf.Deg2Rad * yaw;
            float rp = Mathf.Deg2Rad * pitch;
            Vector3 offset = new Vector3(
                distance * Mathf.Cos(rp) * Mathf.Sin(ry),
                distance * Mathf.Sin(rp),
                distance * Mathf.Cos(rp) * Mathf.Cos(ry)
            );
            transform.position = target.position + offset;
            transform.LookAt(target);

            Debug.Log($"[Camera] Yaw={yaw:F1}, Pitch={pitch:F1}");
        }

        // 縮放 (滾輪或雙指捏合)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
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
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);
            // 更新位置
            float ry = Mathf.Deg2Rad * yaw;
            float rp = Mathf.Deg2Rad * pitch;
            Vector3 offset = new Vector3(
                distance * Mathf.Cos(rp) * Mathf.Sin(ry),
                distance * Mathf.Sin(rp),
                distance * Mathf.Cos(rp) * Mathf.Cos(ry)
            );
            transform.position = target.position + offset;
            transform.LookAt(target);

            Debug.Log($"[Camera] Distance={distance:F1}");
        }
    }
}
