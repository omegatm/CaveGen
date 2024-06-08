using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fpControls : MonoBehaviour
{
    // Start is called before the first frame update
    public float sensx;
    public float sensy;
    public Transform orientation;
    bool cursorlock = true;

    float xRotation;
    float yRotation;

    void Start()
    {
        UnityEngine.Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;
    }
    void Update()
    {
        if (cursorlock)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensx;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensy;
            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            cursorlock = !cursorlock;
        }
        if (!cursorlock)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
        
}
