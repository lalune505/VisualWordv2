using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;
    public CanvasGroup loadingGroup;
    private const string MainScenePath = "Assets/VisualWord/Scenes/MainScene.unity";
    private const string FirstScenePath = "Assets/VisualWord/Scenes/FirstScene.unity";
    private string _currentInput = "";
   
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
      
        DontDestroyOnLoad(gameObject);
    }

    public void Load3DSceneWithFade()
    {
        _currentInput = FindObjectOfType<CanvasManager>().GetInputFromInputField();
        StartCoroutine(Load3DSceneWithFade(MainScenePath));
    }
    public void LoadFirstScene()
    { 
        StartCoroutine(Load3DSceneWithFade(FirstScenePath));
    }
    IEnumerator Load3DSceneWithFade(string scenePath)
    {
        loadingGroup.gameObject.SetActive(true);
        yield return StartCoroutine(FadeLoadingScreen(loadingGroup,1, 2f ));
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(scenePath);
        while (!operation.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(FadeLoadingScreen(loadingGroup,0, 0.6f));
        loadingGroup.gameObject.SetActive(false);
    }
    
    IEnumerator FadeLoadingScreen(CanvasGroup canvasGroup, float targetValue, float duration)
    {
        float startValue = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startValue, targetValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = targetValue;
    }

    public string GetCurrentInput()
    {
        return _currentInput;
    }
}
