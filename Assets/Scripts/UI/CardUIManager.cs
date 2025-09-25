using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUIManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite backSprite;
    public Transform[] playerHandAreas;
    public GameObject cardStackObject;
    public GameObject flyingCardPrefab;
    public Button startButton;
    public GameManager gameManager;

    public void OnStartButtonClicked()
    {
        startButton.interactable = false;
        startButton.gameObject.SetActive(false);
        cardStackObject.SetActive(true);
        gameManager.StartGame();
    }

    public void StartCardAnimation(List<List<int>> playerHands, int myPlayerNumber)
    {
        StartCoroutine(DistributeCardsAnimated(playerHands, myPlayerNumber));
    }

    IEnumerator DistributeCardsAnimated(List<List<int>> playerHands, int myPlayerNumber)
    {
        int totalPlayers = 5;
        List<int> displayOrder = GetDisplayOrder(myPlayerNumber);

        for (int round = 0; round < 10; round++)
        {
            for (int i = 0; i < totalPlayers; i++)
            {
                int actualPlayerNumber = displayOrder[i];
                int handIndex = actualPlayerNumber - 1;
                int cardId = playerHands[handIndex][round];

                yield return StartCoroutine(AnimateCardToPlayer(cardId, i, actualPlayerNumber == myPlayerNumber));
            }
        }
        cardStackObject.SetActive(false);

        // 애니메이션 끝났다고 GameManager에게 알림
        cardStackObject.SetActive(false);

        if (gameManager != null)
        {
            Debug.Log("GameManager 연결됨, 애니메이션 완료 후 호출");
            gameManager.OnCardDistributionComplete();
        }
        else
        {
            Debug.LogError("GameManager가 연결되지 않았습니다!");
        }

    }

    IEnumerator AnimateCardToPlayer(int cardId, int uiIndex, bool isLocalPlayer)
    {
        GameObject flyingCard = Instantiate(flyingCardPrefab, cardStackObject.transform.position, Quaternion.identity, cardStackObject.transform.parent);
        Image cardImage = flyingCard.GetComponent<Image>();
        Sprite cardSprite = isLocalPlayer ? gameManager.cardSprites[cardId] : backSprite;
        cardImage.sprite = cardSprite;

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