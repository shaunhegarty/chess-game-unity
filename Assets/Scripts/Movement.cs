using Chess;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement
{
    public static Movement King = new KingMovement();
    public static Movement Queen = new QueenMovement();
    public static Movement Bishop = new BishopMovement();
    public static Movement Rook = new RookMovement();
    public static Movement Knight = new KnightMovement();
    public static Movement Pawn = new PawnMovement();
    public static Dictionary<PieceType, Movement> constraints = new() { 
        { PieceType.King, King },
        { PieceType.Queen, Queen },
        { PieceType.Bishop, Bishop },
        { PieceType.Rook, Rook },
        { PieceType.Knight, Knight },
        { PieceType.Pawn, Pawn }
    };
    public static Movement GetMovement(PieceType pieceType)
    {
        constraints.TryGetValue(pieceType, out Movement constraint);
        return constraint;
    }

    public abstract List<Chess.Square> GetValidSquares(Chess.Board board, Vector2Int startLocation);

    public Chess.Square SquareFromIndex(Chess.Board board, Vector2Int index)
    {
        try
        {
            return board.GetSquare(index);
        }
        catch (System.ArgumentOutOfRangeException)
        {
            return null;
        }

    }
}


public abstract class DirectionalMovement : Movement
{
    protected List<Vector2Int> directions;
    protected int range = 0;

    public override List<Chess.Square> GetValidSquares(Chess.Board board, Vector2Int startLocation)
    {
        // move along each direction until we hit an obstacle
        // if it's on the opposing team, include that square, otherwise exclude it.        

        int directionRange = range == 0 ? MainManager.Instance.GetBoard().Count : range;

        List <Chess.Square> allowed = new();
        // try each direction
        foreach (Vector2Int direction in directions)
        {
            Chess.Square currentSquare = SquareFromIndex(board, startLocation);
            for (int i = 1; i <= directionRange; i++)
            {
                Chess.Square square = SquareFromIndex(board, startLocation + direction * i);
                if (square != null && (square.occupant == null || square.occupant.team != currentSquare.occupant.team))
                {
                    allowed.Add(square);

                    // Include an enemy square so that we can attack it, but don't go any further. 
                    if (square.occupant != null && square.occupant.team != currentSquare.occupant.team)
                    {
                        break;
                    }
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


public enum PieceType
{
    King, Queen, Bishop, Rook, Knight, Pawn
}

public class KingMovement : DirectionalMovement
{
    public KingMovement()
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
        range = 1;
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

public class KnightMovement : DirectionalMovement
{
    public KnightMovement()
    {
        directions = new()
        {
            new(1, 2),
            new(-1, 2),
            new(-1, -2),
            new(1, -2),
            new(2, 1),
            new(-2, 1),
            new(-2, -1),
            new(2, -1)
        };

        range = 1;
    }
}

public class PawnMovement : Movement
{    
    private Vector2Int baseMove = new(1, 0);
    private Vector2Int doubleMove = new(2, 0);
    private readonly List<Vector2Int> attackMoves = new() { new(1, 1), new(1, -1) };
        

    public override List<Square> GetValidSquares(Board board, Vector2Int startLocation)
    {
        List<Square> allowed = new();
        Square currentSquare = SquareFromIndex(board, startLocation);
        Piece currentPiece = currentSquare.occupant;
        
        // Adjust direction based on Team
        Vector2Int direction = new (currentPiece.team == Team.Black ? -1 : 1, 1);

        Square baseMoveSquare = SquareFromIndex(board, startLocation + baseMove * direction);
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
            Square doubleMoveSquare = SquareFromIndex(board, startLocation + doubleMove * direction);
            if (doubleMoveSquare != null && doubleMoveSquare.occupant == null)
            {
                allowed.Add(doubleMoveSquare);
            }
        }

        foreach (Vector2Int attackMove in attackMoves)
        {
            Square attackMoveSquare = SquareFromIndex(board, startLocation + attackMove * direction);
            if (attackMoveSquare != null && attackMoveSquare.occupant != null && attackMoveSquare.occupant.team != currentPiece.team)
            {
                allowed.Add(attackMoveSquare);
            }
        }
        return allowed;
    }
}
