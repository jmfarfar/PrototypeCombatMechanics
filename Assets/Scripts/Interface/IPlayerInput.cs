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
