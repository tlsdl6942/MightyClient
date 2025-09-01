using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SeatsCircleArranger : MonoBehaviour
{
    [Tooltip("좌석으로 쓸 Seat 프리팹(상대용)")]
    public GameObject seatPrefab;

    [Tooltip("총 플레이어 수")]
    [Range(3, 6)] public int playerCount = 5;

    [Tooltip("반지름(px). Canvas 크기에 맞게 조절")]
    public float radius = 380f;

    [Tooltip("내 자리가 화면 아래 중앙이 되도록 시작 각도(-90도)")]
    public float startAngleDeg = -90f;

    [Tooltip("시계 방향으로 배치")]
    public bool clockwise = true;

    [Header("생성된 좌석들(읽기전용)")]
    public List<RectTransform> seats = new List<RectTransform>();

    public void BuildSeatsIfNeeded()
    {
        // 자식들 정리 (Seat만 남기거나 재생성)
        if (seats.Count != playerCount || transform.childCount != playerCount)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            seats.Clear();
            for (int i = 0; i < playerCount; i++)
            {
                var go = Instantiate(seatPrefab, transform);
                go.name = $"Seat_{i}";
                var rt = go.transform as RectTransform;
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); // 중앙 기준
                seats.Add(rt);
            }
        }
    }

    void OnEnable() { BuildSeatsIfNeeded(); Layout(); }
    void OnValidate() { BuildSeatsIfNeeded(); Layout(); }
    void OnRectTransformDimensionsChange() { Layout(); }

    public void Layout()
    {
        if (seats.Count == 0) return;
        float step = 360f / playerCount;
        float sign = clockwise ? 1f : -1f;

        for (int i = 0; i < playerCount; i++)
        {
            float angle = startAngleDeg + sign * step * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

            var rt = seats[i];
            rt.anchoredPosition = pos;
            rt.localEulerAngles = Vector3.zero;
        }
    }
}
