using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // settings
    private readonly Vector3 positionOffset = new(0, 0.35f, 0);
    private MovementConstraint movementConstraint;
    public MovementTypes movementType = MovementTypes.Queen;

    // State
    public BoardSquare currentSquare;
    private BoardSquare highlightedSquare;
    private List<BoardSquare> allowedSquares;

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

            default:
                break;
        }
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

public enum MovementTypes
{
    King, Queen, Bishop, Rook, Knight
}

public abstract class MovementConstraint
{
    public static MovementConstraint King = new KingMovement();
    public static MovementConstraint Queen = new QueenMovement();
    public static MovementConstraint Bishop = new BishopMovement();
    public static MovementConstraint Rook = new RookMovement();
    public static MovementConstraint Knight = new KnightMovement();

    public abstract bool IsMoveAllowed(Vector2Int startLocation, Vector2Int endLocation);
}

public class KingMovement : MovementConstraint
{
    private readonly float movementRange = Mathf.Sqrt(2);

    public override bool IsMoveAllowed(Vector2Int startLocation, Vector2Int endLocation)
    {
        return Vector2Int.Distance(startLocation, endLocation) <= movementRange;
    }
}

public class QueenMovement : MovementConstraint
{
    public override bool IsMoveAllowed(Vector2Int startLocation, Vector2Int endLocation)
    {
        var diff = endLocation - startLocation;
        return (Mathf.Abs(diff.x) == Mathf.Abs(diff.y)) || diff.x == 0 || diff.y == 0;
    }
}

public class BishopMovement : MovementConstraint
{
    public override bool IsMoveAllowed(Vector2Int startLocation, Vector2Int endLocation)
    {
        // For diagonals, the the change is always of the same magnitude for x and y, only the sign can vary
        // e.g. (2, 2) or (3, 3) or (1, -1)
        var diff = endLocation - startLocation;
        return Mathf.Abs(diff.x) == Mathf.Abs(diff.y);
    }
}

public class RookMovement : MovementConstraint
{
    public override bool IsMoveAllowed(Vector2Int startLocation, Vector2Int endLocation)
    {
        // For rooks, only one axis can change
        var diff = endLocation - startLocation;
        return diff.x == 0 || diff.y == 0;
    }    
}

public class KnightMovement : MovementConstraint
{
    private List<Vector2Int> AllowedMoves = new();

    public KnightMovement()
    {
        AllowedMoves.Add(new Vector2Int(1, 2));
        AllowedMoves.Add(new Vector2Int(2, 1));
    }

    public override bool IsMoveAllowed(Vector2Int startLocation, Vector2Int endLocation)
    {
        // Knights may move 2 steps in 1 direction and 1 another. No short steps
        // That means the only permitted movements are (1, 2) and (2, 1)
        var diff = endLocation - startLocation;
        diff.x = Mathf.Abs(diff.x);
        diff.y = Mathf.Abs(diff.y);
        return AllowedMoves.Contains(diff);
    }
}

