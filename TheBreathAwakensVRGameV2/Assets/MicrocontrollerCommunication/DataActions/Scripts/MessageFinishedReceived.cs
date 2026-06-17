using UnityEngine;

public class MessageFinishedReceived : DataAction
{
    public override void OnDataReceived(string value)
    {
        //Debug.Log(value);
        //try
        //{
        //    //Debug.Log("message ended");
        OnDataReceivedEvent?.Invoke();
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogError("MessageFinishedReceived went wrong " + ex.Message);
        //}
    }
}
