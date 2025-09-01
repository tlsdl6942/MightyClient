using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeatUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image backImage;
    public TextMeshProUGUI countText;

    public void SetName(string nick) { if (nameText) nameText.text = nick; }
    public void SetCount(int count) { if (countText) countText.text = "x" + count; }
}
