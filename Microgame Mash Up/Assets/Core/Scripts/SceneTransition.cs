using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [HideInInspector] public string sceneName = "Titlescreen";
    
    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(1);
        Destroy(gameObject);
        
    }
}
