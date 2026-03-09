using TMPro;
using UnityEngine;

namespace GamePack
{
    public class InvalidGameInfoPanel : MonoBehaviour
    {
        public TMP_Text title;
        public TMP_Text titleShadow;
        public TMP_Text errorLog;

        public void View(GameInfo gi)
        {
            if (gi != null)
            {
                gameObject.SetActive(true);
                title.text = gi.gameName.Length==0?"---": gi.gameName;
                titleShadow.text = title.text;
                string e = "";
                gi.IsValid(out e);
                errorLog.text = "Incomplete Game Prefab:\n"+e;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}