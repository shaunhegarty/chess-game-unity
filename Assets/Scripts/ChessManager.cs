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
    // Start is called before the first frame update
    void Start()
    {
        MainManager.Instance.RegisterChessManager(this);
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

        int boardSize = Board.AllSquares.Count;
        // adjust indexes for team;
        if (team == Team.Black)
        {
            rowIndex = boardSize - rowIndex - 1;

            // King and Queen face each other
            if (!(piece.movementType == MovementTypes.King || piece.movementType == MovementTypes.Queen))
            {
                colIndex = boardSize - colIndex - 1;
            }
        }
        piece.SetPositionToTargetSquare(Board.AllSquares[rowIndex][colIndex]);
    }
}
