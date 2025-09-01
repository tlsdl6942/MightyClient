using UnityEngine;

public class HandStripLayout : MonoBehaviour
{
    public enum Orientation { Horizontal, Vertical }
    public enum Align { Start, Center, End }
    public enum FitMode { FixedGap, VisibleStrip, FitToContainer } // ★ 추가

    public Orientation orientation = Orientation.Horizontal;

    [Header("FixedGap 모드")]
    public float gap = -24f; // 음수면 겹침

    [Header("VisibleStrip 모드")]
    public bool useVisibleStrip = false; // (안 쓰면 FixedGap으로 동작)
    public float visibleStrip = 36f;     // 보이게 남길 폭/높이(px)

    [Header("공통 옵션")]
    public Align align = Align.Center;
    public bool reverse = false;
    public float startOffset = 0f; // 세로일 때 살짝 아래로 내리고 싶을 때 등

    [Header("컨테이너에 딱 맞추기")]
    public FitMode fitMode = FitMode.FixedGap; // ★ FitToContainer 로 설정

    void OnTransformChildrenChanged() => Layout();
    void OnValidate() => Layout();

    public void Layout()
    {
        var self = (RectTransform)transform;
        int n = transform.childCount;
        if (n == 0) return;

        // 각 카드의 축-사이즈(가로면 width, 세로면 height)
        float[] size = new float[n];
        for (int i = 0; i < n; i++)
        {
            var rt = transform.GetChild(i) as RectTransform;
            size[i] = orientation == Orientation.Horizontal ? rt.sizeDelta.x : rt.sizeDelta.y;
        }

        // 각 카드 사이의 '이동량(step)' 계산
        float[] step = new float[Mathf.Max(n - 1, 1)];
        if (fitMode == FitMode.FitToContainer)
        {
            float containerLen = orientation == Orientation.Horizontal ? self.rect.width : self.rect.height;
            float sumSize = 0f; for (int i = 0; i < n; i++) sumSize += size[i];
            float s = (n > 1) ? (containerLen - sumSize) / (n - 1) : 0f; // 음수면 겹침
            for (int i = 0; i < step.Length; i++) step[i] = s;
        }
        else if (useVisibleStrip) // VisibleStrip 모드
        {
            for (int i = 0; i < step.Length; i++)
                step[i] = -(size[Mathf.Min(i, n - 1)] - visibleStrip);
        }
        else // FixedGap 모드
        {
            for (int i = 0; i < step.Length; i++)
                step[i] = gap;
        }

        // 총 길이
        float total = 0f;
        for (int i = 0; i < n; i++) total += size[i];
        for (int i = 0; i < step.Length; i++) total += step[i];

        // 시작 커서(왼쪽/아래 가장자리)
        float containerLenAxis = orientation == Orientation.Horizontal ? self.rect.width : self.rect.height;
        float leftEdge = -containerLenAxis * 0.5f;                // 컨테이너의 왼/아래 경계
        float cursor;

        if (fitMode == FitMode.FitToContainer)
            cursor = leftEdge;                                    // ★ 경계에 정확히 붙여 시작
        else
        {
            switch (align)
            {
                case Align.Start:  cursor = -total * 0.5f; break;
                case Align.Center: cursor = -total * 0.5f; break; // Center도 총길이를 기준으로 중앙 배치
                case Align.End:    cursor = -total * 0.5f; break;
                default:           cursor = -total * 0.5f; break;
            }
            // Align은 총길이를 기준으로 중앙(-total/2)에서 시작 → 위에서 center만 썼지만
            // 필요하면 Start/End 케이스를 원하는 방식으로 바꿔도 됩니다.
        }

        // 배치
        for (int i = 0; i < n; i++)
        {
            int idx = reverse ? (n - 1 - i) : i;
            var rt = transform.GetChild(idx) as RectTransform;

            float centerPos = cursor + size[idx] * 0.5f;

            if (orientation == Orientation.Horizontal)
                rt.anchoredPosition = new Vector2(centerPos, startOffset);
            else
                rt.anchoredPosition = new Vector2(startOffset, centerPos);

            rt.localEulerAngles = Vector3.zero;

            if (i < n - 1) cursor += size[idx] + step[i];
        }
    }
}
