using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mighty.Models;

public class TableBootstrap : MonoBehaviour
{
    [Header("Roots")]
    public SeatsCircleArranger seatsArranger;   // SeatsRoot
    public RectTransform myHandArea;            // MyHandArea
    public TrickAreaCircle trickArea;           // TrickArea

    [Header("Prefabs")]
    public GameObject seatPrefab;               // SeatsCircleArranger에도 같게
    public GameObject cardFrontPrefab;
    public GameObject cardBackSmallPrefab;

    [Header("Names (예시)")]
    public string myName = "나";
    public string[] otherNames = { "영삼", "태우", "대중", "두환" };

    private List<List<Card>> hands = new();
    private Deck deck;

    void Start()
    {
        // 1) 좌석 생성/배치
        seatsArranger.seatPrefab = seatPrefab;
        seatsArranger.playerCount = 5;
        seatsArranger.BuildSeatsIfNeeded();
        seatsArranger.Layout();

        // 2) 자리 이름/장수 표시(상대만)
        for (int i = 0; i < seatsArranger.seats.Count; i++)
        {
            var seatRT = seatsArranger.seats[i];
            var seatUI = seatRT.GetComponent<SeatUI>();
            if (!seatUI) continue;

            if (i == 0)
            {
                seatUI.SetName(myName);
                seatUI.SetCount(0);
                seatUI.countText.gameObject.SetActive(false);
                seatUI.backImage.gameObject.SetActive(false);
            }
            else
            {
                int nameIndex = i - 1;
                seatUI.SetName(otherNames[nameIndex % otherNames.Length]);
                seatUI.SetCount(10); // 데모로 10장
                seatUI.countText.gameObject.SetActive(true);
                seatUI.backImage.gameObject.SetActive(true);
            }
        }

        // 3) 덱 만들고 5인에게 10장씩 분배(데모)
        deck = new Deck();
        for (int p = 0; p < 5; p++)
            hands.Add(deck.Draw(10));

        // 4) 내 손패 앞면으로 MyHandArea에 깔기(겹쳐 배치)
        foreach (var c in hands[0])
        {
            var go = Instantiate(cardFrontPrefab, myHandArea);
            var label = go.GetComponentInChildren<TextMeshProUGUI>();
            if (label) label.text = c.ToString();
        }
        var overlap = myHandArea.GetComponent<MyHandOverlapLayout>();
        if (overlap) overlap.Layout();

        // 5) 샘플: 각 자리에서 카드 1장씩 중앙에 내보기
        for (int p = 0; p < 5; p++)
        {
            var card = hands[p][0];
            // 내 카드는 내 손에서 첫 장을 찾아 중앙으로 이동
            if (p == 0)
            {
                var first = myHandArea.GetChild(0) as RectTransform;
                trickArea.PlaceCardAt(0, 5, first);
            }
            else
            {
                // 상대 카드는 앞면 프리팹 하나 생성해서 중앙에 배치 (실전에선 서버에서 받은 카드 정보로)
                var go = Instantiate(cardFrontPrefab).transform as RectTransform;
                var label = go.GetComponentInChildren<TextMeshProUGUI>();
                if (label) label.text = card.ToString();
                trickArea.PlaceCardAt(p, 5, go);
            }
        }
    }
}
