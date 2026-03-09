using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePack
{
    public class GameMenuEntry : MonoBehaviour
    {
        private GameMenu gameMenu;
        private Game game = null;
        public TMP_Text title;
        public Image icon;
        public Toggle toggle;
        
        public void SetInfo(Game g, GameMenu gm)
        {
            game = g;
            gameMenu = gm;
            title.text = g.GetGameInfo().gameName;
            icon.sprite = g.GetGameInfo().icon;
            toggle.isOn = g.GetActive();
        }

        public void Select()
        {
            gameMenu.Select(game.GetGameInfo());
        }

        public void ToggleActive(bool active)
        {
            game.SetActive(active);
            gameMenu.CheckIfCanStartGame();
        }
    }
}