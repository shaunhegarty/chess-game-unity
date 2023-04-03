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
    public RectTransform PawnPromotionDialog;
    public Selectable PawnPromoter;

    private Chess.Piece pawnForPromotion;
    private PieceType promotion = PieceType.Queen;
    private readonly List<PieceType> promotionPieces = new()
    {
        PieceType.Queen,
        PieceType.Bishop,
        PieceType.Knight,
        PieceType.Rook
    };

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

    public void SetPawnForPromotion(Chess.Piece pawn)
    {
        PawnPromotionDialog.gameObject.SetActive(true);
        TMP_Dropdown dropdown = PawnPromoter.gameObject.GetComponent<TMP_Dropdown>();
        dropdown.value = promotionPieces.IndexOf(promotion);
        Debug.Log($"Selecting {pawn} for promotion");
        pawnForPromotion = pawn;
    }

    public void SetPromotion(int selectedPromotion)
    {
        promotion = promotionPieces[selectedPromotion];
        Debug.Log($"Promotion is now {promotion}");
    }

    public void PromotePawn()
    {

        if(pawnForPromotion != null)
        {
            var newPiece = MainManager.Instance.ChessManager.Game.PromotePawn(pawnForPromotion, promotion);
            MainManager.Instance.ChessManager.SpawnPiece(newPiece);
            Debug.Log($"Promoted to {promotion}");
        }
        
        PawnPromotionDialog.gameObject.SetActive(false);
    }


}
