using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Grid[] grids;

    public enum Side
    {
        None,
        X,
        O
    }
    public Side playerSide;
    public Side opponentSide;

    enum Turn
    {
        Player,
        Opponent
    }
    Turn gameTurn;
    
    int[,] winningCondition = new int[,]
    {
        {0,1,2},
        {3,4,5},
        {6,7,8},
        {0,3,6},
        {1,4,7},
        {2,5,8},
        {0,4,8},
        {2,4,6},
    };
    int moveCount;

    public GameObject gameOverPanel;
    public Text gameOverText;

	// Use this for initialization
	void Start ()
    {
        StartGame();
    }

    void StartGame()
    {
        if (opponentSide == Side.X)
        {
            gameTurn = Turn.Opponent;
            OpponentMove();
        }
        else
            gameTurn = Turn.Player;

        gameOverPanel.SetActive(false);
    }

    void OpponentMove()
    {
        int index = FindBestMove();
        grids[index].SetGridSpace();
    }

    int FindEmptyGrid()
    {
        for (int i = 0; i < grids.Length; i++)
        {
            if (grids[i].gridStatus == Side.None)
                return i;
        }

        return 0;
    }

    public Side GetCurrentTurn()
    {
        if (gameTurn == Turn.Player)
            return playerSide;
        else
            return opponentSide;
    }

    public void EndTurn(Side currentTurn)
    {
        moveCount++;
        // Check win condition
        for (int i = 0; i < winningCondition.GetLength(0); i++)
        {
            if(grids[winningCondition[i, 0]].gridStatus == currentTurn &&
               grids[winningCondition[i, 1]].gridStatus == currentTurn &&
               grids[winningCondition[i, 2]].gridStatus == currentTurn)
            {
                GameOver(currentTurn.ToString() + " Wins");
                return;
            }
        }

        if(moveCount == grids.Length)
        {
            GameOver("It's a draw");
            return;
        }
                
        ChangeTurn();
    }

    void GameOver(string text)
    {
        SetButtonInteractable(false);
        gameOverText.text = text;
        gameOverPanel.SetActive(true);
    }

    void SetButtonInteractable(bool value)
    {
        for (int i = 0; i < grids.Length; i++)
        {
            grids[i].button.interactable = value;
        }
    }
	
    void ChangeTurn()
    {
        if (gameTurn == Turn.Player)
        {
            gameTurn = Turn.Opponent;
            OpponentMove();
        }
        else
            gameTurn = Turn.Player;
    }

    public void RestartGame()
    {
        // Renable all buttons
        SetButtonInteractable(true);
        // Hide game over panel
        gameOverPanel.SetActive(false);
        // Reset move count
        moveCount = 0;
        // Reset all grid data
        for (int i = 0; i < grids.Length; i++)
        {
            grids[i].gridStatus = 0;
            grids[i].buttonText.text = "";
        }

        StartGame();
    }

    int FindBestMove()
    {
        Grid[] gridsClone = grids;
        int bestValue = 1000;
        int bestGrid = -1;

        for (int i = 0; i < gridsClone.Length; i++)
        {
            if(gridsClone[i].gridStatus == Side.None)
            {
                // Insert first simulation
                gridsClone[i].gridStatus = opponentSide;
                int moveValue = Minimax(gridsClone, 0, true);
                print("board index " + i + "worth " + moveValue);
                gridsClone[i].gridStatus = Side.None;

                if(moveValue < bestValue)
                {
                    bestValue = moveValue;
                    bestGrid = i;
                }
            }
        }

        return bestGrid;
    }

    int Minimax(Grid[] pGrids, int depth, bool isMax)
    {
        //if udah mentok / ada yg menang, return something
        int score = EvaluateBoard(pGrids, depth);
        if (score >= -10 && score <= 10 && score != 0)
            return score;

        if (score == 0 && HasEmptySpace(pGrids) == false)
            return score;

        if(isMax)
        {
            int bestValue = -1000;
            for (int i = 0; i < pGrids.Length; i++)
            {
                if(pGrids[i].gridStatus == Side.None)
                {
                    pGrids[i].gridStatus = playerSide;
                    bestValue = Mathf.Max(bestValue, Minimax(pGrids, depth + 1, false));
                    pGrids[i].gridStatus = Side.None;
                }
            }
            return bestValue;
        }
        else
        {
            int bestValue = 1000;
            for (int i = 0; i < pGrids.Length; i++)
            {
                if (pGrids[i].gridStatus == Side.None)
                {
                    pGrids[i].gridStatus = opponentSide;
                    bestValue = Mathf.Min(bestValue, Minimax(pGrids, depth + 1, true));
                    pGrids[i].gridStatus = Side.None;
                }
            }
            return bestValue;
        }
    }

    bool IsWinningCondition(Grid[] pGrids, Side currentTurn)
    {
        for (int i = 0; i < winningCondition.GetLength(0); i++)
        {
            if (pGrids[winningCondition[i, 0]].gridStatus == currentTurn &&
                pGrids[winningCondition[i, 1]].gridStatus == currentTurn &&
                pGrids[winningCondition[i, 2]].gridStatus == currentTurn)
            {
                return true;
            }
        }
        return false;
    }

    int EvaluateBoard(Grid[] pGrids, int depth)
    {
        if (IsWinningCondition(pGrids, playerSide))
            return 10 - depth;
        else if (IsWinningCondition(pGrids, opponentSide))
            return -10 + depth;
        else
            return 0;
    }

    bool HasEmptySpace(Grid[] pGrid)
    {
        for (int i = 0; i < pGrid.Length; i++)
        {
            if (pGrid[i].gridStatus == Side.None)
                return true;
        }
        return false;
    }
}
