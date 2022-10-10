using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public CanvasGroup mainMenuCanvasGroup;
    public Slider progressBar;
    public TextMeshProUGUI loadingText;

    void Start()
    {
        canvasGroup.alpha = 0;
        mainMenuCanvasGroup.alpha = 1;
    }

    public void LoadScene(string sceneName, string loadText)
    {
        StartCoroutine(LoadSceneAsync(sceneName, loadText));
    }

    IEnumerator LoadSceneAsync(string sceneName, string loadText)
    {
        canvasGroup.alpha = 1;
        mainMenuCanvasGroup.alpha = 0;
        AsyncOperation ops = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!ops.isDone)
        {
            progressBar.value = ops.progress;
            loadingText.text = loadText + " .. " + (int)((ops.progress/0.9f) * 100) + "%";
            yield return null;
        }
    }
}
