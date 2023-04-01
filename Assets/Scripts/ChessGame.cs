using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class ChessGame
    {
        private readonly int boardSize = 8;
        public Board board;
        public Dictionary<Team, List<Piece>> pieces = new();

        public ChessGame()
        {
            board = new Board(boardSize);
        }

        private int IndexByTeam(Team team, int i)
        {
            return team == Team.White ? i : boardSize - i - 1;
        }

        public void SetupBoard()
        {
            pieces = new() {
                { Team.White, new()},
                { Team.Black, new()}
            };
            foreach (Team team in new List<Team> { Team.White, Team.Black})
            {
                int startRow = IndexByTeam(team, 0);
                // Pawns
                int pawnRow = IndexByTeam(team, 1);
                for(int i = 0; i < boardSize; i++)
                {
                    
                    AddPieceToBoard(PieceType.Pawn, team, new(pawnRow, i));                    
                }

                // Rooks
                AddPieceToBoard(PieceType.Rook, team, new(startRow, IndexByTeam(team, 0)));
                AddPieceToBoard(PieceType.Rook, team, new(startRow, IndexByTeam(team, 7)));
                
                // Knights
                AddPieceToBoard(PieceType.Knight, team, new(startRow, IndexByTeam(team, 1)));
                AddPieceToBoard(PieceType.Knight, team, new(startRow, IndexByTeam(team, 6)));
                
                // Bishops
                AddPieceToBoard(PieceType.Bishop, team, new(startRow, IndexByTeam(team, 2)));
                AddPieceToBoard(PieceType.Bishop, team, new(startRow, IndexByTeam(team, 5)));

                // King and Queen
                AddPieceToBoard(PieceType.Queen, team, new(startRow, IndexByTeam(team, 3)));
                AddPieceToBoard(PieceType.King, team, new(startRow, IndexByTeam(team, 4)));
            }
        }

        private Square GetSquareByPosition(Vector2Int position)
        {
            return board.GetSquare(position);
        }

        private void AddPieceToBoard(PieceType pieceType, Team team, Vector2Int position)
        {
            Piece piece = new(pieceType, team);
            Square square = GetSquareByPosition(position);
            square.AddPiece(piece);

            pieces.TryGetValue(team, out List<Piece> teamPieces);
            teamPieces.Add(piece);
        }

        public void MovePiece(Piece piece, Vector2Int targetPosition)
        {
            Square oldSquare = piece.currentSquare;
            oldSquare.RemovePiece(piece);

            Square square = GetSquareByPosition(targetPosition);
            square.AddPiece(piece);

            piece.IncrementMoveCount();
        }
    }

    public enum SquareState
    {
        Empty, Occupied
    }

    public class Square
    {
        public Vector2Int position;
        public SquareState state;
        public Piece occupant;

        public Square(int x, int y)
        {
            position = new Vector2Int(x, y);
        }

        public void AddPiece(Piece piece)
        {
            occupant = piece;
            state = SquareState.Occupied;
            piece.currentSquare = this;
        }

        public void RemovePiece(Piece piece)
        {
            occupant = null;
            state = SquareState.Empty;
            piece.currentSquare = null;
        }
    }

    public class Board
    {
        List<List<Chess.Square>> internalBoard;

        public Board(int boardSize)
        {
            internalBoard = new();
            for (int x = 0; x < boardSize; x++)
            {
                List<Square> row = new();
                for (int y = 0; y < boardSize; y++)
                {
                    row.Add(new Square(x, y));
                }
                internalBoard.Add(row);
            }
        }

        public Square GetSquare(Vector2Int position)
        {
            return internalBoard[position.x][position.y];
        }

        public Square GetSquare(int x, int y)
        {
            return internalBoard[x][y];
        }

        public List<Square> GetSquares()
        {
            List<Square> squares = new();
            foreach (List<Square> row in internalBoard)
            {
                foreach (Square square in row)
                {
                    squares.Add(square);
                }
            }
            return squares;
        }
    }

    public class Piece
    {
        public PieceType type;
        public Team team;
        public Square currentSquare;
        public int MoveCount { get; private set;  }

        public Piece(PieceType pieceType, Team pieceTeam)
        {
            type = pieceType;
            team = pieceTeam;
        }

        public void IncrementMoveCount()
        {
            MoveCount++;
        }
    }
}

