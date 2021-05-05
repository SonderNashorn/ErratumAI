using UnityEngine;
using StateStuff;

public class WatchState : State<AI>
{
    /*
           "Stare" at player for a set timer
           if timer is over catch 
           if player out of sight return or catch.
    */

    #region //Heart of the code, Instantiates this when it is called for.
    private static WatchState _instance;
    private WatchState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }
    public static WatchState Instance
    {
        get
        {
            if (_instance == null)
            {
                new WatchState();
            }
            return _instance;
        }
    }
    #endregion 

    //Once the state beings start by setting these. (Stop the NPC, change their stat to a hightened state)
    public override void EnterState(AI _owner) 
    {
        _owner.navMeshAgent.isStopped = true;        
        _owner.navMeshAgent.speed = 0f;
    }

    //Once the state is left set these. (Start animation/ reset values )
    public override void ExitState(AI _owner) 
    {
        _owner.navMeshAgent.isStopped = false;        
        //_owner.navMeshAgent.speed = 1.5f;
    }

    //Consider this the Update()
    public override void UpdateState(AI _owner)
    {
        /*
         * Stand and wait for player for 5 seconds
         * Timer runs out and player player doesn't flash the enemy => Chase
         * AI gets flashed => Teleport
         */

        _owner.Stare();
        _owner.Timer(5f);

        // wait 5 seconds and return to patrol if in vision.                
        if (_owner.timerReached )         
            _owner.ignoreTime = true;                        

        //if the AI waited for set time catch the player
        if (_owner.ignoreTime)
        {
            _owner.aiState = AiStates.Catch;
            _owner.stateMachine.ChangeState(CatchState.Instance);
        }
        
        //1 means flashed and in middle section
        if (_owner.visionState == 1)
        {
            _owner.aiState = AiStates.Teleport;
            _owner.stateMachine.ChangeState(TeleportState.Instance);            
        }

        if (_owner.visionState == 2 || _owner.visionState == 4)
        {
            _owner.aiState = AiStates.Catch;
            _owner.stateMachine.ChangeState(CatchState.Instance);
        }

        //5 means there is a wall between the player and the enemy.
        if (_owner.visionState == 5)
        {
            _owner.aiState = AiStates.Patrol;
            _owner.stateMachine.ChangeState(PatrolState.Instance);
        }
    }
}


//Throwaway

//In the first vision state the enemy would flee. This has been replaced to just teleport instead. immediately. 
//_owner.aiState = AiStates.Flee;
//_owner.stateMachine.ChangeState(FleeState.Instance);

/*if (_owner.inSight == false)
    {
        _owner.aiState = AiStates.Catch;
        _owner.stateMachine.ChangeState(CatchState.Instance);
    }*/

/*if (_owner.inSight == true && _owner.isFlashed)
 {
     _owner.aiState = AiStates.Flee;
     _owner.stateMachine.ChangeState(FleeState.Instance);
 }*/

/*if (_owner.visionState == 1  || _owner.visionState == 3)
    {
        _owner.aiState = AiStates.Catch;
        _owner.stateMachine.ChangeState(CatchState.Instance);
    }*/
