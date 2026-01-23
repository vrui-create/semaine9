using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName, Action onLoaded = null)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, onLoaded));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, Action onLoaded)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = true;

        while (!op.isDone)
            yield return null;

        yield return null;

        onLoaded?.Invoke();
    }
}
