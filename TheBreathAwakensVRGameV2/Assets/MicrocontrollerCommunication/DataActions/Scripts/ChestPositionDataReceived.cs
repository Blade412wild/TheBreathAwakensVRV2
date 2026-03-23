using UnityEngine;

public class ChestPositionDataReceived : DataAction
{

    public override void OnDataReceived(string value)
    {
        //Debug.Log(value);
        try
        {
            data.chestPostion = float.Parse(value);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("chestPostion went wrong " + ex.Message);
        }
    }
}
