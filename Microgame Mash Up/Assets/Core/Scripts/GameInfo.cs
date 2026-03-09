using System;
using UnityEngine;

namespace GamePack
{
    [Serializable]
    public class GameInfo
    {
        
        //DO NOT EDIT THIS SCRIPT
        
        
        public string gameName;
        public string authorCredit;
        public string introText1;
        public string introText2;
        public string introText3;
        [TextArea] public string description;

        [Header("Images")] 
        public Sprite icon;
        public Sprite keyArt;

        public bool IsValid(out string validationLog)
        {
            bool valid = true;
            string log = "";
            void CheckIfFieldValid(string fieldName, bool condition, string ifValid, string ifInvalid, bool required = true)
            {
                log += "\t\t- " + fieldName+": ";
                if (condition)
                {
                    log += "<color=green>"+ifValid;
                }
                else if(required)
                {
                    log += "<color=#f01a1a>"+ifInvalid;
                    valid = false;
                }
                else
                {
                    log += "<color=yellow>"+ifInvalid;
                }
                log += "</color>\n";
            }

            CheckIfFieldValid("Game name", gameName.Length>0 ,gameName, "MISSING");
            CheckIfFieldValid("Author credit", authorCredit.Length>0 ,authorCredit, "MISSING");
            CheckIfFieldValid("Intro Text 1", introText1.Length>0 ,introText1, "MISSING");
            CheckIfFieldValid("Intro Text 2", introText2.Length>0 ,introText2, "MISSING");
            CheckIfFieldValid("Intro Text 3", introText3.Length>0 ,introText3, "MISSING");
            CheckIfFieldValid("Description", description.Length>10&&description.Length<200 ,description.Substring(0,Mathf.Min(description.Length,200)), "TOO LONG/SHORT");

            CheckIfFieldValid("Icon", icon!=null ,"LOADED", "MISSING", false);
            CheckIfFieldValid("Key Art", keyArt!=null ,"LOADED", "MISSING", false);

            validationLog = log;
            return valid;
        }
    }
}
