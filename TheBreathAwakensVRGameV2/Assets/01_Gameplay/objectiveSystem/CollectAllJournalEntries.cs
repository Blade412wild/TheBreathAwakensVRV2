using UnityEngine;
public class CollectAllJournalEntries : ObjectiveBaseClass
{
    public override void ObjectiveCompleted()
    {
        Debug.Log(transform.name + " : collected all entries");

    }
}
