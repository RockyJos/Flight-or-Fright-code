using TMPro;
using UnityEngine;

public class GameManager : GAWGameManager
{
    public TMP_Text stateText;
    public TMP_Text gameText;

    private int spaceCount = 0;

    private int countRequired = 50;
    
    private void Update()
    {
        stateText.text = "Game State: "+GameMaster.GetGameState() + ", Time remaining: "+(int)GameMaster.GetTimeRemaining()+"s";
        
        if (GameMaster.GetGameState() == Game.State.RUNNING)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                spaceCount++;

                gameText.text = "Press Space " + (countRequired - spaceCount) + " more times!";
                
                if (spaceCount >= countRequired)
                {
                    GameMaster.GameSucceeded();
                }
            }
            
        }
    }

    public override void OnGameLoad()
    {
        gameText.text = "Ready...";
        switch (GameMaster.GetDifficulty())
        {
            case GameMaster.Difficulty.VERY_EASY: 
                countRequired = 10;
                break;
            case GameMaster.Difficulty.EASY: 
                countRequired = 20;
                break;
            case GameMaster.Difficulty.NORMAL: 
                countRequired = 30;
                break;
            case GameMaster.Difficulty.HARD: 
                countRequired = 40;
                break;
            case GameMaster.Difficulty.VERY_HARD: 
                countRequired = 50;
                break;
        }
    }

    public override void OnGameStart()
    {
        gameText.text = "Press Space "+countRequired+" times!";
    }

    public override void OnGameSucceeded()
    {
        gameText.text = "Space was pressed "+countRequired+" times in "+(int)GameMaster.GetTimeElapsed()+" seconds.";
    }

    public override void OnGameFailed()
    {
        gameText.text = "Space was only pressed "+spaceCount+" times!";
    }

}
