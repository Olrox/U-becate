using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    Rigidbody rb;
    Vector2 inputMov;
    Vector2 inputRot;
    public float VelCamina = 10f;

    public float sensibilidadMouse = 1;
    Transform cam;
    float rotX;

    void Start ()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        cam = transform.GetChild(1);
        rotX = cam.eulerAngles.x;

    }
    void Update()
    {
        inputMov.x = Input.GetAxis("Horizontal"); //
        inputMov.y = Input.GetAxis("Vertical");  //

        inputRot.x = Input.GetAxis("Mouse X") * sensibilidadMouse;
        inputRot.y = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void FixedUpdate()
{
    // Crear vector de movimiento y normalizar
    Vector3 movement = (transform.forward * inputMov.y + transform.right * inputMov.x).normalized;
    
    // Aplicar velocidad manteniendo la velocidad Y actual (gravedad)
    rb.velocity = movement * VelCamina + new Vector3(0, rb.velocity.y, 0);

    // Rotación (tu código actual está bien)
    transform.rotation *= Quaternion.Euler(0, inputRot.x, 0);
    
    rotX -= inputRot.y;
    rotX = Mathf.Clamp(rotX, -50, 30);
    cam.localRotation = Quaternion.Euler(rotX, 0, 0);
}
}