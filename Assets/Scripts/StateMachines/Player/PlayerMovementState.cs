using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovementState : PlayerBaseState
{
    public PlayerMovementState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    private int MovementStateHash = Animator.StringToHash("MovementState");
    private int MovementSpeedHash = Animator.StringToHash("MovementSpeed");

    private int menuSelected;

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(MovementStateHash, 0.1f);
    }

    public override void Exit()
    {

    }

    public override void Tick(float deltaTime)
    {
        Vector2 movementInput = stateMachine.PlayerInput.GetMovementInputVector();

        UpdateAnimation(movementInput);

        Vector3 direction = Camera.main.transform.forward * movementInput.y +
            Camera.main.transform.right * movementInput.x;
        direction.y = 0f;
        direction.Normalize();

        //Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y);

        if (!(direction == Vector3.zero))
        {
            Quaternion toRotate = Quaternion.LookRotation(direction, Vector3.up);
            stateMachine.transform.rotation = Quaternion.RotateTowards(stateMachine.transform.rotation, toRotate, Time.deltaTime * stateMachine.RotationSpeed);
        }

        if (stateMachine.PlayerInput.GetDashButton())
        {

            stateMachine.SwitchState(new PlayerDashState(stateMachine));
        }

        else if (stateMachine.PlayerInput.GetFirstRadialMenuButton())
        {
            ActivateRadialMenu(0);
        }

        else if (stateMachine.PlayerInput.GetSecondRadialMenuButton())
        {
            ActivateRadialMenu(1);
        }

        else if (stateMachine.PlayerInput.GetThirdRadialMenuButton())
        {
            ActivateRadialMenu(2);
        }

        else if (stateMachine.PlayerInput.GetBlockButton() && !stateMachine.isShieldDepleted)
        {
            stateMachine.SwitchState(new PlayerBlockState(stateMachine));
        }

        if (stateMachine.isRadialMenuActive)
        {
            ExecuteRadialMenu(menuSelected);
        }

        stateMachine.CharacterController.Move(direction * stateMachine.Speed * Time.deltaTime);
    }

    private void UpdateAnimation(Vector2 movementInput)
    {
        float movementFactor = Mathf.Ceil(movementInput.sqrMagnitude);

        stateMachine.Animator.SetFloat(MovementSpeedHash,
        Mathf.Lerp(stateMachine.Animator.GetFloat(MovementSpeedHash),
        movementFactor,
        Time.deltaTime * 10f
        ));

    }

    private void ActivateRadialMenu(int menuIndex)
    {
        stateMachine.isRadialMenuActive = !stateMachine.isRadialMenuActive;

        if (stateMachine.isRadialMenuActive)
        {
            stateMachine.RadialMenu[menuIndex].SetActive(true);
            menuSelected = menuIndex;
        }
        else
        {
            stateMachine.RadialMenu[menuIndex].SetActive(false);
        }
    }

    private void ExecuteRadialMenu(int menuIndex)
    {
        int cuadrant = stateMachine.PlayerInput.GetRadialMenuQuadrant();

        int currentSpell = menuIndex switch
        {
            0 => 0, // spells for menu X
            1 => 4, // spells for menu Y
            2 => 8, // spells for menu B
            _ => 0,
        };

        int spellNumber = cuadrant + currentSpell;

        if(spellNumber > currentSpell + 3) return;

        stateMachine.selectedObject.eulerAngles = new Vector3(0f, 0f, cuadrant * 90f);
        stateMachine.highlightedSpellName.text = stateMachine.SpellNames[spellNumber];

        Debug.Log($"{stateMachine.SpellNames[spellNumber]} spell casted");
        stateMachine.SwitchState(new PlayerAttackState(stateMachine, spellNumber));

        stateMachine.isRadialMenuActive = false;
        stateMachine.RadialMenu[menuIndex].SetActive(false);
    }
}
