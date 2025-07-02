using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class GroupUI
{
    public Button startButton;     // BtnStartX
    public GameObject[] partButtons;  // BtnAX1, BtnAX2…
    public Button backButton;      // BtnBackX
    public GameObject rotationRoot;   // RotationRoot_X

    [HideInInspector] public CanvasGroup[] partCanvasGroups;
    [HideInInspector] public CanvasGroup backCanvasGroup;
    [HideInInspector] public Vector3 rootInitialPosition;
}

public class GameController : MonoBehaviour
{
    [Header("群組設定")]
    public GroupUI[] groups;               // Inspector 設定各組

    [Header("UI 淡入/淡出時間")]
    public float uiFadeDuration = 1f;

    [Header("模型飛出/回歸時間")]
    public float modelFlyDuration = 1f;

    [Header("模型飛出偏移 (Z 方向)")]
    public Vector3 modelHideOffset = new Vector3(0, 0, 10f);

    void Awake()
    {
        // 初始化每組：記錄初始位置、加上 CanvasGroup、綁事件
        for (int i = 0; i < groups.Length; i++)
        {
            var g = groups[i];
            // 1. 記錄 RotationRoot 初始位置
            g.rootInitialPosition = g.rotationRoot.transform.localPosition;

            // 2. 為每個 Part Button 加 CanvasGroup
            g.partCanvasGroups = new CanvasGroup[g.partButtons.Length];
            for (int j = 0; j < g.partButtons.Length; j++)
            {
                var go = g.partButtons[j];
                var cg = go.GetComponent<CanvasGroup>() ?? go.AddComponent<CanvasGroup>();
                cg.alpha = 0f; cg.interactable = false; cg.blocksRaycasts = false;
                g.partCanvasGroups[j] = cg;

                // 綁定按鈕 Toggle 該 Part
                int partIndex = j;
                var btn = go.GetComponent<Button>();
                var partFly = g.rotationRoot.transform.GetChild(partIndex).GetComponent<PartFlyElement>();
                btn.onClick.AddListener(() => partFly.Toggle());
            }

            // 3. 為 Back Button 加 CanvasGroup
            {
                var go = g.backButton.gameObject;
                var cg = go.GetComponent<CanvasGroup>() ?? go.AddComponent<CanvasGroup>();
                cg.alpha = 0f; cg.interactable = false; cg.blocksRaycasts = false;
                g.backCanvasGroup = cg;
            }

            // 4. 綁定 Start/Back 事件
            int idx = i;
            g.startButton.onClick.AddListener(() => OnClickGroup(idx));
            g.backButton.onClick.AddListener(() => OnClickBack(idx));

            // 5. 確保 RotationRoot 在初始位置
            g.rotationRoot.transform.localPosition = g.rootInitialPosition;
        }
    }

    // 淡入/淡出 CanvasGroup
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

    // 模型飛出/回歸
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

    // 點 StartX：淡入本組 PartButtons & Back、其餘模型飛出
    void OnClickGroup(int idx)
    {
        var g = groups[idx];
        // UI 淡入
        foreach (var cg in g.partCanvasGroups) StartCoroutine(FadeCanvas(cg, true));
        StartCoroutine(FadeCanvas(g.backCanvasGroup, true));

        // 模型：本組回歸、其餘飛出
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

    // 點 BackX：淡出本組 UI、所有模型回歸
    void OnClickBack(int idx)
    {
        var g = groups[idx];
        // UI 淡出
        foreach (var cg in g.partCanvasGroups) StartCoroutine(FadeCanvas(cg, false));
        StartCoroutine(FadeCanvas(g.backCanvasGroup, false));

        // 模型：所有回到初始
        for (int i = 0; i < groups.Length; i++)
        {
            var root = groups[i].rotationRoot.transform;
            var from = root.localPosition;
            var to = groups[i].rootInitialPosition;
            StartCoroutine(MoveRoot(root, from, to));
        }
    }
}
