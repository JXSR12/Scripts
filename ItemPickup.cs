using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private string itemName;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI itemNameText;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup.alpha = 0;
        itemNameText.text = itemName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetItemName(string itemName)
    {
        this.itemName = itemName;
        itemNameText.text = itemName;
    }

    public void EnterItemShow()
    {
        canvasGroup.alpha = 1;
    }

    public void LeaveItemHide()
    {
        canvasGroup.alpha = 0;
    }
}
