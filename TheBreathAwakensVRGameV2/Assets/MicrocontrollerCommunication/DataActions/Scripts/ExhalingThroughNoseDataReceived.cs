using UnityEngine;

public class ExhalingThroughNoseDataReceived : DataAction
{
    public override void OnDataReceived(string value)
    {
        try
        {
            data.ExHalingThroughNose = bool.Parse(value);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("ExhalingThroughNose went wrong " + ex.Message);
        }
    }

}
