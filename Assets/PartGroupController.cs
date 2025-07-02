using UnityEngine;

public class PartGroupController : MonoBehaviour
{
    [Header("���ʳ]�w")]
    public Transform rotationRoot;       // ��J�P�@����ۤv�Τl����A�Ω����/�Y��
    public float rotationSpeed = 0.5f;
    public float minVertical = -80f, maxVertical = 80f;
    public float minScale = 1f, maxScale = 8f;
    public float scaleSpeed = 0.15f;

    [Header("�������")]
    public Vector3 groupFlyDir = Vector3.back; // �Q�����ɾ�խ��X����V
    public float groupFlyDist = 5f;
    public float groupFlyTime = 1f;

    PartFlyElement[] parts;
    Vector3 groupOrigin;
    Quaternion rotOrigin;
    Vector3 scaleOrigin;

    // �����
    bool isDragging = false;
    Vector3 lastMouse;
    float xRot, yRot;
    bool interactionEnabled = false;

    void Awake()
    {
        // ��X�Ҧ��l PartFlyElement
        parts = GetComponentsInChildren<PartFlyElement>();

        // �O���s�խ�l��m�B����B�Y��
        groupOrigin = transform.localPosition;
        rotOrigin = rotationRoot.localRotation;
        scaleOrigin = rotationRoot.localScale;
        var e = rotOrigin.eulerAngles;
        xRot = e.x; yRot = e.y;
    }

    // ���խ��^���
    public void FlyBackGroup()
    {
        StopAllCoroutines();
        StartCoroutine(FlyGroupTo(groupOrigin));
    }

    // ���խ��X
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

    // ���^�Ҧ��l���B������Y���k��
    public void ResetAll()
    {
        // �Ҧ����󭸦^���I
        foreach (var p in parts) p.FlyBack();
        // �ҫ�����+�Y��^��
        rotationRoot.localRotation = rotOrigin;
        rotationRoot.localScale = scaleOrigin;
    }

    public void EnableInteraction() => interactionEnabled = true;
    public void DisableInteraction() => interactionEnabled = false;

    void Update()
    {
        if (!interactionEnabled) return;

        // �즲����
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
            xRot += delta.y * rotationSpeed; // �W�U�A��
            xRot = Mathf.Clamp(xRot, minVertical, maxVertical);

            rotationRoot.localRotation = Quaternion.Euler(xRot, yRot, 0f);
        }

        // �u���Y��
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float cur = rotationRoot.localScale.x / scaleOrigin.x;
            float nxt = Mathf.Clamp(cur + scroll * scaleSpeed, minScale, maxScale);
            rotationRoot.localScale = scaleOrigin * nxt;
        }
    }
}
