using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player2BaseState : State
{
    protected Player2StateMachine stateMachine;

    public Player2BaseState(Player2StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }


}
