using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GroupUI
{
    [Header("�D�����s (Start)")]
    public Button startButton;

    [Header("�s�ե\����s (Parts)")]
    public GameObject[] partButtons;   // e.g. BtnA1, BtnA2...

    [Header("��^�D�����s (Back)")]
    public Button backButton;          // �C�տW�ߪ� Back ���s

    [Header("3D �s�ծڸ`�I (RotationRoot_X)")]
    public GameObject rotationRoot;

    [HideInInspector]
    public PartGroupController controller;
}

public class MainSequenceController : MonoBehaviour
{
    [Header("�s�ճ]�w")]
    public GroupUI[] groups;       // �b Inspector �̳]���A���X�աA��p A/B/C

    private int currentGroup = -1;

    void Start()
    {
        Debug.Log("[MainSequenceController] Start ��l��");

        for (int i = 0; i < groups.Length; i++)
        {
            var g = groups[i];
            // ���o PartGroupController
            g.controller = g.rotationRoot.GetComponent<PartGroupController>();

            // �j�w Start ���s
            int idx = i;
            g.startButton.onClick.AddListener(() => OnClickGroup(idx));

            // �j�w�U�� Back ���s
            g.backButton.onClick.AddListener(OnClickBack);

            // ��l���A�GStart ��ܡAParts/Back/3D ����
            g.startButton.gameObject.SetActive(true);
            g.backButton.gameObject.SetActive(false);
            foreach (var b in g.partButtons) b.SetActive(false);
            g.rotationRoot.SetActive(false);
        }
    }

    // �I���uStart X�v
    private void OnClickGroup(int idx)
    {
        Debug.Log($"[MainSequenceController] OnClickGroup({idx})");

        currentGroup = idx;

        // ���éҦ� Start ���s
        foreach (var g in groups)
            g.startButton.gameObject.SetActive(false);

        // ��ܳQ��ժ� Parts�BBack�B3D�A��L�ե�����
        for (int i = 0; i < groups.Length; i++)
        {
            var g = groups[i];
            bool isActive = (i == idx);
            // �\����s
            foreach (var b in g.partButtons)
                b.SetActive(isActive);
            // ���� Back
            g.backButton.gameObject.SetActive(isActive);
            // 3D �s��
            g.rotationRoot.SetActive(isActive);

            if (isActive)
            {
                Debug.Log($"[MainSequenceController] ActivateGroup {i}");
                g.controller.ActivateGroup();
            }
            else
            {
                // ���îɥi���k��
                g.controller.ResetGroup();
            }
        }
    }

    // �I�����@�ժ� Back
    private void OnClickBack()
    {
        Debug.Log("[MainSequenceController] OnClickBack");

        if (currentGroup >= 0 && currentGroup < groups.Length)
        {
            var g = groups[currentGroup];
            // ���å��� Parts�BBack�B3D
            foreach (var b in g.partButtons)
                b.SetActive(false);
            g.backButton.gameObject.SetActive(false);
            g.controller.ResetGroup();
            g.rotationRoot.SetActive(false);
            currentGroup = -1;
        }

        // ��ܩҦ� Start
        foreach (var g in groups)
            g.startButton.gameObject.SetActive(true);
    }
}
