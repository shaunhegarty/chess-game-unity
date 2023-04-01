using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManager : MonoBehaviour
{
    public BoardBuilder Board;
    public Piece PawnPrefab;
    public Piece RookPrefab;
    public Piece KnightPrefab;
    public Piece BishopPrefab;
    public Piece QueenPrefab;
    public Piece KingPrefab;
    public Transform pieceParent;

    public int boardSize = 8;
    private bool readyForPieces = false;

    public int turn = 1;
    public Team teamTurn = Team.White;
    public bool GameOver = false;


    public Dictionary<Team, List<Piece>> teams = new() {
        { Team.White, new()},
        { Team.Black, new()}
    };

    public Dictionary<Team, HashSet<BoardSquare>> teamCoverage;

    // Start is called before the first frame update
    void Start()
    {
        MainManager.Instance.RegisterChessManager(this);
    }

    public void SetGameOver()
    {
        GameOver = true;
    }

    public void NextTurn()
    {
        turn++;
        HashSet<BoardSquare> coverage = CalculateTeamCoverage(); // Coverage for team which just took its turn

        teamTurn = turn % 2 == 0 ? Team.Black : Team.White; // Switch the teams

        Piece king = GetKing(teamTurn); // King for new Team        
        if(coverage.Contains(king.currentSquare)) { // Enemy Team can reach the king, therefore it's check, at a minimum
            if(coverage.IsSupersetOf(king.CanMoveTo)) // If the king has no moves outside of what opponent covers -> checkmate
            {
                Debug.Log($"That's Checkmate Buddy!");
            } else
            {
                Debug.Log($"{teamTurn} King is in Check!");
            }
            
        }
        Debug.Log($"It's {teamTurn}'s turn");
    }

    private HashSet<BoardSquare> CalculateTeamCoverage()
    {
        teamCoverage = new()
        {
            { Team.White, new() },
            { Team.Black, new() }
        };

        teams.TryGetValue(teamTurn, out List<Piece> teamPieces);
        teamCoverage.TryGetValue(teamTurn, out HashSet<BoardSquare> coverage);
        foreach (Piece piece in teamPieces)
        {
            if(piece.gameObject.activeInHierarchy)
            {
                piece.GetAllValidMoves();
                coverage.UnionWith(piece.CanMoveTo);
            }
            
        }
        return coverage;
    }

    private Piece GetKing(Team team)
    {
        teams.TryGetValue(team, out List<Piece> pieces);
        foreach(Piece piece in pieces)
        {
            if(piece.IsKing)
            {
                return piece;
            }
        }
        throw new KeyNotFoundException("Can't find the King!");
    }

    public void SetReadyForPieces(bool ready)
    {
        readyForPieces = ready;
        if (readyForPieces)
        {
            foreach (Team team in new List<Team> { Team.White, Team.Black })
            {
                // Pawns
                for (int i = 0; i < boardSize; i++)
                {
                    SpawnPiece(PawnPrefab, 1, i, team);
                }

                // Rooks
                SpawnPiece(RookPrefab, 0, 0, team);
                SpawnPiece(RookPrefab, 0, 7, team);

                // Knights
                SpawnPiece(KnightPrefab, 0, 1, team);
                SpawnPiece(KnightPrefab, 0, 6, team);

                // Bishops
                SpawnPiece(BishopPrefab, 0, 2, team);
                SpawnPiece(BishopPrefab, 0, 5, team);

                // Queen
                SpawnPiece(QueenPrefab, 0, 3, team);

                // King
                SpawnPiece(KingPrefab, 0, 4, team);
            }



        }        
    }

    public void SpawnPiece(Piece piecePrefab, int rowIndex, int colIndex, Team team)
    {
        Piece piece = Instantiate(piecePrefab);
        piece.team = team;
        
        piece.transform.parent = new GameObject($"{piecePrefab} {colIndex + 1}").transform;
        piece.transform.parent.parent = pieceParent;

        int boardSize = Board.AllSquares.Count;
        // adjust indexes for team;

        // Add piece to appropriate team list. 
        teams.TryGetValue(team, out List<Piece> teamPieces);
        teamPieces.Add(piece);
        if (team == Team.Black) 
        {
            rowIndex = boardSize - rowIndex - 1;

            // King and Queen face each other
            if (!(piece.pieceType == PieceType.King || piece.pieceType == PieceType.Queen))
            {
                colIndex = boardSize - colIndex - 1;
            }
        }
        piece.SetPositionToTargetSquare(Board.AllSquares[rowIndex][colIndex]);

    }
}
