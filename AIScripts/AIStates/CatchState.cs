using UnityEngine;
using StateStuff;
using UnityEngine.Analytics;
using System.Collections.Generic;
using UnityEngine.AI;

public class CatchState : State<AI>
{
    #region //Heart of the code, don't touch
    private static CatchState _instance;
    private CatchState()
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;
    }
    public static CatchState Instance
    {
        get
        {
            if (_instance == null)
            {
                new CatchState();
            }
            return _instance;
        }
    }
    #endregion

    public override void EnterState(AI _owner) 
    {
        //Make sure AI is not frozen
        _owner.navMeshAgent.isStopped = false;
        //_owner.playerIsCaught = false;
        //_owner.navMeshAgent.ResetPath();
        //_owner.navMeshAgent.destination = _owner.lastKnownPlayerPosition;
        _owner.navMeshAgent.speed = 5.5f;
        _owner.ignoreTime = true;
        

    }

    public override void ExitState(AI _owner) 
    {
        
        _owner.ignoreTime = false;
        
    }

    
    public override void UpdateState(AI _owner)
    {
        //Try to reach player.
        
        //_owner.PlayerPositionTrackingthingie();
        //
        //If the ai gets flashed return to first waypoint
        if (_owner.visionState != 1 && _owner.visionState != 5)
        {
            _owner.Stare();
            _owner.CatchPlayer();
        } 
        else if (_owner.visionState == 1)
        {
            _owner.aiState = AiStates.Teleport;
            _owner.stateMachine.ChangeState(TeleportState.Instance);
        }

        else if (_owner.visionState == 5)
        {
            _owner.aiState = AiStates.Patrol;
            _owner.stateMachine.ChangeState(PatrolState.Instance);
        }

        //if the AI gets close to the player end the game
        if (Vector3.Distance(_owner.navMeshAgent.transform.position, _owner.lastKnownPlayerPosition) <= 1.5f)
        {
            _owner.playerIsCaught = true;            
        }

        //End Screen
       if (_owner.playerIsCaught)
        {
            _owner.deathCount++;
            /*_owner.deathAnalytics.SetActive(true);
            _owner.deathAnalytics.SetActive(false);*/
            _owner.deathImage.SetActive(true);            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PauseMenuController.GameIsPaused = true;
            PlayerStats.ps.canMove = false;
            _owner.aiState = AiStates.Patrol;
            _owner.stateMachine.ChangeState(PatrolState.Instance);
        }
    }
}


/*
//if the player starts to look at the enemy and the AI hasn't waited for 5 seconds start watching
if (_owner.visionState == 3 && _owner.ignoreTime == false)
{
    _owner.aiState = AiStates.Watch;
    _owner.stateMachine.ChangeState(WatchState.Instance);            
}//Once the timer runs out even if it is in the periphery start chasing.
else if (_owner.visionState == 3 && _owner.ignoreTime == true)
{
    _owner.CatchPlayer();
}*/
