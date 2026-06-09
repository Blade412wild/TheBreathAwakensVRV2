using System.Drawing.Text;
using TMPro;
using UnityEngine;

public class BreathingFeedbackFeature : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] private bool useFeature;

    [Header("Visuals")]
    [SerializeField] private float highestVisualPeak;
    [SerializeField] private float lowestVisualPeak;
    [SerializeField] private float visualZeroLine;

    [Header("Sensor")]
    [SerializeField] private float highestSensorDataPeak;
    [SerializeField] private float lowestSensorDataPeak;

    [Header("References")]
    [SerializeField] private RectTransform dot;
    [SerializeField] private BreathingDeviceData dataContainer;
    [SerializeField] private MessageFinishedReceived messageFinishedReceived;


    private Vector2 dotpos;
    private Vector2 dotPosTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        messageFinishedReceived.OnDataReceivedEvent += UpdateDotPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (!useFeature) return;
    }

    private void OnDisable()
    {
        messageFinishedReceived.OnDataReceivedEvent -= UpdateDotPos;
    }

    private void UpdateDotPos()
    {
        GetTargetPos(dataContainer.BreathingState, dataContainer.inExhaleSpeed);
        dot.localPosition = dotPosTarget;

    }



    private void GetTargetPos(BreathingState currentState, float speed)
    {
        float yPosTarget = 0;

        if (currentState == BreathingState.inhaling)
        {
            yPosTarget = Mapping.MapValue(speed, lowestSensorDataPeak, highestSensorDataPeak, lowestVisualPeak, highestVisualPeak);
            dotPosTarget.y = yPosTarget;

        }
        else if (currentState == BreathingState.exhaling)
        {
            yPosTarget = Mapping.MapValue(speed, lowestSensorDataPeak, highestSensorDataPeak, lowestVisualPeak, highestVisualPeak);
            yPosTarget *= -1;
            dotPosTarget.y = yPosTarget;

        }
        else
        {
            speed = 0;
            yPosTarget = speed;
            dotPosTarget.y = yPosTarget;
        }

        //Debug.Log("rawSpeed = " + speed + " m/s | targetPos : " + dotPosTarget + " m "/* + " | OxygonUsed : " + oxygonUsed*/);
    }
}
