using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhaseRotateButton : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    private float percent;
    private Button button;
    private TextMeshProUGUI tmp;

    private void Awake() {
        button = GetComponent<Button>();
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = "Phase Rotate - " + (percent * 100f).ToString("0");
    }

    private void OnEnable() {
        button.onClick.AddListener(OnButtonPressed);
    }

    private void OnDisable() {
        button.onClick.RemoveAllListeners();
    }

    private void OnButtonPressed() {
        GameBoardManager.Instance.PhaseRotateTile(percent);
        gameObject.SetActive(false);
    }
}
