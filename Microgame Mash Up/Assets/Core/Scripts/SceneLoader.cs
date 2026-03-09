using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    private static SceneTransition currentTransition;


    public static void Load(string sceneName)
    {
        if (currentTransition == null)
        {
            currentTransition = Instantiate(Resources.Load<SceneTransition>("Core/Transition"));
            currentTransition.sceneName = sceneName;
        }
    }

    
    public void LoadScene(string sceneName)
    {
        Load(sceneName);
    }
}
