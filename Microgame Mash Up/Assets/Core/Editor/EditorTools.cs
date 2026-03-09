#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace GamePack
{
    public static class EditorTools
    {
        public static GameMaster.Difficulty GetCustomDifficulty()
        {
            return (GameMaster.Difficulty)PlayerPrefs.GetInt("difficulty_override",0);
        }

        private static void SetDifficulty(GameMaster.Difficulty diff)
        {
            PlayerPrefs.SetInt("difficulty_override",(int)diff);
        }

        [MenuItem("Party Pack/Set Difficulty/- UNSET (Default)")]
        public static void SetDifficulty_UNSET() { SetDifficulty(GameMaster.Difficulty.UNSELECTED); }
        [MenuItem("Party Pack/Set Difficulty/1: Very Easy")]
        public static void SetDifficulty_VEASY() { SetDifficulty(GameMaster.Difficulty.VERY_EASY); }
        [MenuItem("Party Pack/Set Difficulty/2: Easy")]
        public static void SetDifficulty_EASY() { SetDifficulty(GameMaster.Difficulty.EASY); }
        [MenuItem("Party Pack/Set Difficulty/3: Normal")]
        public static void SetDifficulty_NORMAL() { SetDifficulty(GameMaster.Difficulty.NORMAL); }
        [MenuItem("Party Pack/Set Difficulty/4: Hard")]
        public static void SetDifficulty_HARD() { SetDifficulty(GameMaster.Difficulty.HARD); }
        [MenuItem("Party Pack/Set Difficulty/5: Very Hard", false)]
        public static void SetDifficulty_VHARD() { SetDifficulty(GameMaster.Difficulty.VERY_HARD); }

        
        
        [MenuItem("Party Pack/Set Difficulty/- UNSET (Default)", true)]
        public static bool isDifficulty_UNSET(){ return GetCustomDifficulty() != GameMaster.Difficulty.UNSELECTED; }
        [MenuItem("Party Pack/Set Difficulty/1: Very Easy", true)]
        public static bool isDifficulty_VEASY(){ return GetCustomDifficulty() != GameMaster.Difficulty.VERY_EASY; }
        [MenuItem("Party Pack/Set Difficulty/2: Easy", true)]
        public static bool isDifficulty_EASY(){ return GetCustomDifficulty() != GameMaster.Difficulty.EASY; }
        [MenuItem("Party Pack/Set Difficulty/3: Normal", true)]
        public static bool isDifficulty_NORMAL(){ return GetCustomDifficulty() != GameMaster.Difficulty.NORMAL; }
        [MenuItem("Party Pack/Set Difficulty/4: Hard", true)]
        public static bool isDifficulty_HARD(){ return GetCustomDifficulty() != GameMaster.Difficulty.HARD; }
        [MenuItem("Party Pack/Set Difficulty/5: Very Hard", true)]
        public static bool isDifficulty_VHARD(){ return GetCustomDifficulty() != GameMaster.Difficulty.VERY_HARD; }
        
        
    }
}

#endif