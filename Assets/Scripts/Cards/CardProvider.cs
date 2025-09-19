using System.Collections.Generic;
using UnityEngine;

public class CardProvider: MonoBehaviour
{
    public List<Sprite> cardSprites = new List<Sprite>();
    private string[] suits = { "S", "D", "H", "C" }; // 문양
    private string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" }; // 숫자

    private List<int> deck = new List<int>();

    private List<List<int>> playerHands = new List<List<int>>();
    public List<List<int>> PlayerHands => playerHands;

    private List<int> bufferCards = new List<int>();

    void Start()
    {
        LoadCardSprites();
        InitializeDeck();
        ShuffleDeck();
        DealCards();
    }

    void LoadCardSprites()
    {
        cardSprites.Clear();

        foreach (string rank in ranks)
        {
            foreach (string suit in suits)
            {
                string cardName = $"{rank}{suit}";
                Sprite sprite = Resources.Load<Sprite>($"Cards/{cardName}");
                if (sprite != null)
                {
                    cardSprites.Add(sprite);
                }
                else
                {
                    Debug.LogWarning($"스프라이트 {cardName}를 찾을 수 없습니다.");
                }
            }
        }

        Sprite jokerSprite = Resources.Load<Sprite>("Cards/Joker");
        if (jokerSprite != null)
        {
            cardSprites.Add(jokerSprite);
        }
        else
        {
            Debug.LogWarning("조커 스프라이트를 찾을 수 없습니다.");
        }
    }

    void InitializeDeck()
    {
        deck.Clear();
        for (int i = 0; i < cardSprites.Count; i++)
        {
            deck.Add(i); // 카드 인덱스 저장
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(0, deck.Count);
            int temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void DealCards()
    {
        playerHands.Clear();

        for (int i = 0; i < 5; i++)
        {
            List<int> hand = new List<int>();
            for (int j = 0; j < 10; j++)
            {
                hand.Add(deck[0]);
                deck.RemoveAt(0);
            }
            playerHands.Add(hand);
        }

        bufferCards = new List<int>(deck); // 남은 3장

        PrintPlayerHands();
    }

    void PrintPlayerHands()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            string handStr = $"Player {i + 1}: ";
            foreach (int cardIndex in playerHands[i])
            {
                handStr += GetCardName(cardIndex) + " ";
            }
            Debug.Log(handStr);
        }
    }

    string GetCardName(int index)
    {
        return cardSprites[index].name; // 예: "2D", "KC", "Joker"
    }
}
