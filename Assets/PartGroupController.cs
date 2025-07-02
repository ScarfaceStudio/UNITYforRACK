using UnityEngine;

public class PartGroupController : MonoBehaviour
{
    [Header("群組名稱 (Debug 用)")]
    public string groupName;
    private PartFlyElement[] parts;

    void Awake()
    {
        Debug.Log($"[PartGroupController] {groupName} Awake");
        parts = GetComponentsInChildren<PartFlyElement>();
        Debug.Log($"[PartGroupController] {groupName} 共蒐集到 {parts.Length} 個 PartFlyElement");
    }

    public void ActivateGroup()
    {
        Debug.Log($"[PartGroupController] {groupName} ActivateGroup");
        // 若要整組飛入可在此加入動畫
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
