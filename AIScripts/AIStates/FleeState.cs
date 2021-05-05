using UnityEngine;
using StateStuff;

public class FleeState : State<AI>
{
    /*
        NPC beings focussing on the player for the set "Timer" we want it to.  
        catch player if sells stuff
        return patrol after Timer number is exceeded.
    */

    #region //Heart of the code, Instantiates this when it is called for.
    private static FleeState _instance;
    private FleeState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }
    public static FleeState Instance
    {
        get
        {
            if (_instance == null)
            {
                new FleeState();
            }
            return _instance;
        }
    }
    #endregion 

    //Once the state beings start by setting these. (Stop the NPC, change their stat to a hightened state)
    public override void EnterState(AI _owner)
    {
        _owner.navMeshAgent.isStopped = false;
        _owner.navMeshAgent.speed += 1;        
        _owner.isFleeing = true;
    }

    //Once the state is left set these. (Start animation/ reset values )
    public override void ExitState(AI _owner)
    {
        _owner.navMeshAgent.speed -= 1;
        _owner.isFleeing = false;
        
    }

    //Consider this the Update()
    public override void UpdateState(AI _owner)
    {
        
        _owner.Flee();
        _owner.Timer(3);
        if (_owner.timerReached)
        {
            _owner.aiState = AiStates.Teleport;
            _owner.stateMachine.ChangeState(TeleportState.Instance);
        }

        if (_owner.visionState == 5 && _owner.isFleeing == false)
        {
            _owner.aiState = AiStates.Patrol;
            _owner.stateMachine.ChangeState(PatrolState.Instance);
        }
    }
}



//_owner.Timer(5);
//_owner.Stare();
/* if (_owner.interaction.offeredToSell == true)
 {
     _owner.aiState = AiStates.Catch;
     _owner.stateMachine.ChangeState(CatchState.Instance);
 }
 if (_owner.shouldStare == false)
 {
     _owner.aiState = AiStates.Patrol;
     _owner.stateMachine.ChangeState(PatrolState.Instance);
 





    if (_owner.inSight == false && _owner.visionState == 1)
        {
            _owner.aiState = AiStates.Patrol;
            _owner.stateMachine.ChangeState(PatrolState.Instance);
        }






    */
