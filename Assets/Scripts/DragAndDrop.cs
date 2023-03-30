using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{

    Vector3 mousePositionOnClick;

    /* This all works but I don't really know why */

    private Vector3 GetMousePosition()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }    

    private void OnMouseDown()
    {
        // difference from transform centre screen position to mouse screen position
        mousePositionOnClick = Input.mousePosition - GetMousePosition();
    }

    private void OnMouseDrag()
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePositionOnClick);
        newPosition.y = transform.position.y;
        transform.position = newPosition;
        Debug.Log($"WtS: {GetMousePosition()} | StW {Camera.main.ScreenToWorldPoint(Input.mousePosition)} | ");
    }
}
