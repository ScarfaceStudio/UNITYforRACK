using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainSequenceController : MonoBehaviour
{
    [Header("UI 設定")]
    public GameObject mainMenuUI;
    public Button btnBack;
    public GameObject[] groupUI;        // 對應各群組的按鈕群組面板

    [Header("3D 群組根節點")]
    public GameObject[] rotationRoots;  // RotationRoot_A, RotationRoot_B …

    private int currentGroupIndex = -1;
    private List<PartGroupController> groupControllers = new List<PartGroupController>();

    void Start()
    {
        Debug.Log("[MainSequenceController] Start 初始化");
        // 顯示主選單，隱藏所有群組面板與返主選單按鈕
        mainMenuUI.SetActive(true);
        btnBack.gameObject.SetActive(false);
        for (int i = 0; i < groupUI.Length; i++)
            groupUI[i].SetActive(false);
        for (int i = 0; i < rotationRoots.Length; i++)
        {
            rotationRoots[i].SetActive(false);
            var pgc = rotationRoots[i].GetComponent<PartGroupController>();
            if (pgc != null) groupControllers.Add(pgc);
        }
    }

    // Inspector 中把各 BtnStartX 連到此方法，並帶入對應 index
    public void OnClickStartGroup(int groupIndex)
    {
        Debug.Log($"[MainSequenceController] OnClickStartGroup({groupIndex})");
        if (groupIndex < 0 || groupIndex >= rotationRoots.Length) return;

        // 隱藏主選單、顯示返主按鈕
        mainMenuUI.SetActive(false);
        btnBack.gameObject.SetActive(true);

        // 如果之前有開過群組，要先關閉它
        if (currentGroupIndex != -1)
        {
            Debug.Log($"[MainSequenceController] 隱藏舊群組 {currentGroupIndex}");
            groupControllers[currentGroupIndex].FlyOutGroup();
            groupControllers[currentGroupIndex].ResetGroup();
            rotationRoots[currentGroupIndex].SetActive(false);
            groupUI[currentGroupIndex].SetActive(false);
        }

        // 顯示新群組
        currentGroupIndex = groupIndex;
        Debug.Log($"[MainSequenceController] 顯示新群組 {currentGroupIndex}");
        rotationRoots[currentGroupIndex].SetActive(true);
        groupControllers[currentGroupIndex].ActivateGroup();
        groupUI[currentGroupIndex].SetActive(true);
    }

    public void OnClickBack()
    {
        Debug.Log("[MainSequenceController] OnClickBack 返回主選單");
        if (currentGroupIndex != -1)
        {
            groupControllers[currentGroupIndex].ResetGroup();
            rotationRoots[currentGroupIndex].SetActive(false);
            groupUI[currentGroupIndex].SetActive(false);
            currentGroupIndex = -1;
        }
        mainMenuUI.SetActive(true);
        btnBack.gameObject.SetActive(false);
    }
}
