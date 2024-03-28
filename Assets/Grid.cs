using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public GameController gameController;
    public GameController.Side gridStatus;
    public Button button;
    public Text buttonText;
    
    public void SetGridSpace()
    {
        button.interactable = false;
        gridStatus = gameController.GetCurrentTurn();
        buttonText.text = gridStatus.ToString();
        gameController.EndTurn(gridStatus);
    }
}
