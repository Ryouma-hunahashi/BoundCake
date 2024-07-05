using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_CoinControl : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (StatusManager.nowHitPoint < StatusManager.maxHitPoint)
        {
            text.text = "x " + StatusManager.coinCount.ToString();
        }
        else
        {
            text.text = "MAX";
        }
    }
}
