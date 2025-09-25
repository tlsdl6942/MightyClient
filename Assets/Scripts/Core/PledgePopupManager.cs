using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PledgePopupManager : MonoBehaviour
{
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

    public void ShowPopup(List<int> myHand, List<Sprite> cardSprites)
    {
        popupPanel.SetActive(true);

        selectedSuit = GetMostCommonSuit(myHand, cardSprites);
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



        pledgeSlider.minValue = 13;
        pledgeSlider.maxValue = 20;
        pledgeSlider.value = 13;
        pledgeValueText.text = "13";

        pledgeSlider.onValueChanged.AddListener((value) =>
        {
            pledgeValueText.text = value.ToString();
        });

        runButton.onClick.RemoveAllListeners();
        runButton.onClick.AddListener(() =>
        {
            //string selectedSuit = GetSelectedSuit();
            int pledgeCount = (int)pledgeSlider.value;
            Debug.Log($"출마! 문양: {selectedSuit}, 공약: {pledgeCount}장");
            popupPanel.SetActive(false);
        });

        giveUpButton.onClick.RemoveAllListeners();
        giveUpButton.onClick.AddListener(() =>
        {
            Debug.Log("포기!");
            popupPanel.SetActive(false);
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