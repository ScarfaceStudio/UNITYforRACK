using UnityEngine;
using System.Collections;

public class PartFlyElement : MonoBehaviour
{
    [Header("���X & �^��]�w")]
    public Vector3 flyDirection = Vector3.up;
    public float flySpeed = 1f;
    public float flyDistance = 1f;

    private Vector3 initialLocalPos;
    private bool isMoving = false;
    public bool isOut = false;

    void Start()
    {
        initialLocalPos = transform.localPosition;
        Debug.Log($"[PartFlyElement] {name} ��l��m: {initialLocalPos}");
    }

    // �������X / �^��
    public void Toggle()
    {
        if (isMoving) return;
        if (!isOut) FlyOut();
        else ResetPosition();
        isOut = !isOut;
    }

    public void FlyOut()
    {
        StartCoroutine(MoveTo(initialLocalPos + flyDirection.normalized * flyDistance, "FlyOut"));
    }

    public void ResetPosition()
    {
        StartCoroutine(MoveTo(initialLocalPos, "Reset"));
    }

    IEnumerator MoveTo(Vector3 target, string action)
    {
        isMoving = true;
        Debug.Log($"[PartFlyElement] {name} �}�l {action} �� {target}");
        while (Vector3.Distance(transform.localPosition, target) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, flySpeed * Time.deltaTime);
            yield return null;
        }
        transform.localPosition = target;
        Debug.Log($"[PartFlyElement] {name} ���� {action}");
        isMoving = false;
    }
}
