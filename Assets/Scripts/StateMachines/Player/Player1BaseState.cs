using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player1BaseState : State
{
    protected Player1StateMachine stateMachine;

    public Player1BaseState(Player1StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }


}
