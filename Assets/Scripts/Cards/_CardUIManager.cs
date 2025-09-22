using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUIManager : MonoBehaviour
{
    [Header("데이터 연결")]
    public CardProvider cardProvider; // 카드 분배 및 스프라이트 제공자
    public int myPlayerNumber = 1; // 서버에서 받은 내 플레이어 번호 (1~5)

    [Header("UI 연결")]
    public GameObject cardPrefab; // 카드 프리팹 (Image 컴포넌트 포함)
    public Sprite backSprite; // 카드 뒷면 이미지
    public Transform[] playerHandAreas; // 카드가 배치될 5개의 영역 (내 위치 기준 시계방향)

    void Start()
    {
        ShowCards();
    }

    void ShowCards()
    {
        List<int> displayOrder = GetDisplayOrder(myPlayerNumber);

        for (int i = 0; i < displayOrder.Count; i++)
        {
            int actualPlayerNumber = displayOrder[i];
            int deckIndex = actualPlayerNumber - 1;

            foreach (int cardIndex in cardProvider.PlayerHands[deckIndex])
            {
                GameObject cardGO = Instantiate(cardPrefab, playerHandAreas[i]);
                Image cardImage = cardGO.GetComponent<Image>();

                if (actualPlayerNumber == myPlayerNumber)
                {
                    cardImage.sprite = cardProvider.cardSprites[cardIndex]; // 내 카드 앞면
                }
                else
                {
                    cardImage.sprite = backSprite; // 다른 플레이어 카드 뒷면
                }
            }
        }
    }

    List<int> GetDisplayOrder(int myNumber)
    {
        List<int> order = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            int num = ((myNumber - 1 + i) % 5) + 1;
            order.Add(num);
        }
        return order;
    }
}