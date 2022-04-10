using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{
    [SerializeField] private Image loadingBar = null;

    private void Awake()
    {
        Assert.IsNotNull(loadingBar);
    }

    private void Start()
    {
        StartCoroutine(LoadAsyncOperation());
    }

    private IEnumerator LoadAsyncOperation()
    {
        // PERFORMANCE: This jumps from 0 to 0.9 and then freezes while the code runs. See if I can pre-generate all of the plantz randomly so the work isn't done when the game starts
        AsyncOperation loadingProgress = SceneManager.LoadSceneAsync(ConstantValues.Scenes.Outdoors);

        while (!loadingProgress.isDone)
        {
            loadingBar.fillAmount = loadingProgress.progress;
            yield return new WaitForEndOfFrame();
        }
    }
}

