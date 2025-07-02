using UnityEngine;
using UnityEngine.UI;

public class MainSequenceController : MonoBehaviour
{
    [Header("UI")]
    public Button btnStartA, btnStartB, btnStartC; // 主選單
    public Button btnBack;                        // 回主選單
    public RectTransform buttonContainer;                // 動態放子按鈕的父容器
    public Button subButtonPrefab;                // 子按鈕 Prefab

    [Header("模型群組")]
    public PartGroupController[] groups; // A,B,C……依序拖入

    PartGroupController current;  // 目前顯示的群組

    void Start()
    {
        // 綁定主選單
        btnStartA.onClick.AddListener(() => OnSelectGroup(0));
        btnStartB.onClick.AddListener(() => OnSelectGroup(1));
        btnStartC.onClick.AddListener(() => OnSelectGroup(2));
        btnBack.onClick.AddListener(OnBackToMain);

        ShowMainMenu();
    }

    void ShowMainMenu()
    {
        // 按鈕顯示
        btnStartA.gameObject.SetActive(true);
        btnStartB.gameObject.SetActive(true);
        btnStartC.gameObject.SetActive(true);
        btnBack.gameObject.SetActive(false);
        // 隱藏子按鈕
        foreach (Transform t in buttonContainer) Destroy(t.gameObject);
        // 把所有群組飛回原位，關閉互動
        foreach (var g in groups)
        {
            g.FlyBackGroup();
            g.DisableInteraction();
            g.ResetAll();
        }
        current = null;
    }

    void OnSelectGroup(int idx)
    {
        // 隱藏其它主選單
        btnStartA.gameObject.SetActive(false);
        btnStartB.gameObject.SetActive(false);
        btnStartC.gameObject.SetActive(false);
        btnBack.gameObject.SetActive(true);

        // 其餘群組都飛走，只有 idx 飛回
        for (int i = 0; i < groups.Length; i++)
        {
            if (i == idx)
            {
                groups[i].FlyBackGroup();
                groups[i].EnableInteraction();
                groups[i].ResetAll();
                current = groups[i];
            }
            else
            {
                groups[i].FlyAwayGroup();
                groups[i].DisableInteraction();
            }
        }

        // 動態產生子按鈕：對應 current.parts
        foreach (Transform t in buttonContainer) Destroy(t.gameObject);
        for (int i = 0; i < current.GetComponentsInChildren<PartFlyElement>().Length; i++)
        {
            int pi = i;
            var btn = Instantiate(subButtonPrefab, buttonContainer);
            btn.name = $"BtnSub_{idx}_{i}";
            btn.GetComponentInChildren<Text>().text = $"Part {i + 1}";
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                // 切換該零件
                var p = current.GetComponentsInChildren<PartFlyElement>()[pi];
                // 如果在原點就飛出，否則飛回
                if (Vector3.Distance(p.transform.localPosition, p.transform.parent.InverseTransformPoint(p.transform.position)) < 0.01f)
                    p.FlyAway();
                else
                    p.FlyBack();
            });
        }
    }

    void OnBackToMain()
    {
        ShowMainMenu();
    }
}
