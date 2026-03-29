using System;
using System.IO.Ports;
using UnityEngine;

public class SerialPortFinder2 : MonoBehaviour
{
    public int Index = 0;
    public bool SetSerial = false;

    private int baudrate = 9600;
    private int timeOut = 50;
    private SerialPort serialPort;



    private void Start()
    {
    }

    private void Update()
    {
        if (SetSerial)
        {
            SetSerial = false;
            DisposeSerial();

            SetSerialPort();


        }
    }

    private void OnDisable()
    {
        DisposeSerial();
    }

    private void DisposeSerial()
    {
        if (serialPort != null)
        {
            Debug.Log("closing " + serialPort.PortName);
            serialPort.Close();
            serialPort.DataReceived -= OndataReceived;
            serialPort.Dispose();

        }

    }

    private void SetSerialPort()
    {
        string[] ports = SerialPort.GetPortNames();

        if (Index >= ports.Length)
        {
            Debug.Log("index goes beyond ports.lenght");
            return;
        }
        Debug.Log(" trying : " + ports[Index]);
        serialPort = new SerialPort(ports[Index]);
        serialPort.DataReceived += OndataReceived;
        serialPort.Open();

    }



    private void OndataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
        string inData = sp.ReadExisting();
        Debug.Log("received data");
        Debug.Log(inData);
        throw new NotImplementedException();

    }
}

