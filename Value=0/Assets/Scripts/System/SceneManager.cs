using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GLOBAL;

public static class SceneManager
{
    public static IEnumerator LoadScene(SceneID sceneID, Action beforeLoad = null, Action afterLoad = null)
    {
        AsyncOperation process = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)sceneID);
        process!.allowSceneActivation = false;

        beforeLoad?.Invoke();
        
        while (process.progress < 0.9f)
        {
            yield return null;
        }

        afterLoad?.Invoke();

        process.allowSceneActivation = true;
    }
}