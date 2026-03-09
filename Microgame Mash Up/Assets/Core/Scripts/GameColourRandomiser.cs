using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePack
{
    public class GameColourRandomiser : MonoBehaviour
    {
        public List<Color> possibleColours;
        public static List<Color> colours;

        private static GameColourRandomiser instance;

        private static Color currentColour;

        private void Awake()
        {
            instance = this;
            PickNextColour();
        }

        public static void PickNextColour()
        {
            if (instance == null)
            {
                currentColour = new Color(0.15f, 0.5f, 0.4f);
                return;
            }
            
            if (colours == null || colours.Count == 0)
            {
                colours = instance.possibleColours.GetRange(0, instance.possibleColours.Count);
            }

            currentColour = colours[0];
            colours.RemoveAt(0);
        }

        public static Color GetColour()
        {
            return currentColour;
        }
    }
}
