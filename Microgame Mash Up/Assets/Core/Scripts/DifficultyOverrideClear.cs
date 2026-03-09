using System;
using UnityEngine;

namespace GamePack
{
    public class DifficultyOverrideClear : MonoBehaviour
    {
        public GameObject button;

        private void Update()
        {
            #if UNITY_EDITOR
               button.SetActive(EditorTools.GetCustomDifficulty()!=GameMaster.Difficulty.UNSELECTED);
            #endif
        }
        public void ClearOverride()
        {
            #if UNITY_EDITOR
                EditorTools.SetDifficulty_UNSET();
            #endif
        }
    }
}