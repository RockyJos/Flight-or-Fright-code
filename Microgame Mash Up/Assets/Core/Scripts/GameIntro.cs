using TMPro;
using UnityEngine;

namespace GamePack
{
    public class GameIntro : MonoBehaviour
    {
        public TMP_Text introText1;
        public TMP_Text introText2;
        public TMP_Text introText3;
        public TMP_Text introText1_2;
        public TMP_Text introText2_2;
        public TMP_Text introText3_2;

        public void Play(string it1, string it2, string it3)
        {
            introText1.text = it1.ToUpper();
            introText2.text = it2.ToUpper();
            introText3.text = it3.ToUpper()+"!";
            introText1_2.text = it1.ToUpper();
            introText2_2.text = it2.ToUpper();
            introText3_2.text = it3.ToUpper()+"!";
            gameObject.SetActive(true);
        }
    }
}