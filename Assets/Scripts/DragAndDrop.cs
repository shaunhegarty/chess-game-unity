using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    public float dragPlaneOffset = 1;
    public bool DragAndDropEnabled;

    private void OnMouseDrag()
    {
        Debug.Log($"Enabled: {DragAndDropEnabled}");
        if(DragAndDropEnabled)
        {
            DragByPlane();
        }
        
    }

    // https://forum.unity.com/threads/dragging-objects-on-a-plane.5485/
    private void DragByPlane()
    {
        var dragPlane = new Plane(Vector3.up, new Vector3(0, dragPlaneOffset, 0));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter)) //Plane.Raycast is the magic I was missing
        {
            //Get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);

            //Move your cube GameObject to the point where you clicked
            transform.position = hitPoint;
        }
    }
}
