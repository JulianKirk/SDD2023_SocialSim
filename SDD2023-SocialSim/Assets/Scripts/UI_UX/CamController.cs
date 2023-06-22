using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public static float CameraPanSensitivity = 0.5f;

    public static int MapSize;

    Vector3 earlyMousePos;
    Vector3 lateMousePos;

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKey(KeyCode.Mouse0)) 
        {
            lateMousePos = Input.mousePosition;

            transform.Translate(-(lateMousePos - earlyMousePos) * CameraPanSensitivity * (Camera.main.orthographicSize/40));
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -0.5f, MapSize - 1), Mathf.Clamp(transform.position.y, -0.5f, MapSize - 1), transform.position.z);
            // Mathf.Clamp(transform.position.y, -0.5f, MapSize - 1);
            // transform.position += -(lateMousePos - earlyMousePos) * CameraPanSensitivity * (Camera.main.orthographicSize/40);

            earlyMousePos = Input.mousePosition;
        } 
        else 
        {
            earlyMousePos = Input.mousePosition;
        }

        if (Input.mouseScrollDelta.y != 0f) 
        {
            //Ortho vertical size: Height of item you want on the screen / 2
            //Ortho horizontal size: (Width of item you want on the screen * Screen.height) / (Screen.width * 0.5)

            // Camera.main.orthographicSize  

            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Input.mouseScrollDelta.y, 5f, MapGenerator.MapHeight/2);
        }
    }
}
