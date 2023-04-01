using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public Transform target;    
    public float distance = 5.0f;
    public float xSpeed = 125.0f;
    bool rightClicked = false;
    float x;

    void Start()
    {
        var angles = transform.eulerAngles;
        x = angles.y;
    }

    void Update()
    {
        rightClicked = Input.GetMouseButton(1); 
    }

    void LateUpdate()
    {
        if (target && rightClicked == true)
        {
            x = Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            transform.RotateAround(target.position, Vector3.up, x); 
        }

        transform.position += transform.forward * Input.mouseScrollDelta.y;       
    }
}
