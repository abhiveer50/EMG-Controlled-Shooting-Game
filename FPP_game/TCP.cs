using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;

public class TCP : MonoBehaviour
{
    Thread thread;
    public int connectionPort = 25001;
    TcpListener server;
    TcpClient client;
    bool running;

    // Variables to store last received data and its count
    int lastReceivedValue = 0;
    int receivedCount = 0;

    void Start()
    {
        // Receive on a separate thread so Unity doesn't freeze waiting for data
        ThreadStart ts = new ThreadStart(GetData);
        thread = new Thread(ts);
        thread.Start();
    }

    void GetData()
    {
        // Create the server
        server = new TcpListener(IPAddress.Any, connectionPort);
        server.Start();

        // Create a client to get the data stream
        client = server.AcceptTcpClient();

        // Start listening
        running = true;
        while (running)
        {
            Connection();
        }
        server.Stop();
    }

    void Connection()
    {
        try
        {
            // Read data from the network stream
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

            // Decode the bytes into a string
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

            // Check if data received is a valid integer
            if (int.TryParse(dataReceived, out int intValue))
            {
                // Check if the received value is the same as the last received value
                if (intValue == lastReceivedValue)
                {
                    receivedCount++;
                }
                else
                {
                    // Reset the count if the value is different
                    lastReceivedValue = intValue;
                    receivedCount = 1;
                }

                // Use the received integer if it is received 3 times consecutively
                if (receivedCount >= 3)
                {
                    PerformAction(intValue);
                }
            }
            else
            {
                Debug.LogWarning("Received data is not a valid integer: " + dataReceived);
                // Reset the count if the data is invalid
                receivedCount = 0;
            }

            // Optionally, send a response back to the client
            nwStream.Write(buffer, 0, bytesRead);
        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e.Message);
            running = false;
        }
    }

    void PerformAction(int intValue)
    {
        switch (intValue)
        {
            case 0:
                SimulateMouseClick(0);
                break;
            case 1:
                SimulateMouseClick(1);
                break;
            case 2:
                SimulateKeyPress(KeyCode.R);
                break;
            default:
                Debug.LogWarning("Received integer is not mapped to any action: " + intValue);
                break;
        }
    }

    void SimulateMouseClick(int button)
    {
        if (button == 0)
        {
            // Simulate left mouse click
            Debug.Log("Simulating left mouse click");
            MouseClick(MouseButton.Left);
        }
        else if (button == 1)
        {
            // Simulate right mouse click
            Debug.Log("Simulating right mouse click");
            MouseClick(MouseButton.Right);
        }
    }

    void SimulateKeyPress(KeyCode key)
    {
        Debug.Log("Simulating key press: " + key);
        InputSimulator.PressKey(key);
    }

    void MouseClick(MouseButton button)
    {
        // Simulate mouse button down and up for click
        InputSimulator.MouseButtonDown(button);
        InputSimulator.MouseButtonUp(button);
    }

    void Update()
    {
        // Update logic if needed
    }

    void OnApplicationQuit()
    {
        // Stop the thread when the application quits
        running = false;
        if (thread != null)
        {
            thread.Abort();
        }
        if (server != null)
        {
            server.Stop();
        }
        if (client != null)
        {
            client.Close();
        }
    }
}

// Helper classes to simulate input (you need to create these)

public static class InputSimulator
{
    public static void PressKey(KeyCode key)
    {
        // Simulate key press
        Debug.Log("Pressing key: " + key);
        // Add actual key press simulation logic here
    }

    public static void MouseButtonDown(MouseButton button)
    {
        // Simulate mouse button down
        Debug.Log("Mouse button down: " + button);
        // Add actual mouse down simulation logic here
    }

    public static void MouseButtonUp(MouseButton button)
    {
        // Simulate mouse button up
        Debug.Log("Mouse button up: " + button);
        // Add actual mouse up simulation logic here
    }
}

public enum MouseButton
{
    Left,
    Right
}