using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class ChessGame
    {
        // static
        private static readonly List<Team> Teams = new() { Team.White, Team.Black};

        // Settings
        private readonly int boardSize = 8;

        // State
        public Board board;
        public Dictionary<Team, List<Piece>> pieces = new();
        private Check checkState = new();
        public int Turn { get; private set; } = 1;
        public Team TeamTurn { get; private set; } = Team.White;
        public Team NonTeamTurn { get; private set; } = Team.Black;
        public bool CheckMate { get { return checkState.isCheckMate; } }
        public string LastMove = "";

        public delegate void NextTurnCallback();
        private NextTurnCallback nextTurnCallback;

        public void SetNextTurnCallBack(NextTurnCallback cb)
        {
            nextTurnCallback = cb;
        }

        public ChessGame()
        {
            board = new Board(boardSize);
        }

        private int IndexByTeam(Team team, int i)
        {
            return team == Team.White ? i : boardSize - i - 1;
        }

        private Team GetOppositeTeam(Team team)
        {
            return team == Team.Black ? Team.White : Team.Black;
        }

        public void SetupBoard()
        {
            pieces = new() {
                { Team.White, new()},
                { Team.Black, new()}
            };
            foreach (Team team in ChessGame.Teams)
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
                AddPieceToBoard(PieceType.Queen, team, new(startRow, 3));
                AddPieceToBoard(PieceType.King, team, new(startRow, 4));
            }
        }

        private Square GetSquareByPosition(Vector2Int position)
        {
            return board.GetSquare(position);
        }

        private void AddPieceToBoard(PieceType pieceType, Team team, Vector2Int position)
        {
            Piece piece = new(pieceType, team, this);
            Square square = GetSquareByPosition(position);
            square.AddPiece(piece);

            pieces.TryGetValue(team, out List<Piece> teamPieces);
            teamPieces.Add(piece);
        }

        public void MovePiece(Piece piece, Vector2Int targetPosition)
        {
            LastMove = $"Moved {piece} to {Square.LabelFromPosition(targetPosition)}";

            Square oldSquare = piece.currentSquare;
            oldSquare.RemovePiece();

            Square targetSquare = GetSquareByPosition(targetPosition);
            targetSquare.RemovePiece();
            targetSquare.AddPiece(piece);

            piece.PostMove();

            

            NextTurn();
        }

        public void NextTurn()
        {

            Turn++;
            TeamTurn = Turn % 2 == 0 ? Team.Black : Team.White;
            NonTeamTurn = TeamTurn == Team.White ? Team.Black : Team.White;

            if (CalculateCheckAll(NonTeamTurn))
            {
                var check = new Check();
                check.teamInCheck = TeamTurn;
                check.inCheck = true;

                if (IsThatCheckMate(TeamTurn))
                {
                    check.isCheckMate = true;
                }
                
                SetCheck(check);
            } else
            {
                SetCheck(new Check());
            }

            nextTurnCallback?.Invoke();

        }

        public void SetCheck(Check check)
        {
            checkState = check;
        }

        public Piece GetKing(Team team)
        {
            pieces.TryGetValue(team, out List<Piece> teamPieces);
            foreach (Piece piece in teamPieces)
            {
                if (piece.IsKing)
                {
                    return piece;
                }
            }
            throw new KeyNotFoundException("Can't find the King!");
        }

        public bool CalculateCheckAll(Team team)
        {
            // Can pieces of a given team reach the opposing King
            HashSet<Square> coverage = new();
            pieces.TryGetValue(team, out var teamPieces);

            // Add all the covered squares to set
            foreach (var piece in teamPieces)
            {                
                coverage.UnionWith(piece.GetValidSquares(simulate: false));                
            }

            // Get the opposing King
            var king = GetKing(GetOppositeTeam(team));

            bool isInCheck = coverage.Contains(king.currentSquare);           
            return isInCheck;
        }

        public void SimulateMoveUnderCheck(Piece piece, Vector2Int targetPosition, out bool stillInCheck)
        {
            /* Simulating a move */
            // Update the board state
            // Do Check Calc
            // Rewind Board State */

            // Debug.Log($"Simulating {piece} Moving to {Square.LabelFromPosition(targetPosition)}");

            // Update the board state
            var oldSquare = piece.currentSquare;
            var targetSquare = GetSquareByPosition(targetPosition);
            var pieceAtTarget = targetSquare.occupant;

            oldSquare.RemovePiece();                        
            targetSquare.AddPiece(piece);

            // Check if move leaves this team in Check            
            stillInCheck = CalculateCheckAll(GetOppositeTeam(piece.team));

            // Rewind the board state
            targetSquare.RemovePiece();
            oldSquare.AddPiece(piece);            
            if(pieceAtTarget != null)
            {
                targetSquare.AddPiece(pieceAtTarget);
            }

        }

        public bool IsThatCheckMate(Team team)
        { 
            /* Determining CheckMate */
            // Simulate every possible move for a given turn
            // If any result in a non-check scenario, then it is not check mate
            // Only need to do this if the king has been determined to be in check??
        
            // Team X is in check, we want to find out if there is any way out. 
            // Get All the pieces for Team X
            pieces.TryGetValue(team, out var teamPieces);
            
            // For each piece 
            foreach (var piece in teamPieces)
            {
                // Don't bother with pieces which have been taken
                if(piece.currentSquare == null)
                {
                    continue;
                }
                // get all the moves
                var allowedSquares = piece.GetValidSquares(simulate: false);

                // simulate each move, and see if it still results in check
                foreach (var move in allowedSquares)
                {
                    SimulateMoveUnderCheck(piece, move.position, out bool stillInCheck);
                    if(!stillInCheck)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public string GameInfo()
        {
            string checkMessage = "";
            if (checkState.inCheck && checkState.isCheckMate)
            {
                checkMessage = $"\nThat's Checkmate! {GetOppositeTeam(checkState.teamInCheck)} Wins!";
            } else if(checkState.inCheck)
            {
                checkMessage = $"{checkState.teamInCheck} is in check!";
            }                

            return
                $"Last Move: \n - {LastMove}\n\n" + 
                $"Turn: {Turn}\n" +
                $"Team: {TeamTurn}\n" +                
                $"{checkMessage}";
        }

        public struct Check
        {
            public bool inCheck;
            public bool isCheckMate;
            public Team teamInCheck;
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

        public string Label { get { return LabelFromPosition(position); } }

        public static string LabelFromPosition(Vector2Int pos)
        {
            return $"{(char)('A' + pos.y)}{pos.x + 1}";
        }

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

        public void RemovePiece()
        {            
            state = SquareState.Empty;
            if(occupant != null)
            {
                occupant.currentSquare = null;
                occupant = null;
            }            
        }

        public override string ToString()
        {
            return Label;
        }
    }

    public class Board
    {
        private readonly List<List<Square>> internalBoard;

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
        // Inputs
        public PieceType type;
        public Team team;       
        public Movement PieceMovement { get; private set; }

        public ChessGame Game;

        // State
        public Square currentSquare;
        public int MoveCount { get; private set; }
        public List<Square> allowedSquares;

        // Property
        public bool IsKing { get { return type == PieceType.King; } }


        public Piece(PieceType pieceType, Team pieceTeam, ChessGame game)
        {
            type = pieceType;
            team = pieceTeam;
            PieceMovement = Movement.GetMovement(type);
            Game = game;
        }
        

        // PostMove updates and calculations
        public void PostMove()
        {            
            MoveCount++;
        }

        public List<Square> GetValidSquares(bool simulate)
        {
            if(currentSquare == null)
            {
                return new();
            }

            var moves = PieceMovement.GetValidSquares(Game.board, currentSquare.position);
            if (simulate)
            {
                List<Square> slimMoves = new();
                foreach(var move in moves)
                {
                    Game.SimulateMoveUnderCheck(this, move.position, out bool stillInCheck);
                    if (!stillInCheck)
                    {
                        slimMoves.Add(move);
                    }
                }
                return slimMoves;
            } else
            {
                return moves;
            }
            

            
        }

        public override string ToString()
        {
            return $"{team} {type} @ {currentSquare}";
        }
    }
}

