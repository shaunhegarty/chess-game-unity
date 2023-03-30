using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBuilder : MonoBehaviour
{
    public GameObject squarePrefab;

    private BoxCollider squareCollider;
    public List<List<BoardSquare>> AllSquares { get; private set; } = new();

    // Start is called before the first frame update
    void Start()
    {
        squareCollider = squarePrefab.GetComponent<BoxCollider>();
        BuildBoard(MainManager.Instance.ChessGame.boardSize);
        MainManager.Instance.ChessGame.SetReadyForPieces(true);
    }

    public void BuildBoard(int size)
    {
        for (int i = 0; i < size; i++)
        {
            AllSquares.Add(BuildRow(size, i));
        }
    }

    List<BoardSquare> BuildRow(int size, int rowIndex)
    {
        List<BoardSquare> row = new();
        Vector3 squareSize = squareCollider.size;
        for (int i = 0; i < size; i++)
        {
            Vector3 position = new(
                x: i * squareSize.x, 
                y: squarePrefab.transform.position.y, 
                z: rowIndex * squareSize.z
            );
            Team team = (i + rowIndex) % 2 == 0 ? Team.Black : Team.White;
            Vector2Int index = new(rowIndex, i);
            BoardSquare bSquare = CreateSquare(position, team, index);
            row.Add(bSquare);
        }
        return row;
    }

    BoardSquare CreateSquare(Vector3 squarePosition, Team team, Vector2Int index)
    {
        GameObject square = Instantiate(squarePrefab, squarePosition, squarePrefab.transform.rotation);
        square.transform.parent = transform;
        BoardSquare bSquare = square.GetComponent<BoardSquare>();
        bSquare.SetTeam(team);
        bSquare.SetIndex(index);
        return bSquare;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
