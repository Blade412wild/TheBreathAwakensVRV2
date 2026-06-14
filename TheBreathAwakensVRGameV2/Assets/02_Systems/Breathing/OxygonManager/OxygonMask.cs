using System;
using Unity.VisualScripting;
using UnityEngine;

public class OxygonMask : MonoBehaviour
{
    public event Action EquipEvent;
    public event Action UnequipEvent;

    [Header("Control")]
    [SerializeField] private bool equip;
    [SerializeField] private bool unequip;

    [SerializeField] private BreathingDeviceData breathingDeviceData;
    public float CurrentInExhaleSpeed { get; private set; } = 0.0f;
    public BreathingState CurrentBreathingState { get; private set; } = BreathingState.holdingBreath;

    public bool Equipped { get; private set; }

    private float glassIntegrity;
    private float tubeIntergrity;

    private void Start()
    {
        CurrentBreathingState = BreathingState.holdingBreath;
        CurrentInExhaleSpeed = 0.0f;
        Equipped = false;
    }

    private void Update()
    {
        if (equip)
        {

            equip = false;
            if (Equipped == true) return;
            EquipGasMask();
        }

        if (unequip)
        {
            unequip = false;
            if (Equipped == false) return;
            UnequipGasMask();
        }
    }
    public void OnUpdate()
    {
        CurrentInExhaleSpeed = breathingDeviceData.inExhaleSpeed;
        CurrentBreathingState = breathingDeviceData.BreathingState;
    }


    private void UnequipGasMask()
    {
        Equipped = false;
        UnequipEvent?.Invoke();
        CurrentInExhaleSpeed = 0.0f;
        CurrentBreathingState = CurrentBreathingState;
    }
    private void EquipGasMask()
    {
        Equipped = true;
        EquipEvent?.Invoke();
    }
}


