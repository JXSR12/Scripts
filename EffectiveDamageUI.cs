using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectiveDamageUI : MonoBehaviour
{
    public TextMeshProUGUI baseDamageText;
    public TextMeshProUGUI wpnDamageText;
    public TextMeshProUGUI effDamageText;

    // Update is called once per frame
    public void UpdateDamageAmount(int baseDamage, int wpnDamage)
    {
        baseDamageText.text = baseDamage.ToString();
        wpnDamageText.text = wpnDamage.ToString();
        effDamageText.text = (baseDamage+wpnDamage).ToString();
    }
}
