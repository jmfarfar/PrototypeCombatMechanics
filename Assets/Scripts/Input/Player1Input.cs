using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Input : MonoBehaviour, IPlayerInput
{
    private bool inputSent;
    
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

    public bool CharacterSelectionMove(out Vector2 input)
    {
        if (inputSent)
        {
            input = Vector2.zero;
            return false;
        }
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

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
        return Input.GetKeyDown(KeyCode.Return);
    }

    public bool UICancel()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }

    IEnumerator ValidateInput()
    {
        yield return new WaitUntil(
            () => Mathf.Round(Input.GetAxisRaw("Horizontal")) == 0 && Mathf.Round(Input.GetAxisRaw("Vertical")) == 0
        );

        inputSent = false;
    }
}