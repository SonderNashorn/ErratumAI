using UnityEngine;
using StateStuff;

public class PatrolState : State<AI>
{
    #region //Heart of the code, don't touch
    private static PatrolState _instance;

    private PatrolState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }

    public static PatrolState Instance
    {
        get
        {
            if (_instance == null)
            {
                new PatrolState();
            }
            return _instance;
        }

    }
    #endregion


    public override void EnterState(AI _owner)
    {
        _owner.navMeshAgent.speed = 1.5f;
    }
    public override void ExitState(AI _owner)
    {
        
    }
    public override void UpdateState(AI _owner)
    {       
        //visionState 1 means monster is flashed
        _owner.Patrol();
        if (_owner.visionState == 1)
        {
            _owner.particleSystem.Play();
            _owner.aiState = AiStates.Teleport;
            _owner.stateMachine.ChangeState(TeleportState.Instance);
        }
        /*if (_owner.visionState == 4)
        {
            _owner.aiState = AiStates.Catch;
            _owner.stateMachine.ChangeState(CatchState.Instance);
        } */
        //Visionstate 3 is to watch the player
        else if (_owner.visionState < 5 && _owner.visionState != 1)
        {
            _owner.aiState = AiStates.Watch;
            _owner.stateMachine.ChangeState(WatchState.Instance);
        } //visionState 4 is to chase without question
        /*if (_owner.ignoreTime == true || _owner.visionState == 2)
        {
            _owner.aiState = AiStates.Catch;
            _owner.stateMachine.ChangeState(CatchState.Instance);
        }*/
        
        
    }
}

//There is a visionstate 2 originally intended to be there if I want the Ai to attack immediately.
    //for when the player is facing it and the enemy would be afraid of getting flashed so it would charge.
    //for now it is just buckled under the same watch state.

//If the player is facing the enemy CHarge. Which might just become a separate state so I can control the speed better.
/*if (_owner.visionState == 2)
{
    _owner.aiState = AiStates.Catch;
    _owner.stateMachine.ChangeState(CatchState.Instance);
}*/
