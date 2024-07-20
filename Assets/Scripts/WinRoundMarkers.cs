using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinRoundMarkers : MonoBehaviour
{
    [SerializeField] private RoundDataSO roundData;

    [SerializeField] private GameObject[] player1Markers;
    [SerializeField] private GameObject[] player2Markers;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < roundData.player1WinnedRounds; i++)
        {
            player1Markers[i].SetActive(true);
        }
        
        for (int i = 0; i < roundData.player2WinnedRounds; i++)
        {
            player2Markers[i].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}