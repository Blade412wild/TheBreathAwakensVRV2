using UnityEngine;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

public class SerialPortFinder
{
    public Action<SerialPort> OnSerialPortFound;

    private int baudrate;
    private float scanIntervalMs;
    private SerialPort serial;
    private bool portFound = false;
    private bool stopThread = false;
    private string[] availablePorts;
    private Thread portSearchThread;

    public SerialPortFinder(int baudRate, float interval)
    {
        baudrate = baudRate;
        scanIntervalMs = interval;
        Setup();
    }

    void Setup()
    {
        availablePorts = SerialPort.GetPortNames();
        if (availablePorts == null || availablePorts.Length == 0)
        {
            Debug.LogWarning("No serial ports found. Check that the device is connected, the driver is installed, and Unity has permission to access USB devices.");
            return;
        }

        var candidatePorts = availablePorts
            .Where(p => p.IndexOf("usb", StringComparison.OrdinalIgnoreCase) >= 0
                     || p.IndexOf("modem", StringComparison.OrdinalIgnoreCase) >= 0
                     || p.IndexOf("serial", StringComparison.OrdinalIgnoreCase) >= 0)
            .ToArray();

        if (candidatePorts.Length > 0)
            availablePorts = candidatePorts;

        foreach (var p in availablePorts)
            Debug.Log($"Found port: {p}");

        portSearchThread = new Thread(TryToFindPort);
        portSearchThread.Start();
    }

    private void TryToFindPort()
    {
        for (int i = availablePorts.Length - 1; i >= 0; i--)
        {
            if (stopThread || portFound) break;

            Debug.Log($"Trying {availablePorts[i]}....");
            if (TryOpenPort(availablePorts[i]))
                break;

            Thread.Sleep((int)scanIntervalMs);
        }

        if (!portFound)
            Debug.LogWarning("No valid COM port found! If your device is on macOS, verify the actual /dev/cu.* or /dev/tty.* path.");
    }

    private bool TryOpenPort(string portName)
    {
        try
        {
            serial = new SerialPort(portName, baudrate)
            {
                ReadTimeout = 50,
                NewLine = "\n"
            };
            serial.Open();
            Debug.Log($"Opened {portName}");

            DateTime start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < 2000 && !stopThread)
            {
                try
                {
                    string data = serial.ReadLine();
                    if (data.Contains("1"))
                    {
                        Debug.Log($"✅ Connection found on {portName}");
                        portFound = true;
                        //serial.Write("1");
                        // Don't close here - let the callback handler manage the connection
                        OnSerialPortFound?.Invoke(serial);
                        return true;
                    }
                }
                catch (TimeoutException) { }
                Thread.Sleep(10);
            }

            serial.Close();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to open {portName}: {ex.Message}");
        }

        return false;
    }

    public void OnDisable()
    {
        stopThread = true;
        if (serial?.IsOpen == true)
            serial.Close();
    }
}
