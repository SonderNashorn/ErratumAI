using UnityEngine;
using StateStuff;

public class TeleportState : State<AI>
{
    #region //Heart of the code, Instantiates this when it is called for.
    private static TeleportState _instance;
    private TeleportState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }
    public static TeleportState Instance
    {
        get
        {
            if (_instance == null)
            {
                new TeleportState();
            }
            return _instance;
        }
    }
    #endregion 

    //Once the state beings start by setting these. (Stop the NPC, change their stat to a hightened state)
    public override void EnterState(AI _owner) 
    {
        _owner.navMeshAgent.isStopped = true;
        
        if (!_owner.particleSystem.isPlaying)
        {
            _owner.particleSystem.Play();
        }
        _owner.teleported = false;
        _owner.timerReached = false;
        _owner.animator.SetBool("IsFleeing", true);
    }

    //Once the state is left set these. (Start animation/ reset values )
    public override void ExitState(AI _owner) 
    {
        _owner.navMeshAgent.isStopped = false;        
        _owner.animator.SetBool("IsFleeing", false);
        if (_owner.particleSystem.isPlaying)
        {
            _owner.particleSystem.Stop();
        }
        _owner.teleported = false;
        _owner.timerReached = false;
        _owner.isFleeing = false;
        //Debug.Log("Exiting Teleport State");
    }

    //Consider this the Update()
    public override void UpdateState(AI _owner)
    {
        _owner.Timer(2f);
        if (_owner.timerReached)
        {
            _owner.TeleportCheck();
        }
        

        if (_owner.teleported)
        {
            _owner.aiState = AiStates.Patrol;
            _owner.stateMachine.ChangeState(PatrolState.Instance);
        }

        /*if (_owner.aiState == AiStates.Flee && _owner.isCornered)
        {
            _owner.TeleportCheck();
                        
        }*/      

    }
}

