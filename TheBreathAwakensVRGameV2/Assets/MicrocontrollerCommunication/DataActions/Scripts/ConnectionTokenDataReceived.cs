using UnityEngine;

public class ConnectionTokenDataReceived : DataAction
{

    public override void OnDataReceived(string value)
    {
        try
        {
            if (value == "1") // trying to make a connection
            {
                data.IsConnected = true;

                if (MicrocontrollerManager.SerialPort.IsOpen)
                {
                    MicrocontrollerManager.SerialPort.Write(value); //TODO create Serial Writer with que?
                }
            }


        }
        catch (System.Exception ex)
        {
            Debug.LogError("connection token went wrong : " + ex.Message);
        }
    }
}
