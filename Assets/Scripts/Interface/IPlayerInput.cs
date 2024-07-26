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
    bool CharacterSelectionMove(out Vector2 input);
    bool UIAccept();
    bool UICancel();
}

public class EmptyPlayerInput : IPlayerInput
{
    public bool GetBlockButton(){return false;}
    public bool GetDashButton(){return false;}
    public bool GetFirstRadialMenuButton(){return false;}
    public Vector2 GetMovementInputVector(){return Vector2.zero;}
    public int GetRadialMenuQuadrant(){return 4;}

    public bool CharacterSelectionMove(out Vector2 input)
    {
        input = Vector2.zero;
        return true;
    }

    public bool UIAccept() { return false; }
    public bool UICancel() { return false; }
    public bool GetSecondRadialMenuButton(){return false;}
    public bool GetThirdRadialMenuButton(){return false;}
    
}
