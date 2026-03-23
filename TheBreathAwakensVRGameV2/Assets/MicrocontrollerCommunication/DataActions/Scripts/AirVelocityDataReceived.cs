using UnityEngine;

public class AirVelocityDataReceived : DataAction
{
    public override void OnDataReceived(string value)
    {
        try
        {
            data.AirVelocity = float.Parse(value);
            //Debug.Log("airvelocity : " + data.AirVelocity + " ms");

        }
        catch (System.Exception ex)
        {
            Debug.LogError("AirVelocityData went wrong : " + ex.Message);
        }
    }
}
