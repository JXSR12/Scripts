using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelUpOption : MonoBehaviour
{
    private Player player;

    public CanvasGroup canvasGroup;

    public TextMeshProUGUI title;
    public TextMeshProUGUI agility;
    public TextMeshProUGUI strength;
    public TextMeshProUGUI power;

    private bool anyChange = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Character").GetComponent<Player>();
        canvasGroup.alpha = 0;
        anyChange = true;
    }

    public void LevelUpShow()
    {
        anyChange = true;
        canvasGroup.alpha = 1;
        GameObject.Find("GameManager").GetComponent<CursorManager>().UnlockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        if (anyChange)
        {
            title.text = "Skill Points : " + player.GetLevelPoints();
            agility.text = "AGI      " + player.GetAgilityLevel();
            strength.text = "STR      " + player.GetStrengthLevel();
            power.text = "POW      " + player.GetPowerLevel();
            anyChange = false;
        }
    }

    public void SkillLevelUp(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.AGILITY:
                Debug.Log("Button Agility pressed!");
                if (player.LevelPointSpend(1))
                {
                    anyChange = true;
                }
                else
                {
                    anyChange = true;
                    canvasGroup.alpha = 0;
                    GameObject.Find("GameManager").GetComponent<CursorManager>().LockCursor();
                }
                break;
            case ButtonType.STRENGTH:
                Debug.Log("Button Strength pressed!");
                if (player.LevelPointSpend(2))
                {
                    anyChange = true;
                }
                else
                {
                    anyChange = true;
                    canvasGroup.alpha = 0;
                    GameObject.Find("GameManager").GetComponent<CursorManager>().LockCursor();
                }
                break;
            case ButtonType.POWER:
                Debug.Log("Button Power pressed!");
                if (player.LevelPointSpend(3))
                {
                    anyChange = true;
                }
                else
                {
                    anyChange = true;
                    canvasGroup.alpha = 0;
                    GameObject.Find("GameManager").GetComponent<CursorManager>().LockCursor();
                }
                break;
        }
    }
}
