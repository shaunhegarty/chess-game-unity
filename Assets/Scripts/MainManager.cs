using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public ChessManager ChessManager {get; private set;}

    public void RegisterChessManager(ChessManager chessManager)
    {
        ChessManager = chessManager;
    }

    public List<List<BoardSquare>> GetBoard()
    {
        return ChessManager.Board.AllSquares;
    }

    public List<BoardSquare> GetSquares()
    {
        List<BoardSquare> squares = new();
        foreach (List<BoardSquare> row in Instance.GetBoard())
        {
            foreach (BoardSquare square in row)
            {
                squares.Add(square);
            }
        }
        return squares;
    }




    /* Not sure if it's sustainable, but I've made a getter that sets D:
     * The benefit, in theory, is that now I don't need to put null checks 
     * everywhere or do anything special to ensure that it exists somewhere if I start from any scene other than the first
     * */
    private static MainManager instance; // use private lower case instance to be the true value
    public static MainManager Instance // then Instance is just property magic from then on
    {
        get { return GetInstance(); }
        private set { instance = value; }
    }

    private static MainManager GetInstance()
    {
        if (instance != null)
        {
            return instance;
        }
        else
        {
            GameObject newObject = new("MainManager");
            MainManager gameRunner = newObject.AddComponent<MainManager>();
            return gameRunner;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
