using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XpBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public TextMeshProUGUI text;
    public TextMeshProUGUI levelText;

    public void ResetXp(int xp)
    {
        slider.maxValue = xp;
        slider.value = 0;

        text.text = 0 + " / " + xp;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetLevel(int level)
    {
        levelText.text = "Level " + level;
    }

    public void SetXp(int xp, int maxXp)
    {
        slider.value = xp;
        text.text = xp + " / " + maxXp;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
