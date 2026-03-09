using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GamePack
{
    public class GameTransition : MonoBehaviour
    {
        [HideInInspector] public bool screenCovered = false;
        public TMP_Text difficultyText;
        public TMP_Text difficultyTextOutline;

        public IEnumerator Play(int difficultyChange)
        {
            if (difficultyChange==-99){ difficultyText.text = ""; }
            else if (difficultyChange == 0) { difficultyText.text = "Faster!"; }
            else if (difficultyChange > 0) { difficultyText.text = "Difficulty Up!"; }
            else { difficultyText.text = "Difficulty Down!"; }
            difficultyTextOutline.text = difficultyText.text;

            GameColourRandomiser.PickNextColour();
            gameObject.SetActive(true);
            screenCovered = false;
            yield return new WaitUntil((() => screenCovered));
        }

        public void SetScreenCovered()
        {
            screenCovered = true;
        }

        public void AnimationComplete()
        {
            gameObject.SetActive(false);
        }

    }
}