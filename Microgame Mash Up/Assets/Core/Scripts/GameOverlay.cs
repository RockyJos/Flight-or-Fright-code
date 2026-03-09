using GamePack;
using TMPro;
using UnityEngine;

namespace GamePack {
    public class GameOverlay : MonoBehaviour
    {
        [SerializeField] private Timer timer;
        public Timer Timer { get { return timer; } }

        [SerializeField] private GameObject noCameraOverlay;
        public GameObject NoCameraOverlay { get { return noCameraOverlay; } }
        
        [SerializeField] private LifeCounter lifeCounter;
        public LifeCounter LifeCounter { get { return lifeCounter; } }

        [SerializeField] private TMP_Text gameName;
        public TMP_Text GameName { get { return gameName; } }

        [SerializeField] private TMP_Text authorCredit;
        public TMP_Text AuthorCredit { get { return authorCredit; } }
        
        [SerializeField] private GameTransition gameTransition;
        public GameTransition GameTransition { get { return gameTransition; } }
        
        [SerializeField] private GameIntro gameIntro;
        public GameIntro GameIntro { get { return gameIntro; } }
    }
}