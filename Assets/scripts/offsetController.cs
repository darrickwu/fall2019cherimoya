using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class offsetController : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed;

    //move this offset, the camera should follow it
    void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float strafe = Input.GetAxis("Horizontal") * speed;

        translation *= Time.deltaTime;
        strafe *= Time.deltaTime;
        

        // use brackets to rotate camera, rotationspeed per second
        transform.Translate(strafe, 0, translation);
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}
