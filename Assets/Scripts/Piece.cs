using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public BoardSquare currentPosition;
    private readonly Vector3 positionOffset = new(0, 0.35f, 0);

    private BoardSquare highlightedSquare;

    // Start is called before the first frame update
    void Start()
    {        
        SetPositionToTargetSquare();
    }

    void SetPositionToTargetSquare()
    {
        transform.localPosition = positionOffset;
        transform.parent.position = currentPosition.BasePosition;
    }

    private void OnMouseDrag()
    {
        HighlightTargetSquare();
    }

    private void HighlightTargetSquare()
    {
        //Vector3 fwd = -1 * (Camera.main.transform.position - transform.position);

        Ray ray = new(transform.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction, Color.blue);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            BoardSquare square = hit.transform.gameObject.GetComponent<BoardSquare>();
            // Are we hitting something? Are we hitting something different to the last time?
            if (square != null && !ReferenceEquals(highlightedSquare, square))
            {
                // Yes, highlight whatever we're hitting
                square.Highlight();
                if (highlightedSquare != null)
                {
                    // Unhighlight the old
                    highlightedSquare.Dehighlight();
                }
                highlightedSquare = square;
            }
            else if (square == null && highlightedSquare != null)
            {
                highlightedSquare.Dehighlight();
                highlightedSquare = null;
            }
        }
    }

    private void OnMouseUp()
    {
        if (highlightedSquare != null)
        {
            currentPosition = highlightedSquare;
            SetPositionToTargetSquare();
            highlightedSquare.Dehighlight();
            highlightedSquare = null;            
        }
    }
}
