using UnityEngine;

public class BreathingStateDataReceived : DataAction
{
    public override void OnDataReceived(string value)
    {
        try
        {
            //OnDataReceivedEvent?.Invoke();
            int index = int.Parse(value);
            data.BreathingState = (BreathingState)index;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("BreathingData went wrong : " + ex.Message);
        }
    }
}
