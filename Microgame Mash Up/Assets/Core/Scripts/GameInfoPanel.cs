using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePack
{
    public class GameInfoPanel : MonoBehaviour
    {
        public TMP_Text title;
        public TMP_Text titleShadow;
        public TMP_Text introWords;
        public TMP_Text introWordsShadow;
        public TMP_Text description;

        public Image keyArt;

        public void View(GameInfo info)
        {
            if (info != null)
            {
                gameObject.SetActive(true);

                title.text = info.gameName;
                titleShadow.text = info.gameName;
                introWords.text = info.introText1 + " " + info.introText2 + " " + info.introText3 + "!";
                introWordsShadow.text = introWords.text;
                description.text = info.description;
            }
            else
            {
                gameObject.SetActive(false);
            }

        }

    }
}