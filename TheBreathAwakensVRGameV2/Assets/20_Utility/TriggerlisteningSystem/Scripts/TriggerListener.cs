using System;
using UnityEngine;

public class TriggerListener : MonoBehaviour
{
    public event Action OnTargetEnteredTriggerEvent;

    [SerializeField] private Collider targetCollider;
    [SerializeField] private TriggerArea triggerArea;

    public void StartListening()
    {
        triggerArea.Set(targetCollider);
        triggerArea.OnTargetEnteredTriggerEvent += HandleTargetEnteredTriggerEvent;
    }

    public void StopListening()
    {
        triggerArea.OnTargetEnteredTriggerEvent -= HandleTargetEnteredTriggerEvent;
    }

    private void HandleTargetEnteredTriggerEvent()
    {
        //Debug.Log("Handle event (listener) ");
        OnTargetEnteredTriggerEvent?.Invoke();
    }

    private void OnDisable()
    {
        StopListening();
    }


}
