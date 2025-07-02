using UnityEngine;
using System.Collections;

public class PartGroupController : MonoBehaviour
{
    [Header("�s�զW�� (Debug ��)")]
    public string groupName;

    [Header("���X & �^�� (Fly)")]
    public Vector3 flyDirection = Vector3.up;
    public float flySpeed = 1f;
    public float flyDistance = 1f;

    [Header("���� & �Y��]�w")]
    public float rotateSpeed = 100f;   // �����F�ӫ�
    public float zoomSpeed = 1f;     // �Y���F�ӫ�
    public float minScale = 0.5f;   // �̤p�Y��
    public float maxScale = 2f;     // �̤j�Y��

    private PartFlyElement[] parts;
    private float yaw, pitch;
    private float currentScale;

    void Awake()
    {
        Debug.Log($"[PartGroupController] {groupName} Awake");
        parts = GetComponentsInChildren<PartFlyElement>();
        Debug.Log($"[PartGroupController] {groupName} �@�`���� {parts.Length} �� PartFlyElement");
    }

    void Start()
    {
        // ��l�Ʊ���P�Y��Ѽ�
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;
        currentScale = transform.localScale.x;
    }

    void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    #region Fly ����
    public void ActivateGroup()
    {
        Debug.Log($"[PartGroupController] {groupName} ActivateGroup");
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

        // ���m�ۨ�����P�Y��
        transform.rotation = Quaternion.Euler(0, yaw, 0);
        transform.localScale = Vector3.one * currentScale;
    }
    #endregion

    #region ����
    private void HandleRotation()
    {
        if (!gameObject.activeSelf) return;

        float h = 0f, v = 0f;
        bool rotating = false;

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
            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
            Debug.Log($"[PartGroupController] {groupName} Rotate Yaw={yaw:F1}, Pitch={pitch:F1}");
        }
    }
    #endregion

    #region �Y��
    private void HandleZoom()
    {
        if (!gameObject.activeSelf) return;

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
            currentScale = Mathf.Clamp(currentScale * (1 + scroll * zoomSpeed), minScale, maxScale);
            transform.localScale = Vector3.one * currentScale;
            Debug.Log($"[PartGroupController] {groupName} Zoom Scale={currentScale:F2}");
        }
    }
    #endregion
}
