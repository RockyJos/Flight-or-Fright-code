using System.Collections;
using GamePack;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
    {
        
        //DO NOT EDIT THIS SCRIPT
        
        public enum State
        {
            NOT_STARTED,
            RUNNING,
            SUCCESS,
            FAILED
        }

        [SerializeField] private GameInfo gameInfo;
        [Space]
        private GAWGameManager gameManager;
        private State state = State.NOT_STARTED;

        private bool prefabAndInfoValid = true;
        public void SetValid(bool v)
        {
            prefabAndInfoValid = v;
        }
        public bool GetValid()
        {
            return prefabAndInfoValid;
        }
        
        private bool activate = true;
        public void SetActive(bool v)
        {
            activate = v;
        }
        public bool GetActive()
        {
            return activate;
        }

        public GameInfo GetGameInfo()
        {
            return gameInfo;
        }

        public void ResetGameInfo()
        {
            gameInfo = new GameInfo();
        }


        public State GetGameState()
        {
            return state;
        }

        GAWGameManager GetGameManager()
        {
            if (gameManager == null)
            {
                gameManager = GetComponent<GAWGameManager>();
            }
            return gameManager;
        }

        public void LoadGame()
        {
            state = State.NOT_STARTED;
            GetGameManager().OnGameLoad();
        }
        
        public void StartGame()
        {
            state = State.RUNNING;
            GetGameManager().OnGameStart();
        }

        public void GameSucceeded()
        {
            state = State.SUCCESS;
            GetGameManager().OnGameSucceeded();
        }

        public void RanOutOfTime()
        {
            state = State.FAILED;
            GetGameManager().OnGameFailed();
        }


        public bool IsValid(out string validationLog)
        {
            bool valid = true;
            string log = "";

            void CheckIfFieldValid(string fieldName, bool condition, string ifValid, string ifInvalid,
                bool required = true)
            {
                log += "\t\t- " + fieldName + ": ";
                if (condition)
                {
                    log += "<color=green>" + ifValid;
                }
                else if (required)
                {
                    log += "<color=#f01a1a>" + ifInvalid;
                    valid = false;
                }
                else
                {
                    log += "<color=yellow>" + ifInvalid;
                }

                log += "</color>\n";
            }

            CheckIfFieldValid("Camera", GetComponentInChildren<Camera>(), "FOUND", "MISSING");

            validationLog = log;
            return valid;
        }

        public GameMaster.Difficulty GetDifficulty() => GameMaster.GetDifficulty();
        public float GetTotalTime() => GameMaster.GetTotalTime();
        public float GetTimeElapsed() => GameMaster.GetTimeElapsed();
        public float GetFractionTimeElapsed() => GameMaster.GetFractionTimeElapsed();
        public float GetTimeRemaining() => GameMaster.GetTimeRemaining();
        public float GetFractionTimeRemaining() => GameMaster.GetFractionTimeRemaining();

    }
