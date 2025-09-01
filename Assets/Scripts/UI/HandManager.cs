using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mighty.Models;

public class HandManager : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject cardPrefab;  // Assets/Prefabs/CardPrefab
    public Transform handPanel;    // Hierarchy의 HandPanel

    void Start()
    {
        var deck = new Deck();
        List<Card> myHand = deck.Draw(5);

        foreach (var card in myHand)
        {
            GameObject obj = Instantiate(cardPrefab, handPanel);
            var label = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = card.ToString();
        }
    }
}
