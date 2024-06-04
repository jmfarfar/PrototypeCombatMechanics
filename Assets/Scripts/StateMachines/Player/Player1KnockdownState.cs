using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1KnockdownState : Player1BaseState
{
    private int KnockdownHash = Animator.StringToHash("Knockdown");
    private int GetupHash = Animator.StringToHash("GetUp");
    private int RollHash = Animator.StringToHash("Roll");
    private float duration;
    float startTime;

    public Player1KnockdownState(Player1StateMachine stateMachine, float duration) : base(stateMachine)
    {
        this.duration = duration;
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(KnockdownHash, 0.1f);
        stateMachine.rollEvent += OnRoll;
    }

    public override void Exit()
    {
        stateMachine.Animator.CrossFadeInFixedTime(GetupHash, 0.1f);
        stateMachine.rollEvent -= OnRoll;
    }

    public override void Tick(float deltaTime)
    {
        //stateMachine.CharacterController.Move(Vector3.zero * deltaTime);
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y);

        duration -= deltaTime;

        if (direction != Vector3.zero && !stateMachine.isRolling)
        {
            stateMachine.rollEvent?.Invoke(direction);
            stateMachine.isRolling = true;
        }

        if (duration <= 0f)
        {
            stateMachine.SwitchState(new Player1MovementState(stateMachine));
        }
    }

    void OnRoll(Vector3 direction)
    {
        // Move the character
        startTime = Time.time;


        if (!(direction == Vector3.zero))
        {
            stateMachine.Animator.CrossFadeInFixedTime(RollHash, 0.1f);
            stateMachine.StartCoroutine(Rolling(direction));
        }

    }

    IEnumerator Rolling(Vector3 direction)
    {
        while (Time.time < startTime + 1.2f)
        {
            yield return new WaitForEndOfFrame();
            stateMachine.CharacterController.Move(direction * stateMachine.Speed * Time.deltaTime);
        }
    }
}
