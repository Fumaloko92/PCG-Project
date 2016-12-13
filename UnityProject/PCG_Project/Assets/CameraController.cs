using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float speed;
    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private bool moveF, moveB, moveU, moveD, moveL, moveR;
    // Update is called once per frame
    void Update()
    {
        RotateCamera();
        if (Input.GetKeyDown(KeyCode.W))
            moveF = true;
        if (Input.GetKeyDown(KeyCode.S))
            moveB = true;
        if (Input.GetKeyDown(KeyCode.A))
            moveL = true;
        if (Input.GetKeyDown(KeyCode.D))
            moveR = true;
        if (Input.GetKeyDown(KeyCode.Z))
            moveD = true;
        if (Input.GetKeyDown(KeyCode.X))
            moveU = true;

        if (moveF)
            MoveForward();
        if (moveB)
            MoveBackward();
        if (moveL)
            MoveLeft();
        if (moveR)
            MoveRight();
        if (moveD)
            MoveDown();
        if (moveU)
            MoveUp();

        if (Input.GetKeyUp(KeyCode.W))
            moveF = false;

        if (Input.GetKeyUp(KeyCode.S))
            moveB = false;

        if (Input.GetKeyUp(KeyCode.A))
            moveL = false;

        if (Input.GetKeyUp(KeyCode.D))
            moveR = false;

        if (Input.GetKeyUp(KeyCode.Z))
            moveD = false;

        if (Input.GetKeyUp(KeyCode.X))
            moveU = false;
    }
    private void RotateCamera()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
    private void MoveForward()
    {
        transform.position = (transform.position + new Vector3(0, 0, Time.deltaTime * speed));
    }

    private void MoveBackward()
    {
        transform.position = (transform.position - new Vector3(0, 0, Time.deltaTime * speed));
    }

    private void MoveLeft()
    {
        transform.position = (transform.position + new Vector3(Time.deltaTime * speed, 0, 0));
    }

    private void MoveRight()
    {
        transform.position = (transform.position - new Vector3(Time.deltaTime * speed, 0, 0));
    }

    private void MoveUp()
    {
        transform.position = (transform.position + new Vector3(0, Time.deltaTime * speed, 0));
    }

    private void MoveDown()
    {
        transform.position = (transform.position - new Vector3(0, Time.deltaTime * speed, 0));
    }
}
