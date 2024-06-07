using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1MovementState : Player1BaseState
{
    public Player1MovementState(Player1StateMachine stateMachine) : base(stateMachine)
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
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {

            stateMachine.SwitchState(new Player1DashState(stateMachine));
        }

        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ActivateRadialMenu(0);
        }

        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ActivateRadialMenu(1);
        }

        else if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            ActivateRadialMenu(2);
        }

        else if (Input.GetKey(KeyCode.O) && !stateMachine.isShieldDepleted)
        {
            stateMachine.SwitchState(new Player1BlockState(stateMachine));
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
        //if (stateMachine.isRadialMenuActive)
        //{
        //Vector2 delta = stateMachine.menuCenter.position - Input.mousePosition;
        //float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        float angle = float.MaxValue;
        if (Input.GetKeyDown(KeyCode.L)) angle = 0f;
        if (Input.GetKeyDown(KeyCode.I)) angle = 90f;
        if (Input.GetKeyDown(KeyCode.J)) angle = 180f;
        if (Input.GetKeyDown(KeyCode.K)) angle = 270f;
        //angle += 180;

        int currentSpell = menuIndex switch
        {
            0 => 0, // spells for menu X
            1 => 4, // spells for menu Y
            2 => 8, // spells for menu B
            _ => 0,
        };

        for (int i = 0; i < 360; i += 90)
        {
            //Debug.Log($"Valor de i: {i}");
            if (angle >= i && angle < i + 90)
            {
                //Debug.Log($"Valor de angle: {angle}");
                stateMachine.selectedObject.eulerAngles = new Vector3(0, 0, i);
                stateMachine.highlightedSpellName.text = stateMachine.SpellNames[currentSpell];



                Debug.Log($"Floor de i: {Mathf.FloorToInt(currentSpell / 4)}");
                stateMachine.SwitchState(new Player1AttackState(stateMachine, Mathf.FloorToInt(currentSpell / 4)));
                Debug.Log($"{stateMachine.SpellNames[currentSpell]} spell casted");
                stateMachine.isRadialMenuActive = false;
                stateMachine.RadialMenu[menuIndex].SetActive(false);

            }
            currentSpell++;
        }
        //}
    }
}
