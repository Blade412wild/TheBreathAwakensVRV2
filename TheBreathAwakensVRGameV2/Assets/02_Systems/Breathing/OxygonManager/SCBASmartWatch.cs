using TMPro;
using UnityEngine;

public class SCBASmartWatch : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI oxygonAmountText;
    [SerializeField] private TextMeshProUGUI oxygonAmountPercecntageText;




    public void UpdateOxygonUI(float amount, float percentage)
    {
        oxygonAmountText.text = amount.ToString() + " (02)";
        oxygonAmountPercecntageText.text = percentage.ToString() + " %";
    }

    public void UpdateTimerUI()
    {

    }
}


