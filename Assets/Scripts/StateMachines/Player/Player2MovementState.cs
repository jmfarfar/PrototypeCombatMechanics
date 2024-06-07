using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2MovementState : Player2BaseState
{
    public Player2MovementState(Player2StateMachine stateMachine) : base(stateMachine)
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
        //Debug.Log($"Right Stick X: {Input.GetAxisRaw("JRHorizontal")}");
        //Debug.Log($"Right Stick Y: {Input.GetAxisRaw("JRVertical")}");
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("JHorizontal"), Input.GetAxisRaw("JVertical")).normalized;

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

        if (Input.GetKeyDown(KeyCode.JoystickButton0)) // gamepad button south
        {
            stateMachine.SwitchState(new Player2DashState(stateMachine));
        }

        else if (Input.GetKeyDown(KeyCode.Joystick1Button1)) // gamepad button east
        {
            ActivateRadialMenu(2);
            //stateMachine.SwitchState(new Player2AttackState(stateMachine, 0));
        }

        else if (Input.GetKeyDown(KeyCode.Joystick1Button2)) // gamepad button north
        {
            ActivateRadialMenu(0);
            //stateMachine.SwitchState(new Player2AttackState(stateMachine, 1));
        }

        else if (Input.GetKeyDown(KeyCode.Joystick1Button3)) // gamepad button west
        {
            ActivateRadialMenu(1);
            //stateMachine.SwitchState(new Player2AttackState(stateMachine, 3));
        }

        else if (Input.GetKey(KeyCode.Joystick1Button4) && !stateMachine.isShieldDepleted)
        {
            stateMachine.SwitchState(new Player2BlockState(stateMachine));
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

        stateMachine.Animator.SetFloat(MovementSpeedHash, Mathf.Lerp(stateMachine.Animator.GetFloat(MovementSpeedHash), movementFactor, Time.deltaTime * 10f));
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
        Vector3 selection = new Vector3(Input.GetAxisRaw("JRHorizontal"), Input.GetAxisRaw("JRVertical")).normalized;
        Debug.Log($"Vector Raw: {new Vector3(Input.GetAxisRaw("JRHorizontal"), Input.GetAxisRaw("JRVertical")).normalized}");
        //Vector2 delta = stateMachine.menuCenter.position - new Vector3(Input.GetAxisRaw("JRHorizontal"), Input.GetAxisRaw("JRVertical")).normalized;
        //Debug.Log($"Dellta vaue: {delta}");
        //float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        //Debug.Log($"Angle before: {angle}");
        float angle = 0;
        if (selection == Vector3.zero) angle = float.MaxValue;
        if (selection.x > 0.8f && selection.y > -0.5f && selection.y < 0.5f) angle = 0f;//right
        if (selection.y > 0.8f && selection.x > -0.5f && selection.x < 0.5f) angle = 90f;//up
        if (selection.x < -0.8f && selection.y > -0.5f && selection.y < 0.5f) angle = 180f;//lellft
        if (selection.y < -0.8f && selection.x > -0.5f && selection.x < 0.5f) angle = 270f;// bot
        //angle += 180;
        //Debug.Log($"Ange after: {angle}");

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


                if (Input.GetMouseButtonDown(0))
                {

                }
                Debug.Log($"Floor de i: {Mathf.FloorToInt(currentSpell / 4)}");
                stateMachine.SwitchState(new Player2AttackState(stateMachine, Mathf.FloorToInt(currentSpell / 4)));
                Debug.Log($"{stateMachine.SpellNames[currentSpell]} spell casted");
                stateMachine.isRadialMenuActive = false;
                stateMachine.RadialMenu[menuIndex].SetActive(false);

            }
            currentSpell++;
        }
        //}
    }
}
