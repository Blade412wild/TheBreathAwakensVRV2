using TMPro;
using UnityEngine;

public class SCBASmartWatch : MonoBehaviour
{
    [Header("Percentage")]
    [SerializeField] private TextMeshProUGUI oxygonAmountPercecntageText;

    [Header("Estimation")]
    [SerializeField] private TextMeshProUGUI oxygonEstimation;
    [SerializeField] private AudioClip oxygonEstimationClip;

    [Header("Extraction")]
    [SerializeField] private TextMeshProUGUI Extraction;
    [SerializeField] private AudioClip extractionClip;


    [Header("Refs")]
    [SerializeField] private AudioSource audioSource;




    public void UpdateOxygonPercentageUI(float amount, float percentage)
    {
        oxygonAmountPercecntageText.text = percentage.ToString() + " %";
    }

    public void UpdateOxygonEstimation(TimeLeftStruct value)
    {

        oxygonEstimation.text = SetTime(value);
    }

    public void UpdateExtraction(TimeLeftStruct value)
    {
        Extraction.text = SetTime(value);
    }

    private string SetTime(TimeLeftStruct value)
    {
        string minutes = "0";

        if (value.Minutes < 10)
        {
            minutes += value.Minutes;
        }
        else
        {
            minutes = value.Minutes.ToString();
        }

        string seconds = "0";

        if (value.Seconds < 10)
        {
            seconds += value.Seconds;
        }
        else
        {
            seconds = value.Seconds.ToString();
        }

        return minutes + ":" + seconds;
    }

}


