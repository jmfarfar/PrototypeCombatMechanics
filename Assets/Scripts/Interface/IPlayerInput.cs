using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPlayerInput
{
    Vector2 GetMovementInputVector();
    bool GetDashButton();
    bool GetFirstRadialMenuButton();
    bool GetSecondRadialMenuButton();
    bool GetThirdRadialMenuButton();
    bool GetBlockButton();
    int GetRadialMenuQuadrant();
}

public class EmptyPlayerInput : IPlayerInput
{
    public bool GetBlockButton(){return false;}
    public bool GetDashButton(){return false;}
    public bool GetFirstRadialMenuButton(){return false;}
    public Vector2 GetMovementInputVector(){return Vector2.zero;}
    public int GetRadialMenuQuadrant(){return 4;}
    public bool GetSecondRadialMenuButton(){return false;}
    public bool GetThirdRadialMenuButton(){return false;}
}
