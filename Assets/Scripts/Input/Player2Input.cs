using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;

public class Player2Input : MonoBehaviour, IPlayerInput
{
    public bool GetFirstRadialMenuButton()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton1);
    }

    public bool GetDashButton()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton0);
    }

    public Vector2 GetMovementInputVector()
    {
        return new Vector2(Input.GetAxisRaw("JHorizontal"), Input.GetAxisRaw("JVertical"));
    }

    public bool GetSecondRadialMenuButton()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton2);
    }

    public bool GetThirdRadialMenuButton()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton3);
    }

    public bool GetBlockButton()
    {
        return Input.GetKey(KeyCode.JoystickButton4);
    }

    public int GetRadialMenuQuadrant()
    {
        Vector2 inputVector = new Vector2(Input.GetAxisRaw("JRHorizontal"), Input.GetAxisRaw("JRVertical"));

        if(inputVector.sqrMagnitude < 0.5f) return 4;

        float angle = Mathf.Atan2(inputVector.y,inputVector.x) * Mathf.Rad2Deg;

        angle = Mathf.Floor(((angle + 360) % 360 / 90) % 3); // assign a quadrant depending on the angle

        return (int)angle;
    }
}
