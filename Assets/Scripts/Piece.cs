using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public BoardSquare currentPosition;
    private readonly Vector3 positionOffset = new(0, 0.35f, 0);

    private BoardSquare highlightedSquare;

    private Vector2Int allowedMove = new Vector2Int(1, 1); // Forward, Sideways
    private List<BoardSquare> allowedSquares;

    public void SetSquare(BoardSquare square)
    {
        currentPosition = square;
    }

    void SetPositionToTargetSquare()
    {
        transform.localPosition = positionOffset;
        transform.parent.position = currentPosition.BasePosition;
        GetValidSquares();
    }

    public void SetPositionToTargetSquare(BoardSquare square)
    {
        SetSquare(square);
        SetPositionToTargetSquare();                
    }

    private void GetValidSquares()
    {
        allowedSquares = new();

        Vector2Int currentIndex = currentPosition.Index;
        float allowedDistance = Vector2Int.Distance(Vector2Int.zero, allowedMove);

        foreach (List<BoardSquare> row in MainManager.Instance.ChessGame.Board.AllSquares)
        {
            foreach(BoardSquare square in row)
            {
                if(Vector2Int.Distance(currentIndex, square.Index) <= allowedDistance) {
                    allowedSquares.Add(square);
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if (allowedSquares == null)
        {
            GetValidSquares();
        }
        HighlightCandidateSquares(true);

    }

    private void OnMouseUp()
    {
        HighlightCandidateSquares(false);

        if (highlightedSquare != null)
        {
            currentPosition = highlightedSquare;            
            highlightedSquare.SetAsTarget(false);
            highlightedSquare = null;
        }
        SetPositionToTargetSquare();

    }

    private void OnMouseDrag()
    {
        HighlightTargetSquare();
    }

    private void HighlightCandidateSquares(bool isCandidate)
    {
        foreach (BoardSquare square in allowedSquares)
        {
            square.SetAsCandidate(isCandidate);
        }
    }

    private void HighlightTargetSquare()
    {
        //Vector3 fwd = -1 * (Camera.main.transform.position - transform.position);

        Ray ray = new(transform.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction, Color.blue);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            BoardSquare square = hit.transform.gameObject.GetComponent<BoardSquare>();
            bool allowed = allowedSquares.Contains(square);
            // Are we hitting something? Are we hitting something different to the last time?
            if (square != null && !ReferenceEquals(highlightedSquare, square) && allowed)
            {
                // Yes, highlight whatever we're hitting
                square.SetAsTarget(true);
                if (highlightedSquare != null)
                {
                    // Unhighlight the old
                    highlightedSquare.SetAsTarget(false);
                }
                highlightedSquare = square;
            }
            else if ((square == null || !allowed) && highlightedSquare != null)
            {
                highlightedSquare.SetAsTarget(false);
                highlightedSquare = null;
            }
        }
    }


}
