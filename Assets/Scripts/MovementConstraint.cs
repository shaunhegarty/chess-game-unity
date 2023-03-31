using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementConstraint
{
    public static MovementConstraint King = new KingMovement();
    public static MovementConstraint Queen = new QueenMovement();
    public static MovementConstraint Bishop = new BishopMovement();
    public static MovementConstraint Rook = new RookMovement();
    public static MovementConstraint Knight = new KnightMovement();
    public static MovementConstraint Pawn = new PawnMovement();

    public abstract List<BoardSquare> GetValidSquares(Vector2Int startLocation);

    public BoardSquare SquareFromIndex(Vector2Int index)
    {
        Debug.Log($"Getting Square {index}");
        try
        {
            return MainManager.Instance.GetBoard()[index.x][index.y];
        }
        catch (System.ArgumentOutOfRangeException)
        {
            return null;
        }

    }
}


public abstract class DirectionalMovement : MovementConstraint
{
    protected List<Vector2Int> directions;

    public override List<BoardSquare> GetValidSquares(Vector2Int startLocation)
    {
        // move along each direction until we hit an obstacle
        // if it's on the opposing team, include that square, otherwise exclude it.        

        List<BoardSquare> allowed = new();
        // try each direction
        foreach (Vector2Int direction in directions)
        {
            BoardSquare currentSquare = SquareFromIndex(startLocation);
            for (int i = 1; i <= MainManager.Instance.GetBoard().Count; i++)
            {
                BoardSquare square = SquareFromIndex(startLocation + direction * i);
                if (square != null && (square.occupant == null || square.occupant.team != currentSquare.occupant.team))
                {
                    allowed.Add(square);
                }
                else
                {
                    // don't go any further once we it something that isn't allowed. 
                    break;
                }
            }
        }

        return allowed;
    }
}


public enum MovementTypes
{
    King, Queen, Bishop, Rook, Knight, Pawn
}

public class KingMovement : MovementConstraint
{
    private readonly float movementRange = Mathf.Sqrt(2);

    public override List<BoardSquare> GetValidSquares(Vector2Int startLocation)
    {
        List<BoardSquare> allowed = new();
        BoardSquare currentSquare = SquareFromIndex(startLocation);
        foreach (BoardSquare square in MainManager.Instance.GetSquares())
        {
            if (IsMoveAllowed(startLocation, square.Index) 
                && (square.occupant == null || square.occupant.team != currentSquare.occupant.team))
            {
                allowed.Add(square);
            }
        }
        
        return allowed;
    }

    public bool IsMoveAllowed(Vector2Int startLocation, Vector2Int endLocation)
    {
        return Vector2Int.Distance(startLocation, endLocation) <= movementRange;
    }
}

public class QueenMovement : DirectionalMovement
{
    public QueenMovement()
    {
        directions = new()
        {
            new(1, 1),
            new(-1, 1),
            new(-1, -1),
            new(1, -1),
            new(1, 0),
            new(0, 1),
            new(-1, 0),
            new(0, -1)
        };
    }
}

public class BishopMovement : DirectionalMovement
{
    public BishopMovement()
    {
        directions = new()
        {
            new(1, 1),
            new(-1, 1),
            new(-1, -1),
            new(1, -1)
        };
    }    
    
}

public class RookMovement : DirectionalMovement
{
    public RookMovement()
    {
        directions = new()
        {
            new(1, 0),
            new(0, 1),
            new(-1, 0),
            new(0, -1)
        };
    }
}

public class KnightMovement : MovementConstraint
{
    private readonly List<Vector2Int> AllowedMoves = new() { new Vector2Int(1, 2), new Vector2Int(2, 1) };

    public override List<BoardSquare> GetValidSquares(Vector2Int startLocation)
    {
        List<BoardSquare> allowed = new();
        BoardSquare currentSquare = SquareFromIndex(startLocation);
        foreach (BoardSquare square in MainManager.Instance.GetSquares())
        {
            if (IsMoveAllowed(startLocation, square.Index)
                && (square.occupant == null || square.occupant.team != currentSquare.occupant.team))
            {
                allowed.Add(square);
            }
        }

        return allowed;
    }

    public bool IsMoveAllowed(Vector2Int startLocation, Vector2Int endLocation)
    {
        // Knights may move 2 steps in 1 direction and 1 another. No short steps
        // That means the only permitted movements are (1, 2) and (2, 1)
        var diff = endLocation - startLocation;
        diff.x = Mathf.Abs(diff.x);
        diff.y = Mathf.Abs(diff.y);
        return AllowedMoves.Contains(diff);
    }
}

public class PawnMovement : MovementConstraint
{    
    private Vector2Int baseMove = new(1, 0);
    private Vector2Int doubleMove = new(2, 0);
    private readonly List<Vector2Int> attackMoves = new() { new(1, 1), new(1, -1) };
        

    public override List<BoardSquare> GetValidSquares(Vector2Int startLocation)
    {
        List<BoardSquare> allowed = new();
        BoardSquare currentSquare = SquareFromIndex(startLocation);
        Piece currentPiece = currentSquare.occupant;

        // TODO: Adjust direction based on Team

        BoardSquare baseMoveSquare = SquareFromIndex(startLocation + baseMove);
        bool baseMoveBlocked = false;
        if (baseMoveSquare != null && baseMoveSquare.occupant == null)
        {
            allowed.Add(baseMoveSquare);
        } else
        {
            baseMoveBlocked = true;
        }

        if (currentPiece.MoveCount == 0 && !baseMoveBlocked)
        {
            BoardSquare doubleMoveSquare = SquareFromIndex(startLocation + doubleMove);
            if (doubleMoveSquare != null && doubleMoveSquare.occupant == null)
            {
                allowed.Add(doubleMoveSquare);
            }
        }

        foreach (Vector2Int attackMove in attackMoves)
        {
            BoardSquare attackMoveSquare = SquareFromIndex(startLocation + attackMove);
            if (attackMoveSquare != null && attackMoveSquare.occupant != null && attackMoveSquare.occupant.team != currentPiece.team)
            {
                allowed.Add(attackMoveSquare);
            }
        }
        return allowed;
    }
}
