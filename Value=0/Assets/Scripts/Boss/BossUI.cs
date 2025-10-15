// BossUI.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text_PlayerHealth;
    [SerializeField] private TMP_Text text_BossHealth;
    [SerializeField] private TMP_Text text_BossTartgetValue;
    [SerializeField] private GameObject bossUI;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text text_GameOver;
    [SerializeField] private Button button_Retry;
    [SerializeField] private TMP_Text text_Retry;


    private void Start()
    {
        if(button_Retry != null)
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
        // 중복 구독 방지를 위해 먼저 구독 해제
        UnsubscribeEvents();

        BossManager.Instance.PlayerHealthChange += UpdatePlayerHealth;
        BossManager.Instance.BossHealthChange += UpdateBossHealth;
        BossManager.Instance.BossTargetValueChange += UpdateBossTargetValue;
        BossManager.Instance.OnPlayerLose += ShowGameOver;

    }

    private void UnsubscribeEvents()
    {
        if (BossManager.Instance == null) return;

        BossManager.Instance.PlayerHealthChange -= UpdatePlayerHealth;
        BossManager.Instance.BossHealthChange -= UpdateBossHealth;
        BossManager.Instance.BossTargetValueChange -= UpdateBossTargetValue;
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
            if(BossManager.Instance != null)
            {
                SubscribeEvents();
            }
        }
    }

    private void UpdatePlayerHealth(int health)
    {
        text_PlayerHealth.text = $"플레이어 HP: {health}";
        Debug.Log($"UI 업데이트: 플레이어 HP = {health}");
    }

    private void UpdateBossHealth(int health)
    {
        text_BossHealth.text = $"보스 HP: {health}";
        Debug.Log($"UI 업데이트: 보스 HP = {health}");
    }

    private void UpdateBossTargetValue(int value)
    {
        text_BossTartgetValue.text = $"목표값: {value}";
        Debug.Log($"UI 업데이트: 목표값 = {value}");
    }

    private void ShowGameOver()
    {
       gameOverPanel.SetActive(true);
       text_GameOver.text = "GAME OVER"; 
       GameManager.Instance.Player._isMovable = false;
       
    }

    private void ReStartStage()
    {
        if(gameOverPanel != null) gameOverPanel.SetActive(false);
        GameManager.Instance.Player.ClearBombs();
        GameManager.Instance?.Restart();
    }
}