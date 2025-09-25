using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PledgePopupManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject popupPanel;
    public Image suitIcon;
    public Slider pledgeSlider;
    public Text pledgeValueText;
    public Button runButton;
    public Button giveUpButton;

    [Header("문양 선택 토글")]
    public Toggle spadeToggle;
    public Toggle heartToggle;
    public Toggle diamondToggle;
    public Toggle clubToggle;

    [Header("문양 아이콘")]
    public Sprite spadeSprite;
    public Sprite heartSprite;
    public Sprite diamondSprite;
    public Sprite clubSprite;

    public void ShowPopup(List<int> myHand, List<Sprite> cardSprites)
    {
        popupPanel.SetActive(true);

        string defaultSuit = GetMostCommonSuit(myHand, cardSprites);
        SelectDefaultSuit(defaultSuit);

        pledgeSlider.minValue = 13;
        pledgeSlider.maxValue = myHand.Count;
        pledgeSlider.value = 13;
        pledgeValueText.text = "13";

        pledgeSlider.onValueChanged.AddListener((value) =>
        {
            pledgeValueText.text = value.ToString();
        });

        runButton.onClick.RemoveAllListeners();
        runButton.onClick.AddListener(() =>
        {
            string selectedSuit = GetSelectedSuit();
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

    void SelectDefaultSuit(string suit)
    {
        switch (suit)
        {
            case "S": spadeToggle.isOn = true; break;
            case "H": heartToggle.isOn = true; break;
            case "D": diamondToggle.isOn = true; break;
            case "C": clubToggle.isOn = true; break;
        }
        suitIcon.sprite = GetSuitSprite(suit);
    }

    string GetSelectedSuit()
    {
        if (spadeToggle.isOn) return "S";
        if (heartToggle.isOn) return "H";
        if (diamondToggle.isOn) return "D";
        if (clubToggle.isOn) return "C";
        return "S";
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
}