using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [Header("카드 데이터")]
    private string[] suits = { "S", "D", "H", "C" };
    private string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

    public List<Sprite> cardSprites = new List<Sprite>();
    public List<List<int>> playerHands = new List<List<int>>();
    private List<int> deck = new List<int>();
    private List<int> bufferCards = new List<int>();

    [Header("UI 설정")]
    public int myPlayerNumber = 1; // 서버에서 받은 내 번호 (1~5)
    public GameObject cardPrefab;
    public Sprite backSprite;
    public Transform[] playerHandAreas; // 내 기준 시계방향으로 배치된 5개 영역

    public Button startButton;
    public GameObject cardStackObject;      // 중앙 카드더미 이미지
    public GameObject flyingCardPrefab;     // 날아가는 카드 프리팹

    void Start()
    {
        LoadCardSprites();
        //InitializeDeck();
        //ShuffleDeck();
        //DealCards();
        //ShowCards();
    }


    // 카드 스프라이트 불러오기
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

    // 시작 애니메이션


    public void OnStartButtonClicked()
    {
        Debug.Log("시작 버튼 눌림!");
        startButton.interactable = false;
        startButton.gameObject.SetActive(false);

        InitializeDeck();
        ShuffleDeck();
        DealCards();
        cardStackObject.SetActive(true); // 카드더미 보이기
        StartCoroutine(DistributeCardsAnimated());
    }
    IEnumerator DistributeCardsAnimated()
    {
        int totalPlayers = 5;
        List<int> displayOrder = GetDisplayOrder(myPlayerNumber);

        for (int round = 0; round < 10; round++)
        {
            for (int i = 0; i < totalPlayers; i++)
            {
                int actualPlayerNumber = displayOrder[i];
                int handIndex = actualPlayerNumber - 1;

                int cardId = playerHands[handIndex][round]; // 이미 분배된 카드 정보 사용

                yield return StartCoroutine(AnimateCardToPlayer(cardId, i, actualPlayerNumber == myPlayerNumber));
            }
        }

        cardStackObject.SetActive(false);
    }

    IEnumerator AnimateCardToPlayer(int cardId, int uiIndex, bool isLocalPlayer)
    {
        GameObject flyingCard = Instantiate(flyingCardPrefab, cardStackObject.transform.position, Quaternion.identity, cardStackObject.transform.parent);
        Image cardImage = flyingCard.GetComponent<Image>();
        cardImage.sprite = isLocalPlayer ? cardSprites[cardId] : backSprite;

        Vector3 targetPos = playerHandAreas[uiIndex].position;
        float duration = 0.04f;
        float elapsed = 0f;
        Vector3 startPos = flyingCard.transform.position;

        while (elapsed < duration)
        {
            flyingCard.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        flyingCard.transform.SetParent(playerHandAreas[uiIndex]);
        flyingCard.transform.localPosition = Vector3.zero;
    }


    // 카드 분배
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
        SortPlayerHands();
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
    void ShowCards()
    {
        List<int> displayOrder = GetDisplayOrder(myPlayerNumber);

        for (int i = 0; i < displayOrder.Count; i++)
        {
            int actualPlayerNumber = displayOrder[i];
            int deckIndex = actualPlayerNumber - 1;

            foreach (int cardIndex in playerHands[deckIndex])
            {
                GameObject cardGO = Instantiate(cardPrefab, playerHandAreas[i]);
                Image cardImage = cardGO.GetComponent<Image>();

                if (actualPlayerNumber == myPlayerNumber)
                {
                    cardImage.sprite = cardSprites[cardIndex]; // 내 카드 앞면
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
        { "A", 0 },
        { "2", 1 },
        { "3", 2 },
        { "4", 3 },
        { "5", 4 },
        { "6", 5 },
        { "7", 6 },
        { "8", 7 },
        { "9", 8 },
        { "10", 9 },
        { "J", 10 },
        { "Q", 11 },
        { "K", 12 }
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


}