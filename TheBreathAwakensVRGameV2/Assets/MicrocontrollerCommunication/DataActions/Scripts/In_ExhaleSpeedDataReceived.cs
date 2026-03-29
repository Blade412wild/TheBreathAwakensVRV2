using UnityEngine;
public class In_ExhaleSpeedDataReceived : DataAction
{

    public override void OnDataReceived(string value)
    {
        //Debug.Log(value);
        try
        {
            data.inExhaleSpeed = float.Parse(value);
        }
        catch(System.Exception ex)
        {
            Debug.LogError("In_ExHaleSpeed went wrong " + ex.Message);
        }
    }
}
