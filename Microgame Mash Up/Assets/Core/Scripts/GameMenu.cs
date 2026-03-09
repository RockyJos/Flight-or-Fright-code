using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GamePack
{
    public class GameMenu : MonoBehaviour
    {
        public Button startGameButton;
        public Transform gameEntryParent;
        public GameMenuEntry entryPrefab;
        public InvalidGameMenuEntry invalidEntryPrefab;

        public GameInfoPanel gameInfoPanel;
        public InvalidGameInfoPanel invalidGameInfoPanel;
        
        private void Start()
        {
            PopulateGameList(GameLibrary.GetAllGamePrefabs());
            Deselect();
            CheckIfCanStartGame();
        }

        public void PopulateGameList(Game[] games)
        {
            foreach (Transform t in gameEntryParent)
            {
                Destroy(t.gameObject);
            }

            foreach (Game g in games)
            {
                if (g.GetValid())
                {
                    GameMenuEntry gme = Instantiate(entryPrefab, gameEntryParent);
                    gme.SetInfo(g, this);
                }
                else
                {
                    InvalidGameMenuEntry gme = Instantiate(invalidEntryPrefab, gameEntryParent);
                    gme.SetInfo(g.GetGameInfo(), this);
                }
            }
        }

        public void Select(GameInfo gi)
        {
            gameInfoPanel.View(gi);
            invalidGameInfoPanel.View(null);
        }

        public void SelectInvalid(GameInfo gi)
        {
            gameInfoPanel.View(null);
            invalidGameInfoPanel.View(gi);
        }

        public void Deselect()
        {
            gameInfoPanel.View(null);
            invalidGameInfoPanel.View(null);
        }

        public void CheckIfCanStartGame()
        {
            int validGameCount = 0;
            foreach (Game g in GameLibrary.GetAllGamePrefabs())
            {
                if (g.GetValid() && g.GetActive())
                {
                    validGameCount++;
                }
            }

            startGameButton.interactable = validGameCount > 0;
        }

    }   
}
