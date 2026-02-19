using AYellowpaper;
using JetBrains.Annotations;
using System;
using UnityEngine;

public class JournalEntry : MonoBehaviour
{
    public event Action<int> entryCollectedEvent;

    public bool IsCompleted { get; private set; }

    private int index;
    private Collider[] hands;

    public void Init(int index, Collider[] handColliders)
    {
        this.index = index;
        hands = handColliders;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckIfcolliderIsHands(other))
            EntryCollected();
    }

    private bool CheckIfcolliderIsHands(Collider collider)
    {
        if (collider == hands[0] || collider == hands[1]) return true;
        return false;
    }

    public void EntryCollected()
    {
        entryCollectedEvent?.Invoke(index);
        Debug.Log(transform.name + " : entry collected");
    }

    public void InitRequirements()
    {
        IsCompleted = false;
    }
}
