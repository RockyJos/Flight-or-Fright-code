using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePack
{
    public class InvalidGameMenuEntry : MonoBehaviour
    {
        private GameMenu gameMenu;
        private GameInfo gameInfo = null;
        public TMP_Text title;
        public Image icon;
        
        public void SetInfo(GameInfo gi, GameMenu gm)
        {
            gameInfo = gi;
            gameMenu = gm;
            title.text = gi.gameName.Length==0?"---": gi.gameName;
            icon.sprite = gi.icon;
        }
        public void Select()
        {
            gameMenu.SelectInvalid(gameInfo);
        }
    }
}