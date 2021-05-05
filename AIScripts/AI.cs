using UnityEngine;
using StateStuff;
using System.Collections.Generic;
using UnityEngine.AI;
//AI Concept for Erratum
/*  
    ***** AI STATES: *****

    Player makes waypoints to follow (currently it is preset waypoints instead)

    Patrol   // Follow set waypoints until player found. (Stay close enough to player)

    Watch    // Stare at player for 5 seconds, then chase

    Chase    // Get close to the player
    
    Teleport // If flashed by the players flashlight burst mode teleport away from the player (longestDistanceCheck)

    Flee     // If flashed by the players flashlight return to the first waypoint  //Not used 

    ***** AI Mechanic: *****
    
    The Agent has multiple states and draws it's actions from this the ai script.
    Each agent has a start and end and an update method that they can call when they enter a state or leave one.
    This way only the required functions are getting called on an update cycle.

    The AI is being told by the player when it should swap between different states. This is called "vision cone"

     */

public enum AiStates
{    
    Patrol,    
    Catch,  
    Flee,
    Watch,
    Teleport
}
public class AI : MonoBehaviour
{
    public StateMachine<AI> stateMachine { get; set; }

    #region Variables
    [Header("AI PARAMETERS")]
    public List<GameObject> aiWayPointList;

    //** Public **//    
    //[HideInInspector]
    public float wayPointDistance;

    //public GameObject playerTracker;

    [HideInInspector]
    public float playerWayPointDistance;

    [HideInInspector]
    public NavMeshAgent navMeshAgent;

    [HideInInspector]
    public float aiSpeed = 1.5f;

    [HideInInspector]
    public bool inSight = false;

    [HideInInspector]
    public bool isFleeing = false;

    [HideInInspector]
    public bool isFlashed = false;

    [Header("Currently in state: ")]
    public AiStates aiState;

    [HideInInspector]
    public bool teleported;

    [HideInInspector]
    public bool playerIsCaught;

    [HideInInspector]
    public Vector3 lastKnownPlayerPosition;
    
    [HideInInspector]
    public bool timerReached = false;

    //[HideInInspector]
    public int visionState;

    public Transform playerRespawnPosition;

    [HideInInspector]
    public Vector3 aiAngle;
    
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public GameObject deathImage;
    [HideInInspector]
    public GameObject deathAnalytics;
    [HideInInspector]
    public int deathCount;
    [HideInInspector]
    public new ParticleSystem particleSystem;
    [HideInInspector]
    public bool ignoreTime;

    [Header("AudioSources")]
    public AudioSource walkAnimationSound;
    public AudioSource runAnimationSound;
	public AudioSource runAnimationSong;

    
    private Transform player;
    //** Private **//
    private Transform watchTarget;
    
    
    //[SerializeField]
    private float nextWPDistancefromPlayer;

    //Waypoint    
    //[SerializeField]
    private int nextWaypoint;

    //Timer related
    private float timer;

    //Used to find the Waypoint that is far enough from the player to teleprot to
    private Vector3 longestPosition;

    private float longestDistanceCheck;

    #endregion

    private void Start()
    {
        
        //Ai Nav Mesh        
        navMeshAgent = GetComponent<NavMeshAgent>();
        nextWaypoint = Random.Range(0, aiWayPointList.Count);

        nextWPDistancefromPlayer = 2f;

        //Player initialization
        player = GameObject.Find("Player").GetComponent<Transform>();

        ignoreTime = false;

        //State machine component
        stateMachine = new StateMachine<AI>(this);
        //timerCheck = 0f;
        isFlashed = false;

        particleSystem = GetComponentInChildren<ParticleSystem>();
        animator = GetComponent<Animator>();

        playerIsCaught = false;
    }

    private void Update()                           //Check AI State and run it's update.
    {
        stateMachine.Update(); // this calls the different states update like any regular update
        CurrentAiState();// runs whichever state we are running
        SpeedCheck(); //Check and Change the speed of the enemy 

        if (playerIsCaught)  //Teleport player back to spawn location
        {
            player.position = playerRespawnPosition.position;
            if (player.position == playerRespawnPosition.position)
            {
                playerIsCaught = false;
            }
        }        
    }                                               //Walk through preset Waypoints
    
    private void SpeedCheck()                       //Check and Change the speed of the enemy
    {
        if (navMeshAgent.speed != aiSpeed)
        {
            navMeshAgent.speed = aiSpeed;
        }
    }                                               //Check and Change the speed of the enemy

    public void Patrol()                            //Walk through preset Waypoints
    {
        animator.SetBool("ignoreTime", false);
        aiAngle = Vector3.forward;
        //animator.SetBool("isWalking", true);        
        if(visionState != 2)
        {
            navMeshAgent.destination = aiWayPointList[nextWaypoint].transform.position;
        }
        
        wayPointDistance = Vector3.Distance(gameObject.transform.position, aiWayPointList[nextWaypoint].transform.position);
        playerWayPointDistance = Vector3.Distance(player.transform.position, aiWayPointList[nextWaypoint].transform.position);

        //We have to reduce the distance between the player and the Enemy at all times. 
        //it should pick a waypoint until it's destination is close to the player.
        if (wayPointDistance <= nextWPDistancefromPlayer) //nextWPDistancefromPlayer is the maximum distance it should keep between the player and the next waypoint.
        {            
            ignoreTime = false;
            nextWaypoint = Random.Range(0, aiWayPointList.Count);
            playerWayPointDistance = Vector3.Distance(player.transform.position, aiWayPointList[nextWaypoint].transform.position);

            //temporarily commented out for causing the AI to walk around one waypoint.
            //purpose: keep the enemy walking by close waypoints. 
            while (playerWayPointDistance > 35f)
            {
                nextWaypoint = Random.Range(0, aiWayPointList.Count);
                playerWayPointDistance = Vector3.Distance(player.transform.position, aiWayPointList[nextWaypoint].transform.position);
                //aiWayPointList[nextWaypoint].transform.position;
            }

        }
    }                                               //Walk through preset Waypoints

    public float Timer(float timerCheck)
    {
        timer += Time.deltaTime;
        if (timerCheck <= timer)
        {
            timerReached = true;
            timer = 0f;
            timerCheck = 0f;
            //shouldStare = !shouldStare;         //Only required if we want active staring   
        }
        return timer;
    }         //Counts until the timer is reached. 

    public void CatchPlayer()                       //Charge at the player
    {   //Take players position and move to it.
        animator.SetBool("ignoreTime", true);
        lastKnownPlayerPosition = player.position;        
        if (navMeshAgent.destination != player.position)
        {
            navMeshAgent.destination = lastKnownPlayerPosition;           
        }
    }
    
    public void TeleportCheck()                     //If the player has the AI cornered disappear to the furthest waypoint possible 
    {                                        //play animation of fading out check which waypoint is further than 40 and teleport there.

        //  Take a waypoint that is far enough and move there once the timer runs out in Teleport state
        Vector3 currentPosition = new Vector3(0, 0, 0);
        for (int i = 0; i < aiWayPointList.Count; i++)
        {
            currentPosition = transform.position;
            longestDistanceCheck = Vector3.Distance(aiWayPointList[i].transform.position, currentPosition);

            if (longestDistanceCheck >= 40 && teleported == false)
            {
                longestPosition = aiWayPointList[i].transform.position;
                transform.position = longestPosition;
                teleported = true;
            }
        }
    }

    public void PlayerVision(int value)             //The player telling the ai and animator what to do based on it's position.
    {
        visionState = value;
        //Apply this or something similar to the vision Cone next
        if (animator.GetInteger("VisionState") != value) //if the value doesn't change don't call again 
        {
            animator.SetInteger("VisionState", value);
            PlayAudio(value);
        }
    }  

    public void PlayAudio(int value)
    {
        //create a list of audiosources in the future + for if loop
        walkAnimationSound.Stop(); //stop sounds from playing when not necessary
        runAnimationSound.Stop(); //stop sounds from playing when not necessary
		runAnimationSong.Stop(); //stop sounds from playing when not necessary
        
        if (value == 2 || value == 4) //play running sound
        {
            runAnimationSound.Play();
			runAnimationSong.Play();
            //Debug.Log("Runsound is playing " + runAnimationSound.isPlaying);
        }
        else if (value == 5) //play idle sound (no walk)
        {
            walkAnimationSound.Play();
            //Debug.Log("walksound is playing " + walkAnimationSound.isPlaying);
        }
    }

    public void Flee()                              // Flee till out of sight and reached first waypoint
    {
        //animator.SetBool("isFlashed", true);
        navMeshAgent.destination = aiWayPointList[0].transform.position;
        animator.SetBool("isFleeing", true);
        if (Vector3.Distance(gameObject.transform.position, aiWayPointList[0].transform.position) <= 0.5f && inSight == false)
        {
            nextWaypoint = Random.Range(0, aiWayPointList.Count);
            isFleeing = false;
            animator.SetBool("isFleeing", false);
        }
        else
        {
            nextWaypoint = Random.Range(0, aiWayPointList.Count);
        }
    }

    public void Stare()
    {        
        watchTarget = player.transform;

        Vector3 direction = (watchTarget.position - transform.position).normalized;
        transform.LookAt(watchTarget.position);
    }                          // Follows the "watchTarget" (Player) hit by a RayCast.
     
    public void CurrentAiState()                    // Switch ai behaviour states
    {

        switch (aiState)
        {
            
            case AiStates.Patrol:
                {
                    if (stateMachine.currentState != PatrolState.Instance)
                    {
                        stateMachine.ChangeState(PatrolState.Instance);
                    }
                    break;
                }            
            case AiStates.Catch:
                {
                    if (stateMachine.currentState != CatchState.Instance)
                    {
                        stateMachine.ChangeState(CatchState.Instance);
                    }
                    break;
                }
            case AiStates.Flee:
                {
                    if (stateMachine.currentState != FleeState.Instance)
                    {
                        stateMachine.ChangeState(FleeState.Instance);
                    }
                    break;
                }
            case AiStates.Watch:
                {
                    if (stateMachine.currentState != WatchState.Instance)
                    {
                        stateMachine.ChangeState(WatchState.Instance);
                    }
                    break;
                }
            case AiStates.Teleport:
                {
                    if (stateMachine.currentState != TeleportState.Instance)
                    {
                        stateMachine.ChangeState(TeleportState.Instance);
                    }
                    break;
                }
        }
    }


}

//Throwaway code - This is a section where I move previously existing, 
//but later removed code that I will probably delete, yet may require
/*

//Spotting variables

 /*private float sightRange = 2f;
 private float sightRadius = 2.5f;
 private string[] tags = { "Player", "NPC" };

 private bool playerFound;
 private bool playerThroughGlass;*/


/* public void MapVariant()               //Load the waypoints from the currently active mapset.
{                                      //The code has been reduced by removing the Procedural hallways.
  foreach (GameObject wayPoint in GameObject.FindGameObjectsWithTag("WaypointList0"))
      {                
          aiWayPointList.Add(wayPoint);
      }
}           */                                    //Load the waypoints from the currently active mapset.


   