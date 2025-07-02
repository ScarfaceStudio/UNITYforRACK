using UnityEngine;
using System.Collections;

public class PartFlyElement : MonoBehaviour
{
    [Tooltip("���a�Ŷ����X��V�P�Z�� (�Ҧp Y=2000 ��ܦV�W�� 2000)")]
    public Vector3 flyDirection;
    [Tooltip("����ʵe�ɶ�")]
    public float flyDuration = 0.5f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isOut = false;

    void Awake()
    {
        initialPosition = transform.localPosition;
        targetPosition = initialPosition + flyDirection;
    }

    public void ToggleFly()
    {
        StopAllCoroutines();
        StartCoroutine(MoveTo(isOut ? initialPosition : targetPosition, flyDuration));
        isOut = !isOut;
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
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, dest, t / dur);
            yield return null;
        }
        transform.localPosition = dest;
    }
}
