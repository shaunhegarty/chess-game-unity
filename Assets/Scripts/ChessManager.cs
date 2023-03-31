using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManager : MonoBehaviour
{
    public BoardBuilder Board;
    public Piece chessPiece;

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
            chessPiece.SetPositionToTargetSquare(Board.AllSquares[1][1]);
        }        
    }
}
