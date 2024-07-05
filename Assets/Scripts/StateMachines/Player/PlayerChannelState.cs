using UnityEngine;

namespace StateMachines.Player
{
    public class PlayerChannelState : PlayerBaseState
    {
        private readonly int channelStateHash = Animator.StringToHash("Channel");
        private float timeCounter;
        // private float chargeTime;
        private readonly AttackData attackData;

        public PlayerChannelState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public PlayerChannelState(PlayerStateMachine stateMachine, AttackData attackData) : base(stateMachine)
        {
            this.attackData = attackData;
        }

        public override void Enter()
        {
            stateMachine.Animator.CrossFadeInFixedTime(channelStateHash, 0.1f);
            // chargeTime = timeCounter > 0f ? timeCounter : attackData.activeTime * 100f;
        }

        public override void Tick(float deltaTime)
        {
            stateMachine.DecreaseChargeTime();
            Debug.Log($"Charge time: {stateMachine.ChargeTime}");

            if (stateMachine.PlayerInput.GetBlockButton() && !stateMachine.isShieldDepleted && stateMachine.ChargeTime > 0f)
            {
                
                stateMachine.SwitchState(new PlayerBlockState(stateMachine));
            }
            else if (stateMachine.ChargeTime <= 0f)
            {
                stateMachine.ChannelComplete();
                stateMachine.SwitchState(new PlayerMovementState(stateMachine));
            }
        }

        public override void Exit()
        {
            // stateMachine.SwitchState(new PlayerMovementState(stateMachine));
        }
    }
}