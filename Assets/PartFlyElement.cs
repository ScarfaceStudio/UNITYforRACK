using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]
public class PartFlyElement : MonoBehaviour
{
    [Header("����Ѽ�")]
    public Vector3 flyDirection = Vector3.up;  // ���X����V�]�b local space�^
    public float flyDistance = 3f;          // ���X���Z��
    public float flyTime = 1f;          // ����һݮɶ�

    Vector3 originPos;

    void Awake()
    {
        // �O����l localPosition
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
