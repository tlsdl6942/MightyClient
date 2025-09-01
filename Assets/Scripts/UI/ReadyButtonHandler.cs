using TMPro;
using UnityEngine;

public class ReadyButtonHandler : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private TextMeshProUGUI buttonText;

    private bool isReady = false;

    public void OnClickReady()
    {
        isReady = !isReady;
        if (buttonText != null)
            buttonText.text = isReady ? "준비 취소" : "준비하기";

        uiManager?.SetStatus(isReady ? "상태: 준비 완료" : "상태: 대기");
        uiManager?.Log(isReady ? "[You] 준비 완료" : "[You] 준비 취소");
    }
}
