using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI logText;

    public void SetStatus(string message)
    {
        if (statusText != null) statusText.text = message;
    }

    public void Log(string line)
    {
        if (logText != null)
        {
            logText.text = (logText.text.Length == 0) ? line : (logText.text + "\n" + line);
        }
    }
}
