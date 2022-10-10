using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageWarning : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup.alpha = 0;
    }

    IEnumerator ShowTimedMessage(string message, int duration)
    {
        canvasGroup.alpha = 1;
        text.text = message;
        yield return new WaitForSeconds(duration);
        canvasGroup.alpha = 0;
    }

    public void ShowMessage(int seconds, string message)
    {
        StartCoroutine(ShowTimedMessage(message, seconds));
    }
}
