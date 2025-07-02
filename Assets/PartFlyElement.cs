using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]
public class PartFlyElement : MonoBehaviour
{
    [Header("飛行參數")]
    public Vector3 flyDirection = Vector3.up;  // 飛出的方向（在 local space）
    public float flyDistance = 3f;          // 飛出的距離
    public float flyTime = 1f;          // 飛行所需時間

    Vector3 originPos;

    void Awake()
    {
        // 記錄原始 localPosition
        originPos = transform.localPosition;
    }

    public void FlyAway()
    {
        Vector3 target = originPos + flyDirection.normalized * flyDistance;
        StopAllCoroutines();
        StartCoroutine(FlyTo(target));
    }

    public void FlyBack()
    {
        StopAllCoroutines();
        StartCoroutine(FlyTo(originPos));
    }

    IEnumerator FlyTo(Vector3 target)
    {
        Vector3 start = transform.localPosition;
        float t = 0f;
        while (t < flyTime)
        {
            transform.localPosition = Vector3.Lerp(start, target, t / flyTime);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = target;
    }
}
