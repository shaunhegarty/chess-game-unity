using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquare : MonoBehaviour
{

    Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    private void OnMouseEnter()
    {
        Highlight();
    }

    public void Highlight()
    {
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", new Color(0, 0.3f, 0));
    }

    public void Dehighlight()
    {
        material.DisableKeyword("_EMISSION");
    }

    private void OnMouseExit()
    {
        Dehighlight();
    }
}
