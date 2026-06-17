using System;
using TMPro;
using UnityEngine;

public class SCBASmartWatch : MonoBehaviour
{
    public event Action EquipEvent;
    public event Action UnequipEvent;

    [Header("Control")]
    [SerializeField] private bool equip;
    [SerializeField] private bool unequip;

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
    [SerializeField] private BreathingFeedbackFeature breathingFeedbackFeature;
    //[SerializeField] private DotTrailBehaviour dotTrailBehaviour;



    private void Update()
    {
        if (equip)
        {
            EquipSmartWatch();
            equip = false; 
        }

        if (unequip)
        {
            unequip = false;
            UnequipSmartWatch();
        }


    }

    public void Init()
    {

    }

    public void UpdateOxygonPercentageUI(float amount, float percentage)
    {
        //oxygonAmountPercecntageText.text = percentage.ToString() + " %";
    }

    public void UpdateOxygonEstimation(TimeLeftStruct value)
    {
        Debug.Log("update estimation : " + value);
        oxygonEstimation.text = TimeLeftConversions.ConvertTimeToDigitalClock(value);
    }

    public void UpdateExtraction(TimeLeftStruct value)
    {
        Extraction.text = TimeLeftConversions.ConvertTimeToDigitalClock(value);
    }

    private void EquipSmartWatch()
    {

        EquipEvent.Invoke();
    }

    private void UnequipSmartWatch()
    {
        UnequipEvent.Invoke();

    }



}

public class GasMaskSoundFeature : MonoBehaviour
{

}


