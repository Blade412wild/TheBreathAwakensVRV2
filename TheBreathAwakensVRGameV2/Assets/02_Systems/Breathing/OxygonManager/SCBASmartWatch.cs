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



    public void Init()
    {

    }

    public void UpdateOxygonPercentageUI(float amount, float percentage)
    {
        //oxygonAmountPercecntageText.text = percentage.ToString() + " %";
    }

    public void UpdateOxygonEstimation(TimeLeftStruct value)
    {

        oxygonEstimation.text = TimeLeftConversions.ConvertTimeToDigitalClock(value);
    }

    public void UpdateExtraction(TimeLeftStruct value)
    {
        Extraction.text = TimeLeftConversions.ConvertTimeToDigitalClock(value);
    }



}


