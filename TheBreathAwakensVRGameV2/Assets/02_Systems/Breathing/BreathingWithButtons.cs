using UnityEngine;
using UnityEngine.InputSystem;

public class BreathingWithButtons : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] private bool Usethis;

    [Range(0,15)] 
    [SerializeField] private float speed;

    [Header("Ref")]
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private BreathingDeviceData data;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actionReference.action.Enable();


    }

    // Update is called once per frame
    void Update()
    {
        if (!Usethis) return;

        float rawInput = actionReference.action.ReadValue<float>();

        SetbreathingState(rawInput);
        SetInhaleExhaleSpeed(rawInput);
        //SetAudioVariable();
    }

    private void SetAudioVariable()
    {
        //Debug.Log(data.inExhaleSpeedAudioScale);
    }

    private void SetInhaleExhaleSpeed(float input)
    {
        float artificialSpeed = ManipulateSpeed(input);
        data.inExhaleSpeed = artificialSpeed;

    }

    private void SetbreathingState(float input)
    {
        if (input == 0)
        {
            data.BreathingState = BreathingState.holdingBreath;
        }
        else if (input == -1)
        {
            data.BreathingState = BreathingState.exhaling;

        }
        else if (input == 1)
        {
            data.BreathingState = BreathingState.inhaling;
        }

    }

    private float ManipulateSpeed(float input)
    {
        float newSpeed = speed;
        //Debug.Log("value : " + rawValue);
        return newSpeed;

    }
}
