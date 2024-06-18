using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Input : MonoBehaviour, IPlayerInput
{
    public bool GetFirstRadialMenuButton()
    {
        return Input.GetKeyDown(KeyCode.J);
    }

    public bool GetDashButton()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public Vector2 GetMovementInputVector()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public bool GetSecondRadialMenuButton()
    {
        return Input.GetKeyDown(KeyCode.K);
    }

    public bool GetThirdRadialMenuButton()
    {
        return Input.GetKeyDown(KeyCode.L);
    }

    public bool GetBlockButton()
    {
        return Input.GetKey(KeyCode.O);
    }

    public int GetRadialMenuQuadrant()
    {
        if (Input.GetKeyDown(KeyCode.L)) return 0;
        if (Input.GetKeyDown(KeyCode.I)) return 1;
        if (Input.GetKeyDown(KeyCode.J)) return 2;
        if (Input.GetKeyDown(KeyCode.K)) return 3;
        else return 4;
    }
}
