using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CardUIManager cardUI;
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
}