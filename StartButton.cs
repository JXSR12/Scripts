using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    // Start is called before the first frame update
    public SceneLoader sceneLoader;
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        GameObject.Find("StartSFX").GetComponent<AudioSource>().Play(0);
        sceneLoader.LoadScene("Scenes/GameScene", "Settling into the forest");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
