using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class GroupConfig
{
    public string groupName;
    public Button startButton;
    public Button backButton;
    public CanvasGroup subButtonsCanvasGroup;
    public Button[] partToggleButtons;
    public Transform rotationRoot;
    [Tooltip("Local offset to hide其他群組")]
    public Vector3 hideOffset = new Vector3(0, -10, 0);
    [HideInInspector] public Vector3 originalPosition;
    public PartFlyElement[] partFlyElements;
}

public class MainSequenceController : MonoBehaviour
{
    [Header("Group Settings")] public GroupConfig[] groups;
    [Header("Transition")] public float transitionTime = 1f;
    [Header("Interaction")] public float rotationSpeed = 100f, zoomSpeed = 1f;
    public float minScale = 0.5f, maxScale = 2f;

    private GroupConfig activeGroup;
    private bool allowRotate = false, allowZoom = false;

    void Awake()
    {
        foreach (var cfg in groups)
        {
            cfg.originalPosition = cfg.rotationRoot.localPosition;
            if (cfg.subButtonsCanvasGroup != null)
            {
                cfg.subButtonsCanvasGroup.alpha = 0;
                cfg.subButtonsCanvasGroup.interactable = false;
                cfg.subButtonsCanvasGroup.blocksRaycasts = false;
            }
            cfg.startButton.onClick.AddListener(() => EnterGroup(cfg));
            cfg.backButton.onClick.AddListener(() => ExitGroup(cfg));
            for (int i = 0; i < cfg.partToggleButtons.Length && i < cfg.partFlyElements.Length; i++)
            {
                int idx = i;
                cfg.partToggleButtons[idx].onClick.AddListener(() => cfg.partFlyElements[idx].ToggleFly());
            }
        }
    }

    void Update()
    {
        // 旋轉：按住左鍵時
        if (allowRotate && activeGroup != null && Input.GetMouseButton(0))
        {
            float h = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float v = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            activeGroup.rotationRoot.Rotate(Vector3.up, -h, Space.World);
            activeGroup.rotationRoot.Rotate(Vector3.right, v, Space.Self);
        }
        // 縮放：滾輪或雙指
        if (allowZoom && activeGroup != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f) ScaleGroup(scroll * zoomSpeed);
            if (Input.touchCount == 2)
            {
                Touch t0 = Input.GetTouch(0), t1 = Input.GetTouch(1);
                Vector2 prev0 = t0.position - t0.deltaPosition;
                Vector2 prev1 = t1.position - t1.deltaPosition;
                float prevDist = (prev0 - prev1).magnitude;
                float currDist = (t0.position - t1.position).magnitude;
                ScaleGroup((currDist - prevDist) * zoomSpeed * Time.deltaTime);
            }
        }
    }

    private void ScaleGroup(float delta)
    {
        float newScale = Mathf.Clamp(activeGroup.rotationRoot.localScale.x + delta, minScale, maxScale);
        activeGroup.rotationRoot.localScale = Vector3.one * newScale;
    }

    private void EnterGroup(GroupConfig cfg)
    {
        activeGroup = cfg;
        if (cfg.subButtonsCanvasGroup != null)
            StartCoroutine(FadeCanvas(cfg.subButtonsCanvasGroup, 0, 1, transitionTime));
        foreach (var g in groups)
            StartCoroutine(MoveTo(g.rotationRoot, (g == cfg ? g.originalPosition : g.originalPosition + g.hideOffset), transitionTime));
        StartCoroutine(EnableInteractionAfterDelay(transitionTime));
    }

    private IEnumerator EnableInteractionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        allowRotate = allowZoom = true;
    }

    private void ExitGroup(GroupConfig cfg)
    {
        foreach (var p in cfg.partFlyElements) p.ResetPosition();
        cfg.rotationRoot.localRotation = Quaternion.identity;
        cfg.rotationRoot.localScale = Vector3.one;
        if (cfg.subButtonsCanvasGroup != null)
            StartCoroutine(FadeCanvas(cfg.subButtonsCanvasGroup, 1, 0, transitionTime));
        allowRotate = allowZoom = false;
        foreach (var g in groups)
            StartCoroutine(MoveTo(g.rotationRoot, g.originalPosition, transitionTime));
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float dur)
    {
        cg.alpha = from; cg.interactable = to > 0; cg.blocksRaycasts = to > 0;
        float t = 0; while (t < dur) { t += Time.deltaTime; cg.alpha = Mathf.Lerp(from, to, t / dur); yield return null; }
        cg.alpha = to; cg.interactable = to > 0; cg.blocksRaycasts = to > 0;
    }

    private IEnumerator MoveTo(Transform tr, Vector3 tgt, float dur)
    {
        Vector3 start = tr.localPosition; float t = 0;
        while (t < dur) { t += Time.deltaTime; tr.localPosition = Vector3.Lerp(start, tgt, t / dur); yield return null; }
        tr.localPosition = tgt;
    }
}
