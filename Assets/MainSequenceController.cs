using UnityEngine;
using UnityEngine.UI;

public class MainSequenceController : MonoBehaviour
{
    [Header("UI")]
    public Button btnStartA, btnStartB, btnStartC; // �D���
    public Button btnBack;                        // �^�D���
    public RectTransform buttonContainer;                // �ʺA��l���s�����e��
    public Button subButtonPrefab;                // �l���s Prefab

    [Header("�ҫ��s��")]
    public PartGroupController[] groups; // A,B,C�K�K�̧ǩ�J

    PartGroupController current;  // �ثe��ܪ��s��

    void Start()
    {
        // �j�w�D���
        btnStartA.onClick.AddListener(() => OnSelectGroup(0));
        btnStartB.onClick.AddListener(() => OnSelectGroup(1));
        btnStartC.onClick.AddListener(() => OnSelectGroup(2));
        btnBack.onClick.AddListener(OnBackToMain);

        ShowMainMenu();
    }

    void ShowMainMenu()
    {
        // ���s���
        btnStartA.gameObject.SetActive(true);
        btnStartB.gameObject.SetActive(true);
        btnStartC.gameObject.SetActive(true);
        btnBack.gameObject.SetActive(false);
        // ���äl���s
        foreach (Transform t in buttonContainer) Destroy(t.gameObject);
        // ��Ҧ��s�խ��^���A��������
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
        // ���è䥦�D���
        btnStartA.gameObject.SetActive(false);
        btnStartB.gameObject.SetActive(false);
        btnStartC.gameObject.SetActive(false);
        btnBack.gameObject.SetActive(true);

        // ��l�s�ճ������A�u�� idx ���^
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

        // �ʺA���ͤl���s�G���� current.parts
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
                // �����ӹs��
                var p = current.GetComponentsInChildren<PartFlyElement>()[pi];
                // �p�G�b���I�N���X�A�_�h���^
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
