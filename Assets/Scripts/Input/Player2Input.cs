using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;

public class Player2Input : MonoBehaviour, IPlayerInput
{
    private bool inputSent;
    
    public bool GetFirstRadialMenuButton()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton2);
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
        return Input.GetKeyDown(KeyCode.JoystickButton3);
    }

    public bool GetThirdRadialMenuButton()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton1);
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

    public bool CharacterSelectionMove(out Vector2 input)   
    {
        
        if (inputSent)
        {
            input = Vector2.zero;
            return false;
        }
        
        float horizontal = Input.GetAxisRaw("JHorizontal");
        float vertical = Input.GetAxisRaw("JVertical");

        Vector2 axis = new Vector2(horizontal, vertical);

        if (Mathf.Round(axis.x) != 0 || Mathf.Round(axis.y) != 0)
        {
            input = new Vector2(Mathf.Round(axis.x), Mathf.Round(axis.y));
            inputSent = true;
            StartCoroutine(ValidateInput());
            return true;
        }

        input = Vector2.zero;
        return false;
    }

    public bool UIAccept()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton0);
    }

    public bool UICancel()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton1);
    }

    IEnumerator ValidateInput()
    {
        yield return new WaitUntil(
            () => Mathf.Round(Input.GetAxisRaw("JHorizontal")) == 0 && Mathf.Round(Input.GetAxisRaw("JVertical")) == 0
        );

        inputSent = false;
    }
}
