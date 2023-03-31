using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // settings
    private readonly Vector3 positionOffset = new(0, 0.35f, 0);
    private MovementConstraint movementConstraint;
    public MovementTypes movementType = MovementTypes.Queen;
    public Team team = Team.White;

    // State
    public BoardSquare currentSquare;
    private BoardSquare highlightedSquare;
    private List<BoardSquare> allowedSquares;
    public int MoveCount { get; private set; }

    private void Start()
    {
        LoadMovement();
    }

    private void LoadMovement()
    {
        switch (movementType)
        {
            case MovementTypes.King:
                movementConstraint = MovementConstraint.King;
                break;
            case MovementTypes.Queen:
                movementConstraint = MovementConstraint.Queen;
                break;
            case MovementTypes.Rook:
                movementConstraint = MovementConstraint.Rook;
                break;
            case MovementTypes.Bishop:
                movementConstraint = MovementConstraint.Bishop;
                break;
            case MovementTypes.Knight:
                movementConstraint = MovementConstraint.Knight;
                break;
            case MovementTypes.Pawn:
                movementConstraint = new PawnMovement(this);
                break;

            default:
                break;
        }
    }

    public void UpdateMoveCount()
    {
        MoveCount++;
    }

    public void SetSquare(BoardSquare square)
    {
        currentSquare = square;
    }

    void SetPositionToTargetSquare()
    {
        transform.localPosition = positionOffset;
        transform.parent.position = currentSquare.BasePosition;
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

        foreach (List<BoardSquare> row in MainManager.Instance.ChessGame.Board.AllSquares)
        {
            foreach(BoardSquare square in row)
            {
                if(movementConstraint.IsMoveAllowed(currentSquare.Index, square.Index)) {
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
            currentSquare = highlightedSquare;            
            highlightedSquare.SetAsTarget(false);
            highlightedSquare = null;
            UpdateMoveCount();
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



