// BossUI.cs

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text_PlayerHealth;
    [SerializeField] private TMP_Text text_BossHealth;
    //[SerializeField] private TMP_Text text_BossTartgetValue;
    [SerializeField] private GameObject bossUI;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text text_GameOver;
    [SerializeField] private Button button_Retry;
    [SerializeField] private TMP_Text text_Retry;


    private void Start()
    {
        if (button_Retry != null)
        {
            button_Retry.onClick.AddListener(ReStartStage);
        }

        if (text_Retry != null)
        {
            text_Retry.text = "Retry?";
        }

        gameOverPanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStageLoaded += OnStageChanged;
        }
    }

    private void OnEnable()
    {
        if (BossManager.Instance != null)
        {
            SubscribeEvents();
        }
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
        GameManager.Instance.OnStageLoaded -= OnStageChanged;
    }

    private void SubscribeEvents()
    {
        if (BossManager.Instance == null)
        {
            return;
        }

        bossUI.SetActive(true);
        // �ߺ� ���� ������ ���� ���� ���� ����
        UnsubscribeEvents();

        BossManager.Instance.PlayerHealthChange += UpdatePlayerHealth;
        BossManager.Instance.BossHealthChange += UpdateBossHealth;
        //BossManager.Instance.BossTargetValueChange += UpdateBossTargetValue;
        BossManager.Instance.OnPlayerLose += ShowGameOver;
    }

    private void UnsubscribeEvents()
    {
        if (BossManager.Instance == null) return;

        BossManager.Instance.PlayerHealthChange -= UpdatePlayerHealth;
        BossManager.Instance.BossHealthChange -= UpdateBossHealth;
        //BossManager.Instance.BossTargetValueChange -= UpdateBossTargetValue;
        BossManager.Instance.OnPlayerLose -= ShowGameOver;
    }

    private void OnStageChanged()
    {
        if (!GameManager.Instance.CurrentBossStage)
        {
            UnsubscribeEvents();
            bossUI.SetActive(false);
            gameOverPanel.SetActive(false);
        }
        else
        {
            if (BossManager.Instance != null)
            {
                SubscribeEvents();
            }
        }
    }

    private void UpdatePlayerHealth(int health)
    {
        text_PlayerHealth.text = $"Player HP: {health}";
    }

    private void UpdateBossHealth(int health)
    {
        text_BossHealth.text = $" Boss HP: {health}";
    }

    //private void UpdateBossTargetValue(int value)
    //{
    //    if (value > 0)
    //        text_BossTartgetValue.text = $"+{value}";
    //    else
    //        text_BossTartgetValue.text = $"{value}";
    //}

    private void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        text_GameOver.text = "GAME OVER";
        GameManager.Instance.Player.IsMovable = false;
    }

    private void ReStartStage()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        GameManager.Instance.Player.ClearBombs();
        GameManager.Instance?.Restart();
    }
}