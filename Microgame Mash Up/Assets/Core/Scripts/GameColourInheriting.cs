using System;
using UnityEngine;
using UnityEngine.UI;

namespace GamePack
{
    public class GameColourInheriting : MonoBehaviour
    {
        public Color baseColour;

        private void Start()
        {
            OnEnable();
        }

        private void OnEnable()
        {
            if (TryGetComponent(out Image image))
            {
                image.color = GameColourRandomiser.GetColour() * baseColour;
            }
            else if (TryGetComponent(out SpriteRenderer sr))
            {
                sr.color = GameColourRandomiser.GetColour() * baseColour;
            }
        }
    }
}
