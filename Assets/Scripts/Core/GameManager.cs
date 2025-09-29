using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

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

    public List<CandidateInfo> candidates = new List<CandidateInfo>();
    public List<int> playerOrder;
    int currentPlayerIndex = 0;
    public int currentHighestPledge = 12; // pledge popup manager에서 +1 하기 때문에 결국 초깃값은 13이 됨.
    public bool isFirstRound = true; // 1차 라운드 여부 추적

    public int rulingPartyLeader = -1; // 여당 대표 플레이어 번호 저장용
    public string rulingSuit = ""; // 기루다
    public int rulingPledge = -1; // 공약 카드 수
    public bool isLeaderFinalized = false; // 대표 확정 여부 플래그

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

    //void DealCards()
    //{
    //    playerHands.Clear();
    //    for (int i = 0; i < 5; i++)
    //    {
    //        List<int> hand = new List<int>();
    //        for (int j = 0; j < 10; j++)
    //        {
    //            hand.Add(deck[0]);
    //            deck.RemoveAt(0);
    //        }
    //        playerHands.Add(hand);
    //    }
    //}
    void DealCards()
    {
        playerHands.Clear();

        // 인덱스 0은 비워두고, 1~5번 플레이어용 리스트 생성
        playerHands.Add(new List<int>()); // dummy for index 0

        for (int i = 1; i <= 5; i++)
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

    public void Sort(List<int> hand)
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

        hand.Sort((a, b) =>
        {
            string nameA = cardSprites[a].name;
            string nameB = cardSprites[b].name;

            if (nameA == "Joker") return 1;
            if (nameB == "Joker") return -1;

            string rankA = nameA.Substring(0, nameA.Length - 1);
            string suitA = nameA.Substring(nameA.Length - 1);

            string rankB = nameB.Substring(0, nameB.Length - 1);
            string suitB = nameB.Substring(nameB.Length - 1);

            int suitCompare = suitOrder[suitA].CompareTo(suitOrder[suitB]);
            if (suitCompare != 0) return suitCompare;

            return rankOrder[rankA].CompareTo(rankOrder[rankB]);
        });

    }

    void SortPlayerHands()
    {
        for (int i = 1; i <= 5; i++) // 1~5번 플레이어
        {
            Sort(playerHands[i]);
        }
    }

    void PrintPlayerHands()
    {
        for (int i = 1; i <= 5; i++)
        {
            string handStr = $"Player {i}: ";
            foreach (int cardIndex in playerHands[i])
            {
                handStr += GetCardName(cardIndex) + " ";
            }
            Debug.Log(handStr);
        }
    }

    void PrintDeck()
    {
        string deckStr = $"덱: ";
        foreach (int cardIndex in deck)
        {
            deckStr += GetCardName(cardIndex) + " ";
        }
        Debug.Log(deckStr);
    }

    string GetCardName(int index)
    {
        return cardSprites[index].name; // 예: "2D", "KC", "Joker"
    }

    // 출마 공약 팝업 띄우기

    public void StartPledgeRound()
    {
        currentPlayerIndex = 0;

        // 1차 라운드
        if (candidates.Count == 0)
        {
            isFirstRound = true;
            playerOrder = new List<int> { 1, 2, 3, 4, 5 };
        }
        else
        {
            isFirstRound = false;
            // 이후 라운드는 출마자만 대상으로 순서 재정의
            playerOrder = GetOrderFromCandidates();
        }

        StartCoroutine(ShowPledgeSequentially(playerOrder));
    }

    IEnumerator ShowPledgeSequentially(List<int> playerOrder)
    {
        while (currentPlayerIndex < playerOrder.Count)
        {
            // 대표가 확정되었으면 코루틴 종료
            if (isLeaderFinalized)
            {
                yield break;
            }

            int playerNum = playerOrder[currentPlayerIndex];

            // 팝업 띄우기
            yield return StartCoroutine(ShowPledgePopupForPlayer(playerNum));

            currentPlayerIndex++;
        }

        if (!isLeaderFinalized)
            EvaluateCandidates();
    }

    IEnumerator ShowPledgePopupForPlayer(int playerNum)
    {
        // 팝업 상태 초기화
        pledgePopupManager.isPopupClosed = false;

        // 팝업 띄우기
        pledgePopupManager.ShowPopupForPlayer(playerNum);

        // 사용자가 버튼을 누를 때까지 기다림
        while (!pledgePopupManager.isPopupClosed)
        {
            yield return null;
        }
    }


    List<int> GetOrderFromCandidates()
    {
        return candidates.Select(c => c.playerNumber).OrderBy(n => n).ToList();
    }


    public void AddOrUpdateCandidate(int playerNumber, string suit, int pledgeCount)
    {
        RemoveCandidate(playerNumber);
        candidates.Add(new CandidateInfo(playerNumber, suit, pledgeCount));
    }

    public void RemoveCandidate(int playerNumber)
    {
        candidates.RemoveAll(c => c.playerNumber == playerNumber);
    }

    public void EvaluateCandidates()
    {
        int count = candidates.Count;

        if (count == 0)
        {
            Debug.Log("딜 미스! 게임 리셋");
            // 딜 미스 함수 짜기
            // RestartGame();
        }
        else if (count == 1)
        {
            FinalizeLeader(candidates[0].playerNumber);
        }
        else
        {
            StartPledgeRound();
        }
    }

    public void FinalizeLeader(int playerNum)
    {
        rulingPartyLeader = playerNum;

        // 후보 정보에서 문양과 공약 수치 가져오기
        CandidateInfo info = candidates.Find(c => c.playerNumber == playerNum);
        if (info != null)
        {
            rulingSuit = info.suit;
            rulingPledge = info.pledgeCount;
        }

        isLeaderFinalized = true;

        Debug.Log($"여당 대표 확정! Player: {playerNum}, 기루다: {rulingSuit}, 카드 수: {rulingPledge}");

        // 이후 게임 진행 로직 연결
        List<int> extraCards = GetExtraCardsForLeader();
        cardUI.DistributeExtraCardsToLeader(rulingPartyLeader, extraCards);
        // 예: 정책 제시, 야당 등장, UI 업데이트 등

    }
    public List<int> GetExtraCardsForLeader()
    {
        List<int> extraCards = deck.Take(3).ToList();
        PrintDeck();
        deck.RemoveRange(0, 3);
        PrintDeck();
        return extraCards;
    }
}