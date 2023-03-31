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
            // Pawns
            for (int i = 0; i < boardSize; i++)
            {
                SpawnPiece(PawnPrefab, 1, i);                
            }

            // Rooks
            SpawnPiece(RookPrefab, 0, 0);
            SpawnPiece(RookPrefab, 0, 7);

            // Knights
            SpawnPiece(KnightPrefab, 0, 1);
            SpawnPiece(KnightPrefab, 0, 6);

            // Bishops
            SpawnPiece(BishopPrefab, 0, 2);
            SpawnPiece(BishopPrefab, 0, 5);

            // Queen
            SpawnPiece(QueenPrefab, 0, 3);

            // King
            SpawnPiece(KingPrefab, 0, 4);


        }        
    }

    public void SpawnPiece(Piece piecePrefab, int rowIndex, int colIndex)
    {
        Piece piece = Instantiate(piecePrefab);
        piece.transform.parent = new GameObject($"{piecePrefab} {colIndex + 1}").transform;
        piece.SetPositionToTargetSquare(Board.AllSquares[rowIndex][colIndex]);
    }
}
