using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathOverlay : MonoBehaviour
{
    private Button mainMenuButton;
    private Button quitButton;
    public SceneLoader sceneLoader;

    private bool showAnim = false;

    public CanvasGroup canvasGroup;
    public CanvasGroup onShowHiddenCanvasGroup;

    public bool isEnabled;
    
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup.alpha = 0;
        mainMenuButton = GameObject.Find("Main Menu").GetComponent<Button>();
        quitButton = GameObject.Find("Quit").GetComponent<Button>();
        mainMenuButton.onClick.AddListener(MainMenu);
        quitButton.onClick.AddListener(QuitApp);
        isEnabled = false;
    }

    void MainMenu()
    {
        if(isEnabled)
        {
            sceneLoader.LoadScene("Scenes/SampleScene", "Returning to main menu");
        }
    }

    void QuitApp()
    {
        if(isEnabled)
        {
            Debug.Log("Button clicked: QUIT APP");
            Application.Quit();
        }
        
    }

    public void OnDeathShow()
    {
        showAnim = true;
        onShowHiddenCanvasGroup.alpha = 0;
        isEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (showAnim)
        {
            if (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += 0.1f;
            }
        }
    }
    
    
}
