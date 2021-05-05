using UnityEngine;


/*
 
    If the enemy is in the perifery tell it to stare
 
    If out of perifery              tell it to chase
 
    If in perifery                  tell it to flee    
          
 */

public class VisionCone : MonoBehaviour
{
    public GameObject _enemy;
    public GameObject torch;
    private AI enemyAI;
    public Vector3 directionToEnemy;
    //[HideInInspector]
    public float enemyAngle;
    public float distanceToEnemy;

    [Range(0, 360)] 
    public float spotAngle = 90;
    public float flashAngle = 30;
    public float chaseAngle;
    public float listenRadius = 10f;
    [SerializeField]
    public LayerMask obstacleMask;
    
    [SerializeField]
    private bool flashLightActive;


    //private bool playerInHearingRange;
    private void Awake()
    {
        chaseAngle = spotAngle + 30;                
        
    }
    private void Start()
    {
        enemyAI = _enemy.GetComponent<AI>(); 
    }
    private void Update()
    {
        FOV();        
    }
    /*
     Check if the player is in the visible range and is not behind an obstacle 
     
     Calculate the angle from  where the player is facing. Compare the enemy position to it.
     If the enemy is out of visual tell the AI it can chase, 
      -If the Ai is slightly in vision (on perifery) slowly approach)
        -If the player begins to turn towards the enemy, DASH! 
    */

    void FOV() 
    {
        //Vector direction from enemy to player
        directionToEnemy = (_enemy.transform.position - transform.position).normalized;
        //_enemy.SendMessage("PlayerInfo", directionToEnemy);

        //Vector Angle from player to enemy
        enemyAngle = Vector3.Angle(transform.forward, directionToEnemy);

        //distance from player to enemy
        distanceToEnemy = Vector3.Distance(transform.position, _enemy.transform.position);

        if (!Physics.Raycast(transform.position, directionToEnemy, distanceToEnemy, obstacleMask))
        {
            ConeCheck();                 
        }
        else if(enemyAI.visionState != 5)            
        {
            _enemy.SendMessage("PlayerVision", 5);
            
        }

    }

  

    void ConeCheck()            
    {
        if (enemyAngle >= chaseAngle && distanceToEnemy > 12)                        // Enemy angle larger than 90  Enemy should CHASE      
        {
            if (enemyAI.visionState != 4)
            {
                _enemy.SendMessage("PlayerVision", 4);      //Watch first and then chase                
                
            }              
        }

        if (enemyAngle > spotAngle / 2 && enemyAngle < chaseAngle && distanceToEnemy > 12)  //Enemy angle between 45 and 90   Peek/ Approach  //&& distanceToEnemy > 3
        {
            if (enemyAI.visionState != 3)
            {
                _enemy.SendMessage("PlayerVision", 3);      //watch               
               
            }
        }

        else if (enemyAngle < spotAngle / 2 && distanceToEnemy < 6)                //Enemy angle less than 45    CHARGE
        {
            if (enemyAI.visionState != 2)
            {
                _enemy.SendMessage("PlayerVision", 2);      //CHARGE
               
            }
        }

        if (torch.activeSelf == true && enemyAngle < spotAngle / 3)  // Enemy angle less than 22.5  in flashing angle      
        {
            {
                if (enemyAI.visionState != 1)
                {
                    _enemy.SendMessage("PlayerVision", 1);          //teleport
                   
                }
            }
        }
    } 
    
    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}


//throw away
/*if (Input.GetKeyDown(KeyCode.Mouse0))
        torch.SetActive(true);
    else
        torch.SetActive(false);
        */
