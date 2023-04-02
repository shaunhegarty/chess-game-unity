using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIManager : MonoBehaviour
{

    public Image panelImage;
    public TextMeshProUGUI infoText;

    public void OnCheckMate()
    {
        Debug.Log("Fade the panel");
        panelImage.color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
    }

    public void UpdateInfoText(string info)
    {
        infoText.text = info;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
