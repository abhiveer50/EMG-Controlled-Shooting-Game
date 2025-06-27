using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 500f;
    public float topClamp = -90f;
    public float bottomClamp = 90f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private SerialPort serialPort;

    private float receivedMouseX = 0f;
    private float receivedMouseY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // // Set up the serial port
        // serialPort = new SerialPort("COM10", 115200); // Replace "COM3" with your Arduino serial port
        // serialPort.Open();
        // serialPort.ReadTimeout = 100;
    }

    // Update is called once per frame
    void Update()
    {
    //     // Read data from the serial port
    //     if (serialPort.IsOpen)
    //     {
    //         try
    //         {
    //             string data = serialPort.ReadLine();
    //             ParseData(data);
    //         }
    //         catch (System.Exception)
    //         {
    //             // Handle timeout or any other exceptions
    //         }
    //     }

        // Use the received mouse data
        float mouseX = receivedMouseX * mouseSensitivity * Time.deltaTime;
        float mouseY = receivedMouseY * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;

        // xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

    //     transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}

//     void OnDestroy()
//     {
//         if (serialPort != null && serialPort.IsOpen)
//         {
//             serialPort.Close();
//         }
//     }

//     void ParseData(string data)
//     {
//         string[] coordinates = data.Split(',');
//         if (coordinates.Length == 3)
//         {   
//             float.TryParse(coordinates[1], out receivedMouseX);
//             // float.TryParse(coordinates[2], out receivedMouseY);
//         }
//     }
// }
