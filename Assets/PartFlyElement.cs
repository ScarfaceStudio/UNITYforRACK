using UnityEngine;
using System.Collections;

public class PartFlyElement : MonoBehaviour
{
    [Header("飛出方向與參數")]
    public Vector3 flyDirection = Vector3.up;
    public float flySpeed = 1f;
    public float flyDistance = 1f;

    private Vector3 initialLocalPos;
    private bool isMoving = false;

    void Start()
    {
        initialLocalPos = transform.localPosition;
        Debug.Log($"[PartFlyElement] {name} 初始位置 {initialLocalPos}");
    }

    public void FlyOut()
    {
        if (isMoving) return;
        StartCoroutine(MoveTo(initialLocalPos + flyDirection.normalized * flyDistance, "FlyOut"));
    }

    public void ResetPosition()
    {
        if (isMoving) return;
        StartCoroutine(MoveTo(initialLocalPos, "Reset"));
    }

    private IEnumerator MoveTo(Vector3 target, string action)
    {
        isMoving = true;
        Debug.Log($"[PartFlyElement] {name} 開始 {action} 到 {target}");
        while (Vector3.Distance(transform.localPosition, target) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                target,
                flySpeed * Time.deltaTime
            );
            yield return null;
        }
        transform.localPosition = target;
        Debug.Log($"[PartFlyElement] {name} 完成 {action}");
        isMoving = false;
    }
}
