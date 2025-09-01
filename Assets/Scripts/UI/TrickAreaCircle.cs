using UnityEngine;

public class TrickAreaCircle : MonoBehaviour
{
    [Tooltip("중앙 원 반지름(px)")]
    public float radius = 120f;

    [Tooltip("시작 각도(내 슬롯이 살짝 아래쪽)")]
    public float startAngleDeg = -90f;

    public RectTransform PlaceCardAt(int seatIndex, int playerCount, RectTransform card)
    {
        float step = 360f / playerCount;
        float angle = startAngleDeg + step * seatIndex;
        float rad = angle * Mathf.Deg2Rad;
        Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

        card.SetParent(transform, false);
        card.anchoredPosition = pos;
        card.localEulerAngles = Vector3.zero;
        return card;
    }
}
