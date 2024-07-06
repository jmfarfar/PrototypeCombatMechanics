using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RoundData", menuName = "RoundData", order = 0)]
public class RoundDataSO : ScriptableObject 
{
    public int round = 1;
    public int player1WinnedRounds;
    public int player2WinnedRounds;

    public void ResetData()
    {
        round = 1;
        player1WinnedRounds = 0;
        player2WinnedRounds = 0;
    }
}
