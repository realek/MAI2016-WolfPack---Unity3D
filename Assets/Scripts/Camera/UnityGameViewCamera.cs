using UnityEngine;
using System.Collections;

public class UnityGameViewCamera : MonoBehaviour
{

    float m_mainSpeed = 100.0f; //default speed
    float m_shiftAdd = 250.0f; //shift speed factor multiplied by time shift is held down
    float m_maxShift = 1000.0f; // max speed while using shift
    float m_camSens = 0.25f; // mouse sensitivity
    private Vector3 lastMouse;
    private float totalRun = 1.0f;

    private bool m_mouseLook = false;

    void Update()
    {
        //toggle mouse look
        if (Input.GetKeyDown(KeyCode.Space))
            m_mouseLook = !m_mouseLook;

        if (m_mouseLook)
        {
            lastMouse = Input.mousePosition - lastMouse;
            lastMouse = new Vector3(-lastMouse.y * m_camSens, lastMouse.x * m_camSens, 0);
            lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            transform.eulerAngles = lastMouse;
            lastMouse = Input.mousePosition;
        }
        else
            lastMouse = Input.mousePosition;

        Vector3 velocity = GetVelocity();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            totalRun += Time.deltaTime;
            velocity = velocity * totalRun * m_shiftAdd;
            velocity.x = Mathf.Clamp(velocity.x, -m_maxShift, m_maxShift);
            velocity.y = Mathf.Clamp(velocity.y, -m_maxShift, m_maxShift);
            velocity.z = Mathf.Clamp(velocity.z, -m_maxShift, m_maxShift);
        }
        else
        {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            velocity = velocity * m_mainSpeed;
        }

        velocity = velocity * Time.deltaTime;
        Vector3 newPosition = transform.position;
        transform.Translate(velocity);

    }

    private Vector3 GetVelocity()
    { 
        //returns current velocity based on input
        Vector3 velocity = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            velocity += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            velocity += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            velocity += Vector3.right;
        }
        return velocity;
    }
}