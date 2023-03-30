using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    public Material white;
    public Material black;
    public Team squareTeam;

    Material currentMaterial;
    Renderer squareRenderer;

    public Vector3 BasePosition { get; private set; }

    bool highlighted = false;

    float positionHighlightOffset = 0.05f;
    float positionHighlightOffsetSpeed = 1f;

    void Start()
    {
        squareRenderer = GetComponent<Renderer>();
        BasePosition = transform.position;
        UpdateMaterials();        
    }

    public void SetTeam(Team team)
    {
        squareTeam = team;
    }

    private void UpdateMaterials()
    {
        if (squareTeam == Team.White)
        {
            currentMaterial = Instantiate(white);
        }
        else
        {
            currentMaterial = Instantiate(black);
        }

        squareRenderer.material = currentMaterial;
    }

    public void Update()
    {
        if (highlighted)
        {
            if (transform.position.y < BasePosition.y + positionHighlightOffset)
            {
                transform.position = transform.position + new Vector3(0, positionHighlightOffsetSpeed * Time.deltaTime, 0);
            }            
        } else if (transform.position.y > BasePosition.y)
        {
            transform.position = transform.position - new Vector3(0, positionHighlightOffsetSpeed * Time.deltaTime, 0);
        }
    }

    public void Highlight()
    {
        highlighted = true;
        currentMaterial.EnableKeyword("_EMISSION");        
        currentMaterial.SetColor("_EmissionColor", new Color(0, 0.3f, 0));
    }

    public void Dehighlight()
    {
        highlighted = false;
        currentMaterial.DisableKeyword("_EMISSION");
    }
}

public enum Team
{
    Black, White
}

