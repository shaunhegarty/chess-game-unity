using System.Collections.Generic;
using UnityEngine;

public abstract class MovementConstraint
{
    public static MovementConstraint King = new KingMovement();
    public static MovementConstraint Queen = new QueenMovement();
    public static MovementConstraint Bishop = new BishopMovement();
    public static MovementConstraint Rook = new RookMovement();
    public static MovementConstraint Knight = new KnightMovement();

    public abstract bool IsMoveAllowed(Vector2Int startLocation, Vector2Int endLocation);
}

public enum MovementTypes
{
    King, Queen, Bishop, Rook, Knight, Pawn
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

public class PawnMovement : MovementConstraint
{
    private List<Vector2Int> AllowedMoves = new();
    private Piece chessPiece;

    private Vector2Int baseMove = new(1, 0);
    private Vector2Int startMove = new(2, 0);
    private Vector2Int enPassant = new(1, 1);

    public PawnMovement(Piece piece)
    {
        chessPiece = piece;
    }

    public override bool IsMoveAllowed(Vector2Int startLocation, Vector2Int endLocation)
    {
        // Knights may move 2 steps in 1 direction and 1 another. No short steps
        // That means the only permitted movements are (1, 2) and (2, 1)

        var diff = endLocation - startLocation;
        if (chessPiece.team == Team.Black)
        {
            diff *= -1;  // Switch direction for
        }

        if ((diff == baseMove) || (chessPiece.MoveCount == 0 && diff == startMove))
        {
            return true;
        }
        /* en passant */
        return false;
    }
}
