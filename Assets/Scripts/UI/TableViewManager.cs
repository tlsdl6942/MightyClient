using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mighty.Models;

public class TableViewManager : MonoBehaviour
{
    [Header("Slots")]
    public RectTransform slotBottom;
    public RectTransform slotLeftMid;
    public RectTransform slotLeftTop;
    public RectTransform slotRightTop;
    public RectTransform slotRightMid;

    [Header("Prefabs")]
    public GameObject cardFrontPrefab;
    public GameObject cardBackSmallPrefab;

    [Header("Seating")]
    public int playerCount = 5;
    public int myIndex = 0; // ★ 서버에서 받은 내 좌석 인덱스로 세팅

    // 데모용
    private List<List<Card>> hands = new();
    private Deck deck;

    RectTransform GetSlotByViewIndex(int v)
    {
        return v switch
        {
            0 => slotBottom,
            1 => slotLeftMid,
            2 => slotLeftTop,
            3 => slotRightTop,
            4 => slotRightMid,
            _ => slotBottom
        };
    }

    public int ToViewIndex(int playerIndex) =>
        (playerIndex - myIndex + playerCount) % playerCount;

    void Start()
    {
        // 데모: 5인에게 10장씩
        deck = new Deck();
        for (int p = 0; p < playerCount; p++)
            hands.Add(deck.Draw(10));

        for (int p = 0; p < playerCount; p++)
        {
            int v = ToViewIndex(p);
            var slot = GetSlotByViewIndex(v);

            if (v == 0) // 나: 앞면 가로 스트립
            {
                foreach (var c in hands[p])
                {
                    var go = Instantiate(cardFrontPrefab, slot);
                    var label = go.GetComponentInChildren<TextMeshProUGUI>();
                    if (label) label.text = c.ToString();
                }
            }
            else // 상대: 뒷면 스트립(장수만큼 겹침). 장수만 숫자로 보여주려면 아래 루프를 숫자 TMP 하나로 바꾸세요.
            {
                for (int i = 0; i < hands[p].Count; i++)
                    Instantiate(cardBackSmallPrefab, slot);
            }

            // 각 슬롯의 스트립 레이아웃 새로고침
            slot.GetComponent<HandStripLayout>()?.Layout();
        }
    }
}
