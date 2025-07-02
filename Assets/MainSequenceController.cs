using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GroupUI
{
    [Header("主選單按鈕 (Start)")]
    public Button startButton;

    [Header("群組功能按鈕 (Parts)")]
    public GameObject[] partButtons;   // e.g. BtnA1, BtnA2...

    [Header("返回主選單按鈕 (Back)")]
    public Button backButton;          // 每組獨立的 Back 按鈕

    [Header("3D 群組根節點 (RotationRoot_X)")]
    public GameObject rotationRoot;

    [HideInInspector]
    public PartGroupController controller;
}

public class MainSequenceController : MonoBehaviour
{
    [Header("群組設定")]
    public GroupUI[] groups;       // 在 Inspector 裡設成你有幾組，比如 A/B/C

    private int currentGroup = -1;

    void Start()
    {
        Debug.Log("[MainSequenceController] Start 初始化");

        for (int i = 0; i < groups.Length; i++)
        {
            var g = groups[i];
            // 取得 PartGroupController
            g.controller = g.rotationRoot.GetComponent<PartGroupController>();

            // 綁定 Start 按鈕
            int idx = i;
            g.startButton.onClick.AddListener(() => OnClickGroup(idx));

            // 綁定各組 Back 按鈕
            g.backButton.onClick.AddListener(OnClickBack);

            // 初始狀態：Start 顯示，Parts/Back/3D 隱藏
            g.startButton.gameObject.SetActive(true);
            g.backButton.gameObject.SetActive(false);
            foreach (var b in g.partButtons) b.SetActive(false);
            g.rotationRoot.SetActive(false);
        }
    }

    // 點擊「Start X」
    private void OnClickGroup(int idx)
    {
        Debug.Log($"[MainSequenceController] OnClickGroup({idx})");

        currentGroup = idx;

        // 隱藏所有 Start 按鈕
        foreach (var g in groups)
            g.startButton.gameObject.SetActive(false);

        // 顯示被選組的 Parts、Back、3D，其他組全隱藏
        for (int i = 0; i < groups.Length; i++)
        {
            var g = groups[i];
            bool isActive = (i == idx);
            // 功能按鈕
            foreach (var b in g.partButtons)
                b.SetActive(isActive);
            // 本組 Back
            g.backButton.gameObject.SetActive(isActive);
            // 3D 群組
            g.rotationRoot.SetActive(isActive);

            if (isActive)
            {
                Debug.Log($"[MainSequenceController] ActivateGroup {i}");
                g.controller.ActivateGroup();
            }
            else
            {
                // 隱藏時可先歸位
                g.controller.ResetGroup();
            }
        }
    }

    // 點擊任一組的 Back
    private void OnClickBack()
    {
        Debug.Log("[MainSequenceController] OnClickBack");

        if (currentGroup >= 0 && currentGroup < groups.Length)
        {
            var g = groups[currentGroup];
            // 隱藏本組 Parts、Back、3D
            foreach (var b in g.partButtons)
                b.SetActive(false);
            g.backButton.gameObject.SetActive(false);
            g.controller.ResetGroup();
            g.rotationRoot.SetActive(false);
            currentGroup = -1;
        }

        // 顯示所有 Start
        foreach (var g in groups)
            g.startButton.gameObject.SetActive(true);
    }
}
