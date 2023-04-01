using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // settings
    public Vector3 positionOffset = new(0, 1f, 0);
    private MovementConstraint movementConstraint;
    public MovementConstraint Movement {
        get {
            if (movementConstraint == null)
            {
                LoadMovement();
            }
            return movementConstraint;
        }
        set
        {
            movementConstraint = value;
        }
    }

    public PieceType pieceType = PieceType.Queen;
    public Team team = Team.White;

    // Materials
    public Material whiteMaterial;
    public Material blackMaterial;
    Material currentMaterial;


    // State
    public BoardSquare currentSquare;
    private BoardSquare highlightedSquare;
    private List<BoardSquare> allowedSquares;
    public List<BoardSquare> CanMoveTo { get { return allowedSquares; } }
    public int MoveCount { get; private set; }
    public bool canRegicide = false;

    // Components
    Renderer pieceRenderer;
    DragAndDrop pieceDragNDrop;

    private void Start()
    {
        pieceRenderer = GetComponent<Renderer>();
        pieceDragNDrop = GetComponent<DragAndDrop>();
        LoadMovement();
        UpdateMaterials();
    }

    private void UpdateMaterials()
    {
        currentMaterial = team == Team.White ? Instantiate(whiteMaterial) : Instantiate(blackMaterial);
        pieceRenderer.material = currentMaterial;
    }


    private void LoadMovement()
    {
        switch (pieceType)
        {
            case PieceType.King:
                movementConstraint = MovementConstraint.King;
                break;
            case PieceType.Queen:
                movementConstraint = MovementConstraint.Queen;
                break;
            case PieceType.Rook:
                movementConstraint = MovementConstraint.Rook;
                break;
            case PieceType.Bishop:
                movementConstraint = MovementConstraint.Bishop;
                break;
            case PieceType.Knight:
                movementConstraint = MovementConstraint.Knight;
                break;
            case PieceType.Pawn:
                movementConstraint = MovementConstraint.Pawn;
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
        // GetValidSquares();
    }

    public void SetPositionToTargetSquare(BoardSquare square)
    {
        if (currentSquare != null)
        {
            currentSquare.SetOccupant(null);
        }        
        SetSquare(square);
        if(square.occupant != null)
        {
            square.occupant.PieceAttacked();
        }
        square.SetOccupant(this);
        SetPositionToTargetSquare();                
    }

    private void GetValidSquares()
    {        
        allowedSquares = Movement.GetValidSquares(currentSquare.Index);
    }

    private bool CheckCanRegicide()
    {
        foreach(BoardSquare square in allowedSquares)
        {
            if(square.occupant != null && square.occupant.team != team && square.occupant.IsKing)
            {
                return true;                
            }
        }
        return false;
    }

    public bool GetAllValidMoves()
    {
        GetValidSquares();
        canRegicide = CheckCanRegicide();
        return canRegicide;
    }

    private bool IsMyTurn => team == MainManager.Instance.ChessGame.teamTurn && !MainManager.Instance.ChessGame.GameOver;
    public bool IsKing => pieceType == PieceType.King;

    private void OnMouseDown()
    {
        if(IsMyTurn)
        {
            pieceDragNDrop.DragAndDropEnabled = true;
            GetValidSquares();
            HighlightCandidateSquares(true);
        }
        
    }

    private void OnMouseUp()
    {
        if (IsMyTurn)
        {
            pieceDragNDrop.DragAndDropEnabled = false;
            HighlightCandidateSquares(false);

            if (highlightedSquare != null)
            {
                SetPositionToTargetSquare(highlightedSquare);
                ClearHighlight();
                UpdateMoveCount();
                MainManager.Instance.ChessGame.NextTurn();
            }
            else
            {
                SetPositionToTargetSquare();
            }
        }
    }


    private void OnMouseDrag()
    {
        if (IsMyTurn)
        {
            HighlightTargetSquare();
        }
    }

    private void ClearHighlight()
    {
        highlightedSquare.SetAsTarget(false);
        highlightedSquare = null;
    }


    private void HighlightCandidateSquares(bool isCandidate)
    {
        foreach (BoardSquare square in allowedSquares)
        {
            square.SetAsCandidate(isCandidate);
        }
    }

    public void PieceAttacked()
    {        
        gameObject.SetActive(false);
        if (pieceType == PieceType.King)
        {
            MainManager.Instance.ChessGame.SetGameOver();
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



