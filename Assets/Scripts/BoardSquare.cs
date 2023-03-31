using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    // Settings
    public Material white;
    public Material black;
    public Team squareTeam;

    public Vector2Int Index;

    // Components
    Material currentMaterial;
    Renderer squareRenderer;

    // State
    public Vector3 BasePosition { get; private set; }
    bool movementTarget = false;
    bool movementCandidate = false;


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
    }

    private void UpdateMaterials()
    {
        currentMaterial = squareTeam == Team.White ? Instantiate(white) : Instantiate(black);
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
            return;
        }
        
        if (movementTarget)
        {
            currentMaterial.SetColor("_EmissionColor", new Color(0, 0.3f, 0));
        } else if (movementCandidate)
        {
            currentMaterial.SetColor("_EmissionColor", new Color(0, 0.3f, 0.3f));
        }
        
    }
}

public enum Team
{
    Black, White
}

