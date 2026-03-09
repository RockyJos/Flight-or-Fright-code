using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GamePack
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private Image timerBarFill;
        
        private float timeRemaining = 20;
        private float maxTimeRemaining = 20;

        public float GetTimeRemaining()
        {
            return timeRemaining;
        }

        public void Restart(float time)
        {
            timeRemaining = time;
            maxTimeRemaining = time;
            Tick(0);
        }

        public void Tick(float deltaTime)
        {
            timeRemaining = Mathf.Max(timeRemaining - deltaTime, 0);
            timerBarFill.fillAmount = 1f - (timeRemaining/maxTimeRemaining);
        }

        public IEnumerator Play(bool allowEarlySuccess)
        {
            while (timeRemaining > 0)
            {
                if (allowEarlySuccess && GameMaster.CurrentGame.GetGameState() == Game.State.SUCCESS)
                {
                    break;
                }
                Tick(Time.deltaTime);
                yield return null;
            }
        }
    }
}
