using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="CharacterSelection", menuName = "Character Selection")]
public class CharacterSelection : ScriptableObject
{
    public GameObject Player1;
    public GameObject Player2;

    public void ResetPlayers()
    {
        Player1 = null;
        Player2 = null;
    }
}
