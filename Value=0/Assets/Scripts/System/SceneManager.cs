using System;
using UnityEngine;
using static GLOBAL;

public static class SceneManager
{
    public static void LoadScene(SceneID scene, Action onBeforeLoad = null, Action onAfterLoad = null)
    {
        AsyncOperation loading = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)scene);
        loading!.allowSceneActivation = false;

        onBeforeLoad?.Invoke();

        while (loading.progress < 0.9f)
        {
            //TODO: Do Something
        }

        float t = 0;
        while (t <= 1) t += Time.deltaTime;

        onAfterLoad?.Invoke();

        loading.allowSceneActivation = true;
    }
}