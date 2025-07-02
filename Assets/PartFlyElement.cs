// PartFlyElement.cs
using UnityEngine;
using System.Collections;

public class PartFlyElement : MonoBehaviour
{
    [Tooltip("���a�Ŷ����X��V")]
    public Vector3 flyDirection = Vector3.up;
    [Tooltip("���X�Z��")]
    public float flyDistance = 1f;
    [Tooltip("����ʵe�ɶ�")]
    public float flyDuration = 0.5f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isOut = false;

    void Awake()
    {
        initialPosition = transform.localPosition;
        targetPosition = initialPosition + flyDirection.normalized * flyDistance;
    }

    public void ToggleFly()
    {
        StopAllCoroutines();
        if (!isOut)
        {
            StartCoroutine(MoveTo(targetPosition, flyDuration));
            isOut = true;
        }
        else
        {
            StartCoroutine(MoveTo(initialPosition, flyDuration));
            isOut = false;
        }
    }

    public void ResetPosition()
    {
        StopAllCoroutines();
        transform.localPosition = initialPosition;
        isOut = false;
    }

    private IEnumerator MoveTo(Vector3 dest, float dur)
    {
        Vector3 start = transform.localPosition;
        float t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, dest, t / dur);
            yield return null;
        }
        transform.localPosition = dest;
    }
}
