using UnityEngine;

[ExecuteAlways]
public class MyHandOverlapLayout : MonoBehaviour
{
    [Range(-40f, 40f)] public float overlap = -24f; // 음수면 겹칩니다(좌우)
    public float yOffset = 0f;

    void OnTransformChildrenChanged() => Layout();
    void OnValidate() => Layout();

    public void Layout()
    {
        int n = transform.childCount;
        if (n == 0) return;

        float x = 0f;
        for (int i = 0; i < n; i++)
        {
            var rt = transform.GetChild(i) as RectTransform;
            if (!rt) continue;
            rt.anchoredPosition = new Vector2(x, yOffset);
            rt.localEulerAngles = Vector3.zero;
            x += rt.sizeDelta.x + overlap;
        }
        // 가운데 정렬
        var self = transform as RectTransform;
        self.pivot = new Vector2(0.5f, 0.5f);
        self.anchoredPosition = new Vector2(0, self.anchoredPosition.y);
    }
}
