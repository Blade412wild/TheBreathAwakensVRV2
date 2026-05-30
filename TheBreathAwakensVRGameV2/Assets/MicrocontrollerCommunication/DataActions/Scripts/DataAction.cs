using System;
using UnityEngine;

public class DataAction : MonoBehaviour
{
    public Action OnDataReceivedEvent;

    [SerializeField] protected BreathingDeviceData data;
    public virtual void OnDataReceived(string value)
    {

    }

}
