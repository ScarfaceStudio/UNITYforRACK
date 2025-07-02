// MainSequenceController.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class GroupConfig
{
    public string groupName;                     // e.g., "A"
    public Button startButton;                   // BtnStartA, BtnStartB...
    public Button backButton;                    // BtnBackA, BtnBackB...
    public CanvasGroup subButtonsCanvasGroup;    // Container (CanvasGroup) for BtnA1, BtnA2, BtnBackA
    public Button[] partToggleButtons;           // �l���s�GBtnA1�BBtnA2�K ���� PartFlyElement
    public Transform rotationRoot;               // RotationRoot_A, RotationRoot_B...
    [Tooltip("Local offset to hide other groups when inactive")]
    public Vector3 hideOffset = new Vector3(0, -10, 0);
    [HideInInspector] public Vector3 originalPosition;
    public PartFlyElement[] partFlyElements;     // PartA1, PartA2�K
}

public class MainSequenceController : MonoBehaviour
{
    [Header("Group Settings")]
    public GroupConfig[] groups;
    [Header("Transition Settings")]
    public float transitionTime = 1f;

    [Header("Interaction Settings")]
    public float rotationSpeed = 100f;
    public float zoomSpeed = 2f;
    public float minScale = 0.5f;
    public float maxScale = 2f;

    private GroupConfig activeGroup;
    private bool allowRotate = false;
    private bool allowZoom = false;

    void Awake()
    {
        // ��l�ơG�֨���m�B���äl UI�B�j�w���s�ƥ�
        foreach (var cfg in groups)
        {
            cfg.originalPosition = cfg.rotationRoot.localPosition;

            // �D�e�����äl���
            if (cfg.subButtonsCanvasGroup != null)
            {
                cfg.subButtonsCanvasGroup.alpha = 0;
                cfg.subButtonsCanvasGroup.interactable = false;
                cfg.subButtonsCanvasGroup.blocksRaycasts = false;
            }

            // �I Start �� �i�J�Ӹs��
            cfg.startButton.onClick.AddListener(() => EnterGroup(cfg));
            // �I Back �� ��^�D�e��
            cfg.backButton.onClick.AddListener(() => ExitGroup(cfg));

            // �I�l���s �� �������󭸥X���Y�^
            for (int i = 0; i < cfg.partToggleButtons.Length && i < cfg.partFlyElements.Length; i++)
            {
                int idx = i;
                cfg.partToggleButtons[idx].onClick.AddListener(() => {
                    cfg.partFlyElements[idx].ToggleFly();
                });
            }
        }

        allowRotate = allowZoom = false;  // �D�e�������\����
    }

    void Update()
    {
        // ����G���ݫ���A�I�� Start ��Y�i�즲����
        if (allowRotate && activeGroup != null)
        {
            float h = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float v = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            activeGroup.rotationRoot.Rotate(v, -h, 0, Space.World);
        }

        // �Y��G�Ȧb������]��Ĳ�����U�^�ɡA�κu�������X��եͮ�
        if (allowZoom && activeGroup != null && Input.GetMouseButton(0))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                Vector3 s = activeGroup.rotationRoot.localScale;
                s += Vector3.one * scroll * zoomSpeed;
                s = Vector3.ClampMagnitude(s, maxScale);
                s = new Vector3(
                    Mathf.Clamp(s.x, minScale, maxScale),
                    Mathf.Clamp(s.y, minScale, maxScale),
                    Mathf.Clamp(s.z, minScale, maxScale)
                );
                activeGroup.rotationRoot.localScale = s;
            }
            // �Y�ݤ䴩Ĳ���������X�A�i�b���[�J Touch.fingerCount == 2 �íp�����Z���ܤ�
        }
    }

    private void EnterGroup(GroupConfig cfg)
    {
        activeGroup = cfg;
        // �H�J�l���s
        if (cfg.subButtonsCanvasGroup != null)
            StartCoroutine(FadeCanvas(cfg.subButtonsCanvasGroup, 0, 1, transitionTime));

        // ��ܷ�e�s�աA���è�L
        foreach (var g in groups)
        {
            Vector3 tgt = (g == cfg) ? g.originalPosition : g.originalPosition + g.hideOffset;
            StartCoroutine(MoveTo(g.rotationRoot, tgt, transitionTime));
        }
        // �L������}�񤬰�
        StartCoroutine(EnableInteractionAfterDelay(transitionTime));
    }

    private IEnumerator EnableInteractionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        allowRotate = allowZoom = true;
    }

    private void ExitGroup(GroupConfig cfg)
    {
        // ���m�Ҧ������m
        foreach (var part in cfg.partFlyElements)
            part.ResetPosition();

        // ���m�ҫ�����P�Y��
        cfg.rotationRoot.localRotation = Quaternion.identity;
        cfg.rotationRoot.localScale = Vector3.one;

        // �H�X�l���s
        if (cfg.subButtonsCanvasGroup != null)
            StartCoroutine(FadeCanvas(cfg.subButtonsCanvasGroup, 1, 0, transitionTime));

        allowRotate = allowZoom = false;  // �����

        // ��_�Ҧ��s�ը��l��m
        foreach (var g in groups)
            StartCoroutine(MoveTo(g.rotationRoot, g.originalPosition, transitionTime));
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float dur)
    {
        cg.alpha = from;
        cg.interactable = to > 0;
        cg.blocksRaycasts = to > 0;
        float t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }
        cg.alpha = to;
        cg.interactable = to > 0;
        cg.blocksRaycasts = to > 0;
    }

    private IEnumerator MoveTo(Transform tr, Vector3 target, float dur)
    {
        Vector3 start = tr.localPosition;
        float t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            tr.localPosition = Vector3.Lerp(start, target, t / dur);
            yield return null;
        }
        tr.localPosition = target;
    }
}
