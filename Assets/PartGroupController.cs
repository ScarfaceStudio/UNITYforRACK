using UnityEngine;
using System.Collections;

public class PartGroupController : MonoBehaviour
{
    [Header("�s�զW�� (Debug��)")]
    public string groupName;

    [Header("���X�]�w")]
    public Vector3 flyDirection = Vector3.up;
    public float flySpeed = 1f;
    public float flyDistance = 1f;

    [Header("���� & �Y��]�w")]
    public float rotateSpeed = 100f;   // �����F�ӫ�
    public float zoomSpeed = 1f;     // �Y���F�ӫ�
    public float minScale = 0.5f;   // �̤p�Y��
    public float maxScale = 2f;     // �̤j�Y��

    private PartFlyElement[] parts;
    private Vector3 initialLocalPos;
    private bool isMoving = false;

    // �����
    private float yaw = 0f;
    private float pitch = 0f;
    // �Y���
    private float currentScale;

    void Awake()
    {
        Debug.Log($"[PartGroupController] {groupName} Awake");
        parts = GetComponentsInChildren<PartFlyElement>();
        Debug.Log($"[PartGroupController] {groupName} �@�`���� {parts.Length} �� PartFlyElement");
    }

    void Start()
    {
        // �O�������l��m
        foreach (var p in parts)
            p.StoreInitialPosition();

        // ���� & �Y����
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

    #region Fly ����]����ʡ^
    public void ActivateGroup()
    {
        Debug.Log($"[PartGroupController] {groupName} ActivateGroup");
        // �i�[�J�i���ʵe
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
        // ���m����P�Y��
        transform.rotation = Quaternion.Euler(0, yaw, 0);
        transform.localScale = Vector3.one * currentScale;
    }
    #endregion

    #region �����޿�
    private void HandleRotation()
    {
        // �u���b�Ӫ���ҥήɤ~�ͮ�
        if (!gameObject.activeSelf) return;

        bool rotating = false;
        float h = 0f, v = 0f;

        // �ƹ�����즲
        if (Input.GetMouseButton(0))
        {
            h = Input.GetAxis("Mouse X");
            v = Input.GetAxis("Mouse Y");
            rotating = true;
        }
        // ���Ĳ��
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
            // �]�i�ۦ�[ pitch ����^
            // pitch = Mathf.Clamp(pitch, -80f, 80f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
            Debug.Log($"[PartGroupController] {groupName} Rotate Yaw={yaw:F1}, Pitch={pitch:F1}");
        }
    }
    #endregion

    #region �Y���޿�
    private void HandleZoom()
    {
        if (!gameObject.activeSelf) return;

        // �ƹ��u��
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // �������X
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
