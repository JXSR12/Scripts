using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public TextMeshProUGUI text;

    public void ResetHealth(int hp)
    {
        slider.maxValue = hp;
        slider.value = hp;

        text.text = hp + " / " + hp;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int hp, int maxHp)
    {
        slider.value = hp;
        text.text = hp + " / " + maxHp;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
