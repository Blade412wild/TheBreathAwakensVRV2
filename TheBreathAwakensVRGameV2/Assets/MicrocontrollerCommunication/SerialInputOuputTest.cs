using UnityEngine;
using System.IO.Ports;

public class SerialInputOuputTest : MonoBehaviour
{



    long currentTime = 0;
    long lastTimeSwitched;
    int portSwitchInterval = 160; // ms
    int baudrate = 9600;

    private SerialPort serial;

    private bool portFound;


    private void Start()
    {
        //serial.Open();
        //serial.ReadTimeout = 100;
    }

    private void Update()
    {
        if (serial == null) return;
        string incomingData = serial.ReadLine();
    }

    //public void ButtonPress(ButtonID id, bool value)
    //{

    //}

    public void TurnLedOnn()
    {
        serial.Write("1");
    }

    public void TurnLedOff()
    {
        serial.Write("0");
    }

    private void OnDisable()
    {
        if (serial == null) return;
        serial.Close();
    }
}
