using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}
    public Action<LayerMask> onPlayerDefeated;
    public Action<int> roundStart;
    public Action gameStart;
    public Action onTimeUp;
    public Action clearHUD;
    int round = 1;
    int player1WinnedRounds;
    int player2WinnedRounds;
    int numberOfRounds = 3;
    public string gameSceneName;
    PlayerStateMachine[] players;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        roundStart?.Invoke(round);
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

        round++;
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

    public void HitPause(int frames)
    {
        StartCoroutine(HitPauseRoutine(frames));
    }

    IEnumerator HitPauseRoutine(int frames)
    {
        int frameNumber = 0;
        while (frameNumber < frames)
        {
            Time.timeScale = 0;
            yield return new WaitForEndOfFrame();
            frameNumber++;
        }
        Time.timeScale = 1f;
    }

    public void OnPlayerDefeated(LayerMask playerLayer)
    {
        CalculateRoundWinner(playerLayer);

        if(IsPlayerMatchWinner(playerLayer))return;

        onPlayerDefeated?.Invoke(playerLayer);
        StartCoroutine(NextRoundRoutine());

        round++;
    }

    private void CalculateRoundWinner(LayerMask playerLayer)
    {
        if (playerLayer == LayerMask.NameToLayer("Player1Hurtbox"))
        {
            player2WinnedRounds++;
        }
        else
        {
            player1WinnedRounds++;
        }
    }

    private bool IsPlayerMatchWinner(LayerMask playerLayer)
    {
        if (player1WinnedRounds == Mathf.CeilToInt(numberOfRounds / 2f))
        {
            onPlayerDefeated?.Invoke(playerLayer);
            Debug.Log("Player 1 Wins the Set");
            return true;
        }
        else if (player2WinnedRounds == Mathf.CeilToInt(numberOfRounds / 2f))
        {
            onPlayerDefeated?.Invoke(playerLayer);
            Debug.Log("Player 1 Wins the Set");
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
        roundStart?.Invoke(round);
    }

    private void OnDestroy() 
    {
        foreach (PlayerStateMachine player in players)
        {
            player.playerKO -= OnPlayerDefeated;
        }
    }
}
