using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlusButton : MonoBehaviour
{
    public ButtonType buttonType;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("Button ADD_"+buttonType+" has been clicked!");
        gameObject.GetComponentInParent<LevelUpOption>().SkillLevelUp(buttonType);
    }
}
