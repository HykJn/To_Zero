using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    #region ==========Properties==========
    public static GameManager Instance => instance;
    public int Stage
    {
        get => curStage;
        set
        {
            if (value < 1 || value > stages.Length)
                throw new IndexOutOfRangeException();

            curStage = value;
            for (int i = 0; i < stages.Length; i++)
            {
                stages[i].SetActive(i == curStage - 1);
            }
        }
    }
    #endregion

    #region ==========Fields==========
    private static GameManager instance = null;

    [SerializeField] private GameObject[] stages;
    [SerializeField] private int curStage;
    #endregion

    #region ==========Unity Methods==========
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        Stage = 1;
    }
    #endregion

    #region ==========Methods==========
    public void Transition(EventID id) => Camera.main.GetComponent<PPTransition1>().Transition(id);

    public void SwapTiles() => stages[curStage - 1].GetComponent<Stage>().SwapTiles();

    public void Restart() => stages[curStage - 1].GetComponent<Stage>().Restart();
    #endregion
}
