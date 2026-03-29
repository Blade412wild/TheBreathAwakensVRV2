using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BreathingDevice/DataContainer", fileName = "DataContainer")]
public class BreathingDeviceData : ScriptableObject
{
    //public system data
    public bool IsConnected;

    // sensor data
    public BreathingState BreathingState;

    public bool ExHalingThroughNose;
    public float inExhaleSpeed;
    public float chestPostion;
    public float AirVelocity;

}
public enum BreathingState { inhaling = 0, holdingBreath = 1, exhaling = 2 };

