using UnityEngine;

public class PartGroupController : MonoBehaviour
{
    [Header("�s�զW�� (Debug ��)")]
    public string groupName;
    private PartFlyElement[] parts;

    void Awake()
    {
        Debug.Log($"[PartGroupController] {groupName} Awake");
        parts = GetComponentsInChildren<PartFlyElement>();
        Debug.Log($"[PartGroupController] {groupName} �@�`���� {parts.Length} �� PartFlyElement");
    }

    public void ActivateGroup()
    {
        Debug.Log($"[PartGroupController] {groupName} ActivateGroup");
        // �Y�n��խ��J�i�b���[�J�ʵe
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
    }
}
