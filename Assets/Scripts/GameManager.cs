using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}
    public Action<LayerMask> onPlayerDefeated;
    public Action<int> roundStart;
    public Action gameStart;
    public Action onTimeUp;
    int round = 1;
    int player1Wins;
    int player2Wins;
    int numberOfRounds = 3;
    public string gameSceneName;

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
    }

    private IEnumerator Start() 
    {
        yield return new WaitForEndOfFrame();
        roundStart?.Invoke(round);
    }

    public void OnTimeOut()
    {
        onTimeUp?.Invoke();
        round++;
        numberOfRounds++;
        StartCoroutine(NextRoundRoutine());
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
        if (playerLayer == LayerMask.NameToLayer("Player1Hurtbox"))
        {
            player2Wins++;
        }
        else
        {
            player1Wins++;
        }

        if (player1Wins == Mathf.CeilToInt(numberOfRounds/2f))
        {
            onPlayerDefeated?.Invoke(playerLayer);
            Debug.Log("Player 1 Wins the Set");
            return;
        }
        else if (player2Wins == Mathf.CeilToInt(numberOfRounds/2f))
        {
            onPlayerDefeated?.Invoke(playerLayer);
            Debug.Log("Player 1 Wins the Set");
            return;
        }

        onPlayerDefeated?.Invoke(playerLayer);
        StartCoroutine(NextRoundRoutine());
        
        round++;
    }

    IEnumerator NextRoundRoutine()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(gameSceneName);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        roundStart?.Invoke(round);
    }
}
