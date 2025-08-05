using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    #region ==========Properties==========
    [SerializeField] private GameObject loadingPanel;
    #endregion

    #region ==========Fields==========

    #endregion

    #region ==========Unity Methods==========
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    #region ==========Methods==========
    public void Loading(int stage) => StartCoroutine(Loading_(stage));

    private IEnumerator Loading_(int stage)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(1);
        loading.allowSceneActivation = false;

        loadingPanel.SetActive(true);
        loadingPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        loadingPanel.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1, 0);
        float t = 0;
        //int idx = 0;
        while (t <= 3)
        {
            t += Time.deltaTime;
            loadingPanel.GetComponent<Image>().color = new Color(1, 1, 1, Mathf.Clamp01(t));
            loadingPanel.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1, Mathf.Clamp01(t));
            loadingPanel.GetComponentInChildren<TMP_Text>().text = "Now Loading" + new string('.', (int)(t * 3) % 6);
            yield return null;
        }
        loading.allowSceneActivation = true;
        loadingPanel.GetComponentInChildren<TMP_Text>().text = "Press any key to continue...";

        while (!Input.anyKeyDown) yield return null;

        GameManager.Instance.Stage = stage;
        GameObject.FindWithTag("Player").GetComponent<Player>().IsMovable = false;
        t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime;
            loadingPanel.GetComponent<Image>().color = new Color(1, 1, 1, 1 - Mathf.Clamp01(t));
            loadingPanel.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1, 1 - Mathf.Clamp01(t));
            yield return null;
        }
        GameObject.FindWithTag("Player").GetComponent<Player>().IsMovable = true;

        loadingPanel.SetActive(false);
        loadingPanel.GetComponent<Image>().color = Color.white;
    }
    #endregion
}
