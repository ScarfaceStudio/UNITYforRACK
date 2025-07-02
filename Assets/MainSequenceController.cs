using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainSequenceController : MonoBehaviour
{
    [Header("UI �]�w")]
    public GameObject mainMenuUI;
    public Button btnBack;
    public GameObject[] groupUI;        // �����U�s�ժ����s�s�խ��O

    [Header("3D �s�ծڸ`�I")]
    public GameObject[] rotationRoots;  // RotationRoot_A, RotationRoot_B �K

    private int currentGroupIndex = -1;
    private List<PartGroupController> groupControllers = new List<PartGroupController>();

    void Start()
    {
        Debug.Log("[MainSequenceController] Start ��l��");
        // ��ܥD���A���éҦ��s�խ��O�P��D�����s
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

    // Inspector ����U BtnStartX �s�즹��k�A�ña�J���� index
    public void OnClickStartGroup(int groupIndex)
    {
        Debug.Log($"[MainSequenceController] OnClickStartGroup({groupIndex})");
        if (groupIndex < 0 || groupIndex >= rotationRoots.Length) return;

        // ���åD���B��ܪ�D���s
        mainMenuUI.SetActive(false);
        btnBack.gameObject.SetActive(true);

        // �p�G���e���}�L�s�աA�n��������
        if (currentGroupIndex != -1)
        {
            Debug.Log($"[MainSequenceController] �����¸s�� {currentGroupIndex}");
            groupControllers[currentGroupIndex].FlyOutGroup();
            groupControllers[currentGroupIndex].ResetGroup();
            rotationRoots[currentGroupIndex].SetActive(false);
            groupUI[currentGroupIndex].SetActive(false);
        }

        // ��ܷs�s��
        currentGroupIndex = groupIndex;
        Debug.Log($"[MainSequenceController] ��ܷs�s�� {currentGroupIndex}");
        rotationRoots[currentGroupIndex].SetActive(true);
        groupControllers[currentGroupIndex].ActivateGroup();
        groupUI[currentGroupIndex].SetActive(true);
    }

    public void OnClickBack()
    {
        Debug.Log("[MainSequenceController] OnClickBack ��^�D���");
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
