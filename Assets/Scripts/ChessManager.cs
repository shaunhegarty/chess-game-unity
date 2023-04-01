using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManager : MonoBehaviour
{
    // Game Objects
    public BoardBuilder Board;
    public GamePiece PawnPrefab;
    public GamePiece RookPrefab;
    public GamePiece KnightPrefab;
    public GamePiece BishopPrefab;
    public GamePiece QueenPrefab;
    public GamePiece KingPrefab;
    public Transform pieceParent;

    // Settings
    public int boardSize = 8;

    // State
    private bool readyForPieces = false;

    public bool GameOver = false;

    public Chess.ChessGame Game { get; private set;  }

    public Dictionary<Team, List<GamePiece>> teams = new() {
        { Team.White, new()},
        { Team.Black, new()}
    };

    public Dictionary<Team, HashSet<BoardSquare>> teamCoverage;

    void Start()
    {
        MainManager.Instance.RegisterChessManager(this);
        Game = new Chess.ChessGame();
        Game.SetupBoard();
    }

    public void SetGameOver()
    {
        GameOver = true;
    }

    /*public void NextTurn()
    {
        turn++;
        HashSet<BoardSquare> coverage = CalculateTeamCoverage(); // Coverage for team which just took its turn

        teamTurn = turn % 2 == 0 ? Team.Black : Team.White; // Switch the teams

        GamePiece king = GetKing(teamTurn); // King for new Team        
        if(coverage.Contains(king.currentSquare)) { // Enemy Team can reach the king, therefore it's check, at a minimum
            if(coverage.IsSupersetOf(king.CanMoveTo)) // If the king has no moves outside of what opponent covers -> checkmate
            {
                Debug.Log($"That's Checkmate Buddy!");
            } else
            {
                // Debug.Log($"{teamTurn} King is in Check!");
            }
            
        }
        Debug.Log($"It's {teamTurn}'s turn");
    }*/

    private HashSet<BoardSquare> CalculateTeamCoverage()
    {
        teamCoverage = new()
        {
            { Team.White, new() },
            { Team.Black, new() }
        };

        teams.TryGetValue(Game.TeamTurn, out List<GamePiece> teamPieces);
        teamCoverage.TryGetValue(Game.TeamTurn, out HashSet<BoardSquare> coverage);
        foreach (GamePiece piece in teamPieces)
        {
            if(piece.gameObject.activeInHierarchy)
            {
                piece.GetAllValidMoves();
                coverage.UnionWith(piece.CanMoveTo);
            }
            
        }
        return coverage;
    }

    public List<BoardSquare> SquaresToBoardSquares(List<Chess.Square> squares)
    {
        List<BoardSquare> boardSquares = new();
        foreach(Chess.Square square in squares)
        {
            boardSquares.Add(Board.AllSquares[square.position.x][square.position.y]);
        }
        return boardSquares;
    }

    private GamePiece GetKing(Team team)
    {
        teams.TryGetValue(team, out List<GamePiece> pieces);
        foreach(GamePiece piece in pieces)
        {
            if(piece.IsKing)
            {
                return piece;
            }
        }
        throw new KeyNotFoundException("Can't find the King!");
    }

    private GamePiece PrefabByType(PieceType pieceType)
    {
        Dictionary<PieceType, GamePiece> prefabMapping = new()
        {
            { PieceType.King, KingPrefab },
            { PieceType.Queen, QueenPrefab },
            { PieceType.Bishop, BishopPrefab },
            { PieceType.Knight, KnightPrefab },
            { PieceType.Rook, RookPrefab },
            { PieceType.Pawn, PawnPrefab },
        };

        prefabMapping.TryGetValue(pieceType, out GamePiece prefab);
        return prefab;
    }

    public void SetReadyForPieces(bool ready)
    {
        readyForPieces = ready;
        if (readyForPieces)
        {
            foreach (Team team in new List<Team> { Team.White, Team.Black })
            {
                Game.pieces.TryGetValue(team, out List<Chess.Piece> pieces);
                foreach (Chess.Piece piece in pieces)
                {
                    SpawnPiece(piece);
                }
            }
        }        
    }

    public void SpawnPiece(Chess.Piece chessPiece)
    {
        GamePiece prefab = PrefabByType(chessPiece.type);
        GamePiece piece = Instantiate(prefab);
        piece.team = chessPiece.team;
        piece.SetPiece(chessPiece);

        int rowIndex = chessPiece.currentSquare.position.x;
        int colIndex = chessPiece.currentSquare.position.y;
        piece.transform.parent = new GameObject($"{prefab} {colIndex + 1}").transform;
        piece.transform.parent.parent = pieceParent;

        // Add piece to appropriate team list. 
        teams.TryGetValue(piece.team, out List<GamePiece> teamPieces);
        teamPieces.Add(piece);
        piece.SetPositionToTargetSquare(Board.AllSquares[rowIndex][colIndex]);

    }
}
