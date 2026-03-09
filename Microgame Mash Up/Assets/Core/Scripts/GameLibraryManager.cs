using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePack
{
    public static class GameLibrary
    {
        private static List<Game> gamePrefabs = new List<Game>();
        private static Queue<Game> gameQueue = new Queue<Game>();
        
        public static Game[] GetAllGamePrefabs()
        {
            if (gamePrefabs.Count == 0)
            {
                LoadGames();
            }
            return gamePrefabs.ToArray();
        }
        
        public static Game GetNextGamePrefab()
        {
            if (gameQueue.Count == 0)
            {
                gameQueue = new Queue<Game>(ShuffleList(GetValidGamePrefabs()));
            }

            return gameQueue.Dequeue();
        }

        public static List<Game> GetValidGamePrefabs()
        {
            if (gamePrefabs.Count == 0)
            {
                LoadGames();
                if (gamePrefabs.Count == 0)
                {
                    //no valid games found
                    Debug.LogError("No valid games found to play.");
                    return null;
                }
            }

            List<Game> validGames = new List<Game>();
            foreach (Game g in gamePrefabs)
            {
                if (g.GetValid() && g.GetActive())
                {
                    validGames.Add(g);
                }
            }

            return validGames;
        }

        private static List<Game> ShuffleList(List<Game> list)
        {
            List<Game> shuffled = new List<Game>(list);
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
            }
            return shuffled;
        }

        private static void LoadGames()
        {
            StringBuilder outputLog = new StringBuilder();
            gamePrefabs.Clear();
            Game[] prefabs = Resources.LoadAll<Game>("");
            bool foundInvalidPrefab = false;
            foreach (Game g in prefabs)
            {
                bool gameValid = true;
                outputLog.AppendLine("Validating Game: " + g.gameObject.name);
                if (g.GetGameInfo().IsValid(out string infoValidationLog))
                {
                    outputLog.AppendLine("\t- <color=green>Valid Game Info</color>");
                    outputLog.Append(infoValidationLog);
                }
                else
                {
                    outputLog.AppendLine("\t- <color=#f01a1a>Invalid Game Info</color>");
                    outputLog.Append(infoValidationLog);
                    foundInvalidPrefab = true;
                    gameValid = false;
                }

                if (g.IsValid(out string prefabValidationLog))
                {
                    outputLog.AppendLine("\t- <color=green>Valid Prefab</color>");
                    outputLog.AppendLine(prefabValidationLog + "\n");
                }
                else
                {
                    outputLog.AppendLine("\t- <color=#f01a1a>Invalid Prefab</color>");
                    outputLog.AppendLine(prefabValidationLog + "\n");
                    foundInvalidPrefab = true;
                    gameValid = false;
                }

                g.SetValid(gameValid);    
                gamePrefabs.Add(g);
            }

            if (foundInvalidPrefab)
            {
                outputLog.Insert(0, "<color=#f01a1a>Game Load Incomplete</color>\n");
                Debug.LogError(outputLog);
            }
            else
            {
                outputLog.Insert(0, "<color=green>Game Load Complete</color>\n");
                Debug.Log(outputLog);

                if (gamePrefabs.Count > 1)
                {
                    for (int i = gamePrefabs.Count - 1; i >= 0 && gamePrefabs.Count > 1; i--)
                    {
                        if (gamePrefabs[i].name.Contains("Example"))
                        {
                            //gamePrefabs.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
}
