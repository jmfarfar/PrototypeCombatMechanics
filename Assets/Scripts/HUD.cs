using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerLabel;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private GameObject roundPanel;
    [SerializeField] private TextMeshProUGUI roundLabel;
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private TextMeshProUGUI winnerLabel;
    [SerializeField] private GameObject fightPanel;
    [SerializeField] private GameObject timeUpPanel;
    float timer;
    float maxTime = 99f;
    private bool isTimeRunning;

    private void Start() 
    {
        timer = maxTime;
        GameManager.Instance.onPlayerDefeated += OnPlayerDefeated;
        GameManager.Instance.roundStart += OnRoundStart;
        GameManager.Instance.onTimeUp += OnTimeUp;

        fightPanel.SetActive(false);
        timerPanel.SetActive(false);
        roundPanel.SetActive(false);
        winnerPanel.SetActive(false);
        timeUpPanel.SetActive(false);
    }

    private void OnTimeUp()
    {
        timeUpPanel.SetActive(true);
        timer = maxTime;
        isTimeRunning = false;
    }

    private void OnRoundStart(int roundNumber)
    {
        StartCoroutine(RoundStartRoutine(roundNumber));
    }

    private void OnPlayerDefeated(LayerMask mask)
    {
        isTimeRunning = false;
        if (mask == LayerMask.NameToLayer("Player1Hurtbox"))
        {
            winnerPanel.SetActive(true);
            winnerLabel.text = "Player 2 Wins!";
        }
        else
        {
            winnerPanel.SetActive(true);
            winnerLabel.text = "Player 1 Wins!";
        }
    }

    private void Update() 
    {
        if (!isTimeRunning){return;}

        timer -= Time.unscaledDeltaTime;
        timerLabel.text = Mathf.Ceil(timer).ToString();

        if (timer <= 0f)
        {
            GameManager.Instance.OnTimeOut();
        }
    }

    private void OnDestroy() 
    {
        GameManager.Instance.onPlayerDefeated -= OnPlayerDefeated;
        GameManager.Instance.roundStart -= OnRoundStart;
        GameManager.Instance.onTimeUp -= OnTimeUp;
    }

    IEnumerator RoundStartRoutine(int round)
    {
        roundPanel.SetActive(true);
        roundLabel.text = $"Round {round}";
        yield return new WaitForSecondsRealtime(1f);
        roundPanel.SetActive(false);
        fightPanel.SetActive(true);
        timerPanel.SetActive(true);
        isTimeRunning = true;
        GameManager.Instance.StartRound();
        yield return new WaitForSecondsRealtime(1f);
        fightPanel.SetActive(false);
    }
}
