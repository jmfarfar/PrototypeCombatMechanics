using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public AttackData[] attacks;
    public AttackData currentAttack;
    public Action attackCompletedEvent;
    public Action onActiveFramesEnd;
    public Action activateAttackInput;

    public void StartAttack(int index)
    {
        if (index >= attacks.Length) { attackCompletedEvent?.Invoke(); return; }

        currentAttack = attacks[index];
    }

    public void ExecuteAttack()
    {
        currentAttack.attackEvent?.Invoke(currentAttack);
    }

    public void CancelAttack()
    {
        CancelInvoke();
    }

    public void CompleteAttack()
    {
        attackCompletedEvent?.Invoke();
    }

    public void EndActiveFrames()
    {
        onActiveFramesEnd?.Invoke();
    }

    public void ActivateAttackInput()
    {
        activateAttackInput?.Invoke();
    }
}

