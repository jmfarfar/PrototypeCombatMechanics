using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Action<LayerMask> onPlayerDefeated;
    public Action<int> roundStart;
    public Action gameStart;
    public Action onTimeUp;
    public Action clearHUD;
    int numberOfRounds = 3;
    public string gameSceneName;
    PlayerStateMachine[] players;
    [SerializeField] private RoundDataSO roundData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        players = FindObjectsOfType<PlayerStateMachine>();
        foreach (PlayerStateMachine player in players)
        {
            player.playerKO += OnPlayerDefeated;
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        roundStart?.Invoke(roundData.round);
    }

    public void OnTimeOut()
    {
        onTimeUp?.Invoke();

        float highestHealthPoints = 0f;
        foreach (PlayerStateMachine player in players)
        {
            highestHealthPoints = Mathf.Max(highestHealthPoints, player.Health.CurrentHealth);
        }

        foreach (PlayerStateMachine player in players)
        {
            if (!(player.Health.CurrentHealth == highestHealthPoints))
            {
                StartCoroutine(DelayOnPlayerDefeated(player.gameObject.layer));
                return;
            }
        }

        roundData.round++;

        numberOfRounds++;
        StartCoroutine(NextRoundRoutine());
    }

    IEnumerator DelayOnPlayerDefeated(LayerMask loserLayer)
    {
        yield return new WaitForSeconds(1f);
        clearHUD?.Invoke();
        OnPlayerDefeated(loserLayer);
    }

    public void StartRound()
    {
        gameStart?.Invoke();
    }

    public void HitPause(float time)
    {
        StartCoroutine(HitPauseRoutine(time));
    }

    IEnumerator HitPauseRoutine(float time)
    {
        Time.timeScale = 0f;
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        float timer = 0f;
        while (timer < time)
        {
            Time.timeScale = 0.1f;
            timer += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 1f;
    }

    public void OnPlayerDefeated(LayerMask playerLayer)
    {
        CalculateRoundWinner(playerLayer);

        if(IsPlayerMatchWinner(playerLayer))return;


        onPlayerDefeated?.Invoke(playerLayer);
        StartCoroutine(NextRoundRoutine());

        roundData.round++;
    }

    private void CalculateRoundWinner(LayerMask playerLayer)
    {
        if (playerLayer == LayerMask.NameToLayer("Player1Hurtbox"))
        {
            roundData.player2WinnedRounds++;
        }
        else
        {
            roundData.player1WinnedRounds++;
        }
    }

    private bool IsPlayerMatchWinner(LayerMask playerLayer)
    {
        if (roundData.player1WinnedRounds == Mathf.CeilToInt(numberOfRounds / 2f))
        {
            onPlayerDefeated?.Invoke(playerLayer);
            Debug.Log("Player 1 Wins the Set");
            StartCoroutine(FinishGame());
            return true;
        }
        else if (roundData.player2WinnedRounds == Mathf.CeilToInt(numberOfRounds / 2f))
        {
            onPlayerDefeated?.Invoke(playerLayer);
            Debug.Log("Player 2 Wins the Set");
            StartCoroutine(FinishGame());
            return true;
        }
        return false;
    }

    IEnumerator NextRoundRoutine()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(gameSceneName);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        roundStart?.Invoke(roundData.round);
    }

    IEnumerator FinishGame()
    {
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene(gameSceneName);
        roundData.ResetData();
    }

    private void OnDestroy() 
    {
        foreach (PlayerStateMachine player in players)
        {
            player.playerKO -= OnPlayerDefeated;
        }
    }
}