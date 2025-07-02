using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BG_AutoScaler : MonoBehaviour
{
    [Tooltip("�n��۶񺡵e�����۾�")]
    public Camera cam;
    [Tooltip("Quad ���۾����Z���X�X�V�p�V�K�����Y")]
    public float bgZOffset = 30f;

    void LateUpdate()
    {
        if (cam == null) return;
        // 1) ��m+���� ��۾��@�P
        transform.position = cam.transform.position + cam.transform.forward * bgZOffset;
        transform.rotation = cam.transform.rotation;

        // 2) �p��e�����e
        float halfFOV = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float height = 2f * bgZOffset * Mathf.Tan(halfFOV);
        float width = height * cam.aspect;

        // 3) ������ Quad �Y��� (width, height)
        transform.localScale = new Vector3(width, height, 1f);
    }
}
