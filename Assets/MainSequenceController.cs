using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class GroupUI
{
    public Button startButton;     // BtnStartX
    public GameObject[] partButtons;  // BtnAX1, BtnAX2�K
    public Button backButton;      // BtnBackX
    public GameObject rotationRoot;   // RotationRoot_X

    [HideInInspector] public CanvasGroup[] partCanvasGroups;
    [HideInInspector] public CanvasGroup backCanvasGroup;
    [HideInInspector] public Vector3 rootInitialPosition;
}

public class GameController : MonoBehaviour
{
    [Header("�s�ճ]�w")]
    public GroupUI[] groups;               // Inspector �]�w�U��

    [Header("UI �H�J/�H�X�ɶ�")]
    public float uiFadeDuration = 1f;

    [Header("�ҫ����X/�^�k�ɶ�")]
    public float modelFlyDuration = 1f;

    [Header("�ҫ����X���� (Z ��V)")]
    public Vector3 modelHideOffset = new Vector3(0, 0, 10f);

    void Awake()
    {
        // ��l�ƨC�աG�O����l��m�B�[�W CanvasGroup�B�j�ƥ�
        for (int i = 0; i < groups.Length; i++)
        {
            var g = groups[i];
            // 1. �O�� RotationRoot ��l��m
            g.rootInitialPosition = g.rotationRoot.transform.localPosition;

            // 2. ���C�� Part Button �[ CanvasGroup
            g.partCanvasGroups = new CanvasGroup[g.partButtons.Length];
            for (int j = 0; j < g.partButtons.Length; j++)
            {
                var go = g.partButtons[j];
                var cg = go.GetComponent<CanvasGroup>() ?? go.AddComponent<CanvasGroup>();
                cg.alpha = 0f; cg.interactable = false; cg.blocksRaycasts = false;
                g.partCanvasGroups[j] = cg;

                // �j�w���s Toggle �� Part
                int partIndex = j;
                var btn = go.GetComponent<Button>();
                var partFly = g.rotationRoot.transform.GetChild(partIndex).GetComponent<PartFlyElement>();
                btn.onClick.AddListener(() => partFly.Toggle());
            }

            // 3. �� Back Button �[ CanvasGroup
            {
                var go = g.backButton.gameObject;
                var cg = go.GetComponent<CanvasGroup>() ?? go.AddComponent<CanvasGroup>();
                cg.alpha = 0f; cg.interactable = false; cg.blocksRaycasts = false;
                g.backCanvasGroup = cg;
            }

            // 4. �j�w Start/Back �ƥ�
            int idx = i;
            g.startButton.onClick.AddListener(() => OnClickGroup(idx));
            g.backButton.onClick.AddListener(() => OnClickBack(idx));

            // 5. �T�O RotationRoot �b��l��m
            g.rotationRoot.transform.localPosition = g.rootInitialPosition;
        }
    }

    // �H�J/�H�X CanvasGroup
    IEnumerator FadeCanvas(CanvasGroup cg, bool fadeIn)
    {
        float elapsed = 0f;
        float start = cg.alpha;
        float end = (fadeIn ? 1f : 0f);
        cg.interactable = fadeIn;
        cg.blocksRaycasts = fadeIn;

        while (elapsed < uiFadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / uiFadeDuration);
            yield return null;
        }
        cg.alpha = end;
    }

    // �ҫ����X/�^�k
    IEnumerator MoveRoot(Transform t, Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < modelFlyDuration)
        {
            elapsed += Time.deltaTime;
            t.localPosition = Vector3.Lerp(from, to, elapsed / modelFlyDuration);
            yield return null;
        }
        t.localPosition = to;
    }

    // �I StartX�G�H�J���� PartButtons & Back�B��l�ҫ����X
    void OnClickGroup(int idx)
    {
        var g = groups[idx];
        // UI �H�J
        foreach (var cg in g.partCanvasGroups) StartCoroutine(FadeCanvas(cg, true));
        StartCoroutine(FadeCanvas(g.backCanvasGroup, true));

        // �ҫ��G���զ^�k�B��l���X
        for (int i = 0; i < groups.Length; i++)
        {
            var root = groups[i].rotationRoot.transform;
            var from = root.localPosition;
            var to = (i == idx)
                ? groups[i].rootInitialPosition
                : groups[i].rootInitialPosition + modelHideOffset;
            StartCoroutine(MoveRoot(root, from, to));
        }
    }

    // �I BackX�G�H�X���� UI�B�Ҧ��ҫ��^�k
    void OnClickBack(int idx)
    {
        var g = groups[idx];
        // UI �H�X
        foreach (var cg in g.partCanvasGroups) StartCoroutine(FadeCanvas(cg, false));
        StartCoroutine(FadeCanvas(g.backCanvasGroup, false));

        // �ҫ��G�Ҧ��^���l
        for (int i = 0; i < groups.Length; i++)
        {
            var root = groups[i].rotationRoot.transform;
            var from = root.localPosition;
            var to = groups[i].rootInitialPosition;
            StartCoroutine(MoveRoot(root, from, to));
        }
    }
}
