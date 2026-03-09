using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePack;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public enum Difficulty
    {
        UNSELECTED,
        VERY_EASY,
        EASY,
        NORMAL,
        HARD,
        VERY_HARD
    }

    [SerializeField] private GameOverlay gameOverlay;
    private static GameMaster instance = null;
    private Coroutine currentGameRoutine = null;
    private Game currentGame;
    private Difficulty currentDifficulty = Difficulty.VERY_EASY;

    private int startingLives = 10;
    private float gameIntroLength = 3;
    private float gameOutroLength = 2;
    private bool allowEarlySuccess = true;
    private int gamesPlayed = 0;
    private int gamesPlayedAtDifficultyLevel = 0;
    private int gamesPerDifficulty = 5;

    public GameOverScreen gamveOverPrefab;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        Log("Starting game pack using <color=green><b>"+GameLibrary.GetValidGamePrefabs().Count+"</b></color> valid game(s).");
        gameOverlay.LifeCounter.SetLives(startingLives);
        StartNewGame();
    }

    private void StartNewGame()
    {
        if (currentGameRoutine == null)
        {
            currentGameRoutine = StartCoroutine(RunNextGame());
        }
    }

    IEnumerator RunNextGame()
    {
        currentGame = Instantiate(GameLibrary.GetNextGamePrefab());
        gameOverlay.Timer.Restart(GetTotalTime());
        gameOverlay.NoCameraOverlay.SetActive(!currentGame.GetComponentInChildren<Camera>());
        gameOverlay.GameName.text = currentGame.GetGameInfo().gameName;
        gameOverlay.AuthorCredit.text = currentGame.GetGameInfo().authorCredit;
        
        currentGame.LoadGame();
        Log("Starting game \"<color=green>"+currentGame.GetGameInfo().gameName+"</color>\" at <color=green>"+currentDifficulty+"</color> difficulty, with <color=green>"+GetTotalTime()+"</color> seconds on the clock.");
        gameOverlay.GameIntro.Play(currentGame.GetGameInfo().introText1,currentGame.GetGameInfo().introText2,currentGame.GetGameInfo().introText3);
        yield return new WaitForSeconds(gameIntroLength);
        currentGame.StartGame();
        yield return gameOverlay.Timer.Play(allowEarlySuccess);

        bool shouldLoseLife = false;
        if (currentGame.GetGameState() != Game.State.SUCCESS)
        {
            //must have ran out of time
            currentGame.RanOutOfTime();
            shouldLoseLife = true;
        }

        gamesPlayed++;
        gamesPlayedAtDifficultyLevel++;

        
        int difficultyChange = 0;
        
#if UNITY_EDITOR
        if (EditorTools.GetCustomDifficulty() == Difficulty.UNSELECTED)
        {
#endif
            if (gamesPlayedAtDifficultyLevel >= gamesPerDifficulty)
            {
                difficultyChange++;
            }

            if (difficultyChange != 0 && difficultyChange + (int) GetDifficulty() <= (int) Difficulty.VERY_HARD &&
                difficultyChange + (int) GetDifficulty() >= (int) Difficulty.VERY_EASY)
            {
                gamesPlayedAtDifficultyLevel = 0;
                currentDifficulty = GetDifficulty() + difficultyChange;
                
                Log("Difficulty changed to: <color=green>"+currentDifficulty+"</color>");
            }
            else
            {
                difficultyChange = 0; // enforce no difficulty change if already at max or minimum difficulty
            }
#if UNITY_EDITOR
        }
#endif

        if (shouldLoseLife && gameOverlay.LifeCounter.GetLivesRemaining() <= 1)
        {
            difficultyChange = -99;
        }
        yield return new WaitForSeconds(gameOutroLength); // allow closing animation
        yield return gameOverlay.GameTransition.Play(difficultyChange); //returns when screen is covered
        
        gameOverlay.Timer.gameObject.SetActive(false);
        yield return null;  //toggle timer over a frame so new colours get picked up
        gameOverlay.Timer.gameObject.SetActive(true);
        
        if (shouldLoseLife)
        {
            gameOverlay.LifeCounter.RemoveLife(); // lose life during transition
        }

        if (gameOverlay.LifeCounter.GetLivesRemaining() == 0)
        {
            Destroy(currentGame.gameObject);
            currentGameRoutine = null;
            GameOverScreen gos = Instantiate(gamveOverPrefab);
            yield return new WaitForSeconds(3);
            gos.Populate();
        }
        else
        {
            Destroy(currentGame.gameObject);
            currentGameRoutine = null;
            StartNewGame();
        }
    }

    // PUBLIC FACING METHODS AND PROPERTIES
    public static float GetTimeElapsed(){ return instance==null?0: GetTotalTime() - instance.gameOverlay.Timer.GetTimeRemaining(); }
    public static float GetFractionTimeElapsed() { return instance==null?0: GetTimeElapsed() / GetTotalTime(); }
    public static float GetTimeRemaining(){ return instance==null?10: instance.gameOverlay.Timer.GetTimeRemaining(); }
    public static float GetFractionTimeRemaining(){ return instance==null?1f: instance.gameOverlay.Timer.GetTimeRemaining()/GetTotalTime(); }
    public static Game CurrentGame { get { return instance != null ? instance.currentGame : null; } }
    public static Game.State GetGameState()
    { return (instance == null || instance.currentGame == null) ? Game.State.NOT_STARTED : instance.currentGame.GetGameState(); }
    public static void GameSucceeded()
    {
        if (instance!=null && instance.currentGame != null)
        {
            instance.currentGame.GameSucceeded();
        }
    }
    public static float GetTotalTime() { return  (instance == null)?15:(20-Mathf.Clamp((instance.gamesPlayedAtDifficultyLevel*(10f/(instance.gamesPerDifficulty-1))),0,10)); }
    public static Difficulty GetDifficulty()
    {
#if UNITY_EDITOR
        return EditorTools.GetCustomDifficulty()!=Difficulty.UNSELECTED? EditorTools.GetCustomDifficulty() : ((instance == null) ? Difficulty.VERY_EASY : instance.currentDifficulty);
#endif
        return ((instance == null) ? Difficulty.VERY_EASY : instance.currentDifficulty);
    }

    public static void Log(object o)
    {
        Debug.Log("<color=yellow><b>GAME MASTER</b></color>: "+o);
    }
}

