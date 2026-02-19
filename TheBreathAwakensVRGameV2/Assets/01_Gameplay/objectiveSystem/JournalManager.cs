using System;
using System.Collections.Generic;
using UnityEngine;

public class JournalManager : MonoBehaviour
{
    public event Action journalCompleted;

    [SerializeField] private List<JournalEntry> activeJournalEntries;
    [SerializeField] private Collider[] hands;

    private List<JournalEntry> inactivejournalEntries = new List<JournalEntry>();

    private void Start()
    {
        for (int i = 0; i < activeJournalEntries.Count; i++)
        {
            activeJournalEntries[i].Init(i, hands);
            activeJournalEntries[i].entryCollectedEvent += HandleEntryCollectedEvent;
        }


    }

    public void HandleEntryCollectedEvent(int index)
    {
        activeJournalEntries[index].entryCollectedEvent -= HandleEntryCollectedEvent;
        inactivejournalEntries.Add(activeJournalEntries[index]);
        activeJournalEntries.RemoveAt(index);

        if(activeJournalEntries.Count == 0)
            journalCompleted?.Invoke();

    }
}

