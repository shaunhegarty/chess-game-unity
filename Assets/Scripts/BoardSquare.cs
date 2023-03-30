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
    // Start is called before the first frame update
    void Start()
    {
        squareRenderer = GetComponent<Renderer>();
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

    public void Highlight()
    {
        currentMaterial.EnableKeyword("_EMISSION");
        currentMaterial.SetColor("_EmissionColor", new Color(0, 0.3f, 0));
    }

    public void Dehighlight()
    {
        currentMaterial.DisableKeyword("_EMISSION");
    }
}

public enum Team
{
    Black, White
}

