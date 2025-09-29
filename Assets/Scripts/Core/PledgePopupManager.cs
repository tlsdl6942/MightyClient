using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PledgePopupManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("UI 연결")]
    public GameObject popupPanel;
    public Image suitIcon;
    public Slider pledgeSlider;
    public TextMeshProUGUI pledgeValueText;
    public Button runButton;
    public Button giveUpButton;

    [Header("문양 선택 버튼")]
    public Button spadeButton;
    public Button heartButton;
    public Button diamondButton;
    public Button clubButton;

    [Header("문양 아이콘")]
    public Sprite spadeSprite;
    public Sprite heartSprite;
    public Sprite diamondSprite;
    public Sprite clubSprite;

    public string selectedSuit;

    public bool isPopupClosed = false;

    public void ShowPopupForPlayer(int playerNum)
    {
        Debug.Log($"[팝업] playerNum: {playerNum}");
        //Debug.Log($"[팝업] playerHands.Count: {gameManager.playerHands.Count}");


        isPopupClosed = false;
        popupPanel.SetActive(true);

        List<int> hand = gameManager.playerHands[playerNum];
        selectedSuit = GetMostCommonSuit(hand, gameManager.cardSprites);
        UpdateSuitIcon(selectedSuit);

        // 문양 버튼 이벤트 연결
        spadeButton.onClick.RemoveAllListeners();
        heartButton.onClick.RemoveAllListeners();
        diamondButton.onClick.RemoveAllListeners();
        clubButton.onClick.RemoveAllListeners();

        spadeButton.onClick.AddListener(() => OnSuitSelected("S"));
        heartButton.onClick.AddListener(() => OnSuitSelected("H"));
        diamondButton.onClick.AddListener(() => OnSuitSelected("D"));
        clubButton.onClick.AddListener(() => OnSuitSelected("C"));

        // 슬라이더 설정
        pledgeSlider.minValue = gameManager.currentHighestPledge + 1;
        pledgeSlider.maxValue = 20;
        pledgeSlider.value = pledgeSlider.minValue;
        pledgeValueText.text = pledgeSlider.value.ToString();

        pledgeSlider.onValueChanged.RemoveAllListeners();
        pledgeSlider.onValueChanged.AddListener((value) =>
        {
            pledgeValueText.text = value.ToString();
        });

        runButton.onClick.RemoveAllListeners();
        runButton.onClick.AddListener(() =>
        {
            int pledgeCount = (int)pledgeSlider.value;
            string suit = selectedSuit;

            if (pledgeCount <= gameManager.currentHighestPledge)
            {
                Debug.LogWarning("공약이 현재보다 낮습니다!");
                return;
            }

            gameManager.AddOrUpdateCandidate(playerNum, suit, pledgeCount);
            gameManager.currentHighestPledge = pledgeCount;

            if (pledgeCount == 20)
            {
                gameManager.FinalizeLeader(playerNum);
                // 팝업 닫고 코루틴 종료를 유도
                popupPanel.SetActive(false);
                isPopupClosed = true;
                return; // 이후 흐름 중단

            }

            popupPanel.SetActive(false);
            isPopupClosed = true;
        });

        giveUpButton.onClick.RemoveAllListeners();
        giveUpButton.onClick.AddListener(() =>
        {
            gameManager.RemoveCandidate(playerNum);
            Debug.Log($"Player {playerNum} 출마 포기!");

            // 포기 직후 출마자 수 확인
            if (!gameManager.isFirstRound && gameManager.candidates.Count == 1)
            {
                int lastPlayer = gameManager.candidates[0].playerNumber;
                gameManager.FinalizeLeader(lastPlayer);
            }

            popupPanel.SetActive(false);
            isPopupClosed = true;
        });
    }


    Sprite GetSuitSprite(string suit)
    {
        switch (suit)
        {
            case "S": return spadeSprite;
            case "H": return heartSprite;
            case "D": return diamondSprite;
            case "C": return clubSprite;
            default: return null;
        }
    }

    string GetMostCommonSuit(List<int> hand, List<Sprite> cardSprites)
    {
        Dictionary<string, int> suitCount = new Dictionary<string, int>
        {
            { "S", 0 }, { "D", 0 }, { "H", 0 }, { "C", 0 }
        };

        foreach (int cardIndex in hand)
        {
            string name = cardSprites[cardIndex].name;
            if (name == "Joker") continue;
            string suit = name.Substring(name.Length - 1);
            suitCount[suit]++;
        }

        string mostSuit = "S";
        int max = 0;
        foreach (var pair in suitCount)
        {
            if (pair.Value > max)
            {
                mostSuit = pair.Key;
                max = pair.Value;
            }
        }

        return mostSuit;
    }
    void OnSuitSelected(string suit)
    {
        selectedSuit = suit;
        UpdateSuitIcon(suit);
        Debug.Log($"문양 선택됨: {suit}");
    }

    void UpdateSuitIcon(string suit)
    {
        switch (suit)
        {
            case "S": suitIcon.sprite = spadeSprite; break;
            case "H": suitIcon.sprite = heartSprite; break;
            case "D": suitIcon.sprite = diamondSprite; break;
            case "C": suitIcon.sprite = clubSprite; break;
        }
    }

}