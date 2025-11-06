using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TileValueInputPanel : MonoBehaviour
{
    #region ===== Fields =====

    [Header("UI References")]
    [SerializeField] private TMP_InputField valueInput;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text titleText;

    private Vector2 targetPosition;
    private string operatorType; // "+", "-", "*", "/"
    private Action<Vector2, string> onConfirm;

    #endregion

    #region ===== Unity Events =====

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);

        gameObject.SetActive(false);
    }

    #endregion

    #region ===== Methods =====

    public void Show(Vector2 position, string operatorType, Action<Vector2, string> callback)
    {
        this.targetPosition = position;
        this.operatorType = operatorType;
        this.onConfirm = callback;


        string operatorSymbol = operatorType switch
        {
            "+" => "덧셈",
            "-" => "뺄셈",
            "*" => "곱셈",
            "/" => "나눗셈",
            _ => "연산"
        };
        titleText.text = $"{operatorSymbol} 타일 값 입력";


        valueInput.text = "";
        gameObject.SetActive(true);

   
        valueInput.Select();
        valueInput.ActivateInputField();

        Debug.Log($"Input panel shown for {operatorType} at {position}");
    }

    private void OnConfirm()
    {
        string input = valueInput.text.Trim();

     
        if (string.IsNullOrEmpty(input))
        {
            Debug.LogWarning("값을 입력해주세요!");
            return;
        }

    
        if (!int.TryParse(input, out int value))
        {
            Debug.LogWarning("올바른 숫자를 입력해주세요!");
            return;
        }

 
        if (value < 1 || value > 999)
        {
            Debug.LogWarning("1~999 사이의 값을 입력해주세요!");
            return;
        }


        string tileCode = operatorType + value.ToString();
        Debug.Log($"Tile code created: {tileCode}");

   
        onConfirm?.Invoke(targetPosition, tileCode);

   
        gameObject.SetActive(false);
    }

    private void OnCancel()
    {
        Debug.Log("Input cancelled");
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            OnConfirm();
        }
        else if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            OnCancel();
        }
    }

    #endregion
}