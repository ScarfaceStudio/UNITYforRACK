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
    public Button[] partToggleButtons;           // 子按鈕：BtnA1、BtnA2… 對應 PartFlyElement
    public Transform rotationRoot;               // RotationRoot_A, RotationRoot_B...
    [Tooltip("Local offset to hide other groups when inactive")]
    public Vector3 hideOffset = new Vector3(0, -10, 0);
    [HideInInspector] public Vector3 originalPosition;
    public PartFlyElement[] partFlyElements;     // PartA1, PartA2…
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
        // 初始化：快取位置、隱藏子 UI、綁定按鈕事件
        foreach (var cfg in groups)
        {
            cfg.originalPosition = cfg.rotationRoot.localPosition;

            // 主畫面隱藏子選單
            if (cfg.subButtonsCanvasGroup != null)
            {
                cfg.subButtonsCanvasGroup.alpha = 0;
                cfg.subButtonsCanvasGroup.interactable = false;
                cfg.subButtonsCanvasGroup.blocksRaycasts = false;
            }

            // 點 Start → 進入該群組
            cfg.startButton.onClick.AddListener(() => EnterGroup(cfg));
            // 點 Back → 返回主畫面
            cfg.backButton.onClick.AddListener(() => ExitGroup(cfg));

            // 點子按鈕 → 切換部件飛出／縮回
            for (int i = 0; i < cfg.partToggleButtons.Length && i < cfg.partFlyElements.Length; i++)
            {
                int idx = i;
                cfg.partToggleButtons[idx].onClick.AddListener(() => {
                    cfg.partFlyElements[idx].ToggleFly();
                });
            }
        }

        allowRotate = allowZoom = false;  // 主畫面不允許互動
    }

    void Update()
    {
        // 旋轉：不需按鍵，點擊 Start 後即可拖曳旋轉
        if (allowRotate && activeGroup != null)
        {
            float h = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float v = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            activeGroup.rotationRoot.Rotate(v, -h, 0, Space.World);
        }

        // 縮放：僅在按住左鍵（或觸控壓下）時，用滾輪／捏合手勢生效
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
            // 若需支援觸控雙指捏合，可在此加入 Touch.fingerCount == 2 並計算手指距離變化
        }
    }

    private void EnterGroup(GroupConfig cfg)
    {
        activeGroup = cfg;
        // 淡入子按鈕
        if (cfg.subButtonsCanvasGroup != null)
            StartCoroutine(FadeCanvas(cfg.subButtonsCanvasGroup, 0, 1, transitionTime));

        // 顯示當前群組，隱藏其他
        foreach (var g in groups)
        {
            Vector3 tgt = (g == cfg) ? g.originalPosition : g.originalPosition + g.hideOffset;
            StartCoroutine(MoveTo(g.rotationRoot, tgt, transitionTime));
        }
        // 過場完後開放互動
        StartCoroutine(EnableInteractionAfterDelay(transitionTime));
    }

    private IEnumerator EnableInteractionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        allowRotate = allowZoom = true;
    }

    private void ExitGroup(GroupConfig cfg)
    {
        // 重置所有部件位置
        foreach (var part in cfg.partFlyElements)
            part.ResetPosition();

        // 重置模型旋轉與縮放
        cfg.rotationRoot.localRotation = Quaternion.identity;
        cfg.rotationRoot.localScale = Vector3.one;

        // 淡出子按鈕
        if (cfg.subButtonsCanvasGroup != null)
            StartCoroutine(FadeCanvas(cfg.subButtonsCanvasGroup, 1, 0, transitionTime));

        allowRotate = allowZoom = false;  // 停止互動

        // 恢復所有群組到原始位置
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
