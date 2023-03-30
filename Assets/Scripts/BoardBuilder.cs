using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBuilder : MonoBehaviour
{
    public GameObject squarePrefab;
    private BoxCollider squareCollider;
    private List<List<BoardSquare>> allSquares = new();

    // Start is called before the first frame update
    void Start()
    {
        squareCollider = squarePrefab.GetComponent<BoxCollider>();
        BuildBoard(8);
    }

    void BuildBoard(int size)
    {
        for (int i = 0; i < size; i++)
        {
            allSquares.Add(BuildRow(size, i));
        }
    }

    List<BoardSquare> BuildRow(int size, int rowIndex)
    {
        List<BoardSquare> row = new();
        Vector3 squareSize = squareCollider.size;
        for (int i = rowIndex; i < size + rowIndex; i++)
        {
            Vector3 position = new((i - rowIndex) * squareSize.x, squarePrefab.transform.position.y, rowIndex * squareSize.z);
            GameObject square = Instantiate(squarePrefab, position, squarePrefab.transform.rotation);
            BoardSquare bSquare = square.GetComponent<BoardSquare>();
            if (i % 2 == 0)
            {
                bSquare.SetTeam(Team.Black);
            }
            else
            {
                bSquare.SetTeam(Team.White);
            }
            row.Add(bSquare);
        }
        return row;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
