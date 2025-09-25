using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CardUIManager cardUI;
    public PledgePopupManager pledgePopupManager;
    public int myPlayerNumber = 1;

    private List<int> deck = new List<int>();
    public List<List<int>> playerHands = new List<List<int>>();
    public List<Sprite> cardSprites = new List<Sprite>();
    private string[] suits = { "S", "D", "H", "C" };
    private string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

    void Start()
    {
        LoadCardSprites();
    }

    public void StartGame()
    {
        InitializeDeck();
        ShuffleDeck();
        DealCards();
        SortPlayerHands();
        PrintPlayerHands();
        cardUI.StartCardAnimation(playerHands, myPlayerNumber);
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
                if (sprite != null) cardSprites.Add(sprite);
            }
        }

        Sprite joker = Resources.Load<Sprite>("Cards/Joker");
        if (joker != null) cardSprites.Add(joker);
    }

    void InitializeDeck()
    {
        deck.Clear();
        for (int i = 0; i < cardSprites.Count; i++)
        {
            deck.Add(i);
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int r = Random.Range(0, deck.Count);
            (deck[i], deck[r]) = (deck[r], deck[i]);
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
    }

    void SortPlayerHands()
    {
        Dictionary<string, int> suitOrder = new Dictionary<string, int>
    {
        { "S", 0 },
        { "D", 1 },
        { "H", 2 },
        { "C", 3 }
    };

        Dictionary<string, int> rankOrder = new Dictionary<string, int>
    {
        { "2", 0 },
        { "3", 1 },
        { "4", 2 },
        { "5", 3 },
        { "6", 4 },
        { "7", 5 },
        { "8", 6 },
        { "9", 7 },
        { "10", 8 },
        { "J", 9 },
        { "Q", 10 },
        { "K", 11 },
        { "A", 12 }
    };

        for (int i = 0; i < playerHands.Count; i++)
        {
            playerHands[i].Sort((a, b) =>
            {
                string nameA = cardSprites[a].name;
                string nameB = cardSprites[b].name;

                // Joker 예외 처리
                if (nameA == "Joker") return 1;
                if (nameB == "Joker") return -1;

                string rankA = nameA.Substring(0, nameA.Length - 1);
                string suitA = nameA.Substring(nameA.Length - 1);

                string rankB = nameB.Substring(0, nameB.Length - 1);
                string suitB = nameB.Substring(nameB.Length - 1);

                int suitCompare = suitOrder[suitA].CompareTo(suitOrder[suitB]);
                if (suitCompare != 0)
                    return suitCompare;

                return rankOrder[rankA].CompareTo(rankOrder[rankB]);
            });
        }
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
    public void OnCardDistributionComplete()
    {
        List<int> myHand = playerHands[myPlayerNumber - 1];
        Debug.Log("출마 공약 팝업 on");
        pledgePopupManager.ShowPopup(myHand, cardSprites);
    }
}