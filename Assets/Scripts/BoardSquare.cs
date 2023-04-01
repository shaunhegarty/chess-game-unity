using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    // Game Objects
    public TextMesh IndexText;

    // Settings
    public Material white;
    public Material black;
    public Team squareTeam;

    public Vector2Int Index;

    private Color targetingColor = new(0, 0.5f, 0);
    private Color candidateColor = new(0, 0.5f, 0.5f);
    private Color originalColour;

    // Components
    Material currentMaterial;
    Renderer squareRenderer;

    // State
    public Vector3 BasePosition { get; private set; }
    bool movementTarget = false;
    bool movementCandidate = false;    
    public GamePiece occupant;

    // properties
    public int Column { get { return Index.y; } }
    public char ColumnLetter { get { return (char)('A' + Index.y); } }
    public int Row { get { return Index.x + 1; } }


    void Start()
    {
        squareRenderer = GetComponent<Renderer>();
        BasePosition = transform.position;
        UpdateMaterials();        
    }

    public void Update()
    {
        OffsetIfHighlighted();
    }

    public void SetTeam(Team team)
    {
        squareTeam = team;
    }

    public void SetIndex(Vector2Int index)
    {
        Index = index;
        IndexText.text = $"{ColumnLetter}{Row}";
    }

    public void SetOccupant(GamePiece piece)
    {
        occupant = piece;
    }

    public void SetBasePosition(Vector3 position)
    {
        BasePosition = position;
    }

    private void UpdateMaterials()
    {
        currentMaterial = squareTeam == Team.White ? Instantiate(white) : Instantiate(black);
        originalColour = currentMaterial.GetColor("_Color");
        squareRenderer.material = currentMaterial;
    }

    /* Highlight, colour and offset */
    float positionHighlightOffset = 0.05f;
    float positionHighlightOffsetSpeed = 1f;

    private void OffsetIfHighlighted()
    {
        if (movementTarget)
        {
            if (transform.position.y < BasePosition.y + positionHighlightOffset)
            {
                transform.position = transform.position + new Vector3(0, positionHighlightOffsetSpeed * Time.deltaTime, 0);
            }
        }
        else if (transform.position.y > BasePosition.y)
        {
            transform.position = transform.position - new Vector3(0, positionHighlightOffsetSpeed * Time.deltaTime, 0);
        }
    }

    public void SetAsTarget(bool isTarget)
    {
        movementTarget = isTarget;
        UpdateHighlight();
    }


    public void SetAsCandidate(bool isCandidate)
    {
        movementCandidate = isCandidate;
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        if (movementTarget || movementCandidate)
        {
            currentMaterial.EnableKeyword("_EMISSION");
        } else
        {
            currentMaterial.DisableKeyword("_EMISSION");
            currentMaterial.SetColor("_Color", originalColour);
            return;
        }
        
        if (movementTarget)
        {
            currentMaterial.SetColor("_EmissionColor", targetingColor);
        } else if (movementCandidate)
        {            
            currentMaterial.SetColor("_Color", originalColour * 0.8f);            
            currentMaterial.SetColor("_EmissionColor", candidateColor);
        }
        
    }
}

public enum Team
{
    Black, White
}

