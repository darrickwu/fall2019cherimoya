using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerr : MonoBehaviour
{

    public static bool playerTurn = true;
    public GameObject playerCanvas;
    public GameObject enemyCanvas;

    public GameObject enemy;
    public GameObject player;

    private UnitStats enemyStats;
    private UnitStats playerStats;

    public GameObject aStaar;
    public Grid gridClass;
    public Pathfinding pathfinder;

    public List<Node> waypoints;
    private Node waypointCurrent;
    private int index = 0;
    private bool moveDone = true;
    private bool performedAction = false;

    private float distanceTraveled;
    private Vector3 lastPosition;
    public float maxDistance;
    private Node[,] localGrid;
    public GameObject allCover;
    private List<GameObject> coverList;

    private PlayParticle ps;
    public GameObject enemyWeapon;

    //DELETE THIS
    public GameObject temporarySpot;
    private bool waiting = false;
  

    //ENEMY UNITS
    public GameObject allPlayerUnits;
    public GameObject allEnemyUnits;
    public List<GameObject> enemyUnits;
    public List<GameObject> playerUnits;

    private bool gameOver = false;
    private bool didIWin = false;
    private int counter = 0;

    public bool switchAction = false;
    public bool switchChar = false;
    public bool switchAlly = false;
    private bool destroying = false;

    private int stuck = 0;
    // Start is called before the first frame 
    void Start()
    {
        gridClass = aStaar.GetComponent<Grid>();
        pathfinder = aStaar.GetComponent<Pathfinding>();
        lastPosition = enemy.transform.position;
        localGrid = gridClass.grid;

        playerStats = player.GetComponent<UnitStats>();
        enemyStats = enemy.GetComponent<UnitStats>();

        ps = enemyWeapon.GetComponent<PlayParticle>();

        coverList = new List<GameObject>();
        enemyUnits = new List<GameObject>();
        playerUnits = new List<GameObject>();

        for (int i = 0; i < allPlayerUnits.transform.childCount; i++)
        {
            playerUnits.Add(allPlayerUnits.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < allEnemyUnits.transform.childCount; i++)
        {
            enemyUnits.Add(allEnemyUnits.transform.GetChild(i).gameObject);
        }


        for (int i = 0; i < allCover.transform.childCount; i++)
        {
            coverList.Add(allCover.transform.GetChild(i).gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (gameOver)
        {
            counter++;
            if (counter >= 240)
            {
                endGame(didIWin);
            }
            else
            {
                return;
            }
        }

        playerCanvas.SetActive(get_turn());
        enemyCanvas.SetActive(!get_turn());
        //Perform AI Action and set player turn again
        if (!get_turn() && !waiting)
        {
            if (!performedAction)
            {
                Decision_Tree();

            }

            if (moveDone)
            {

                RaycastHit hitPlayer;
                if (enemy != null && Physics.Raycast(enemy.transform.position, (player.transform.position - enemy.transform.position), out hitPlayer, Mathf.Infinity))
                {
                    //i can see the player
                    if (hitPlayer.collider.gameObject.layer == 11)
                    {
                        //Debug.Log("START SHOOTING");
                        enemy.transform.LookAt(player.transform);
                        ps.playParticle();
                        float hitChance = enemyStats.accuracy - enemyStats.evasion;
                        bool RNGSuccess = Random.Range(0.0f, 101.0f) <= hitChance;
                        if (RNGSuccess)
                        {
                            playerStats.takeDamage(enemyStats.weaponDamage);
                        }
                    }

                }
                performedAction = false;
                waiting = true;
                StartCoroutine(ExampleCoroutine());

            }
            else
            {
                if (enemy != null)
                {
                    AI_Move();
                    if (enemy.transform.position == lastPosition)
                    {
                        stuck++;
                        if (stuck >= 240)
                        {
                            moveDone = true;
                            StartCoroutine(ExampleCoroutine());
                        }
                    }
                }
            }
        }
    }

    public bool get_turn()
    {
        return playerTurn;
    }

    public void set_turn(bool currentTurn)
    {
        playerTurn = currentTurn;
    }

    //Unused at the moment. Intended to simulate enemy AI
    //performing action.
    public void AI_To_Dest(GameObject dest)
    {
        //Debug.Log("SET DESTINATION");
        moveDone = false;
        index = 0;
        waypointCurrent = null;

        pathfinder.FindPath(enemy.transform.position, dest.transform.position);
        waypoints = gridClass.path;
        if (waypoints != null)
        {
            if (waypoints.Count > 0)
                waypointCurrent = waypoints[0];
        }

        performedAction = true;
    }

    //actually move the ai to the player waypoint by waypoint
    public void AI_Move()
    {
        //Debug.Log("MOVE");

        if (waypointCurrent != null)
        {
            //Debug.Log("waypoint not null");
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, waypointCurrent.worldPosition, .1f);

            distanceTraveled += Vector3.Distance(enemy.transform.position, lastPosition);
            lastPosition = enemy.transform.position;


            if (distanceTraveled >= maxDistance)
            {
                index = waypoints.Count;
                distanceTraveled = 0.0f;
            }

            //get to the next waypoint if it is in bounds
            if (enemy.transform.position == waypointCurrent.worldPosition)
            {
                index++;
                if (index < waypoints.Count)
                {
                    waypointCurrent = waypoints[index];
                }
                else
                {
                    waypointCurrent = null;
                    moveDone = true;
                    distanceTraveled = 0.0f;
                }


            }
        }
        else
        {
            waypointCurrent = null;
            moveDone = true;
            distanceTraveled = 0.0f;
        }
    }

    public void Decision_Tree()
    {
        //check if player is visible
        if (!performedAction)
        {
            enemy = enemyUnits[Random.Range(0, enemyUnits.Count)];
            enemyStats = enemy.GetComponent<UnitStats>();
            player = playerUnits[Random.Range(0, playerUnits.Count)];
            playerStats = player.GetComponent<UnitStats>();
            lastPosition = enemy.transform.position;
            enemyWeapon = enemy.transform.GetChild(0).gameObject;
            ps = enemyWeapon.GetComponent<PlayParticle>();

        }

        RaycastHit hitPlayer;
        if (!performedAction && enemy != null && Physics.Raycast(enemy.transform.position, (player.transform.position - enemy.transform.position), out hitPlayer, Mathf.Infinity))
        {
            //i can see the player
            if (hitPlayer.collider.gameObject.layer == 11)
            {
                //Debug.Log("SEE PLAYER");               
                float maxDistCover = maxDistance * .75f;
                //loop through pieces of cover to find min
                float minDist = Mathf.Infinity;
                GameObject closestObj = null;
                foreach (GameObject child in coverList)
                {
                    //Debug.Log("FINDING COVER");
                    float dist = Vector3.Distance(child.transform.position, enemy.transform.position);
                    if (dist < minDist)
                    {
                        closestObj = child;
                        minDist = dist;
                    }
                }
                //Debug.Log(closestObj.transform);
                //the cover must be close enough
                if (closestObj != null && minDist <= maxDistance)
                {
                    //Debug.Log("FINDING HOTSPOT");
                    //pass in the "ideal" cover hotspot(furthest from the player)
                    Transform child0 = closestObj.transform.GetChild(0);
                    Transform child1 = closestObj.transform.GetChild(1);

                    float dist = Vector3.Distance(child0.position, player.transform.position);

                    closestObj = child0.gameObject;

                    if (dist < Vector3.Distance(child1.position, player.transform.position))
                    {
                        closestObj = child1.gameObject;
                    }

                    //Debug.Log("MOVING");
                    //Debug.Log(closestObj.transform);

                    foreach(GameObject obj in enemyUnits)
                    {
                        if(obj.transform.position.x - closestObj.transform.position.x < 2.0f && obj.transform.position.z - closestObj.transform.position.z < 2.0f)
                        {
                            closestObj = player;
                            break;
                        }
                    }

                    AI_To_Dest(closestObj);
                }
                else
                {
                    //Debug.Log("START SHOOTING");
                    enemy.transform.LookAt(player.transform);
                    ps.playParticle();
                    float hitChance = enemyStats.accuracy - enemyStats.evasion;
                    bool RNGSuccess = Random.Range(0.0f, 101.0f) <= hitChance;
                    if (RNGSuccess)
                    {
                        playerStats.takeDamage(enemyStats.weaponDamage);
                    }
                    performedAction = true;
                    waiting = true;
                    StartCoroutine(ExampleCoroutine());
                    //no cover found, start shooting
                }

            }
            //i cannot see the player so i move towards players location
            else
            {
                AI_To_Dest(player);
            }

        }

    }

    public void checkForWin()
    {
        if (enemyUnits.Count == 0)
        {
            gameOver = true;
            didIWin = true;
        }
        else if (playerUnits.Count == 0)
        {
            gameOver = true;
            didIWin = false;
        }
    }

    private void endGame(bool playerWin)
    {
        if (playerWin)
        {
            SceneManager.LoadScene(sceneName: "Win");
        }
        else
        {
            SceneManager.LoadScene(sceneName: "Lose");
        }
    }

    public void switchStats()
    {
        destroying = true;
        if (pathfinder.seeker.gameObject == player && playerUnits.Count > 1)
        {
            switchAction = true;
            switchChar = true;
            switchAlly = true;
        }
        else
        {
            switchDestroy();
        }
    }

    public void switchDestroy()
    {
        if (!switchAction && !switchAlly && !switchChar && destroying)
        {
            playerUnits.Remove(player);
            Destroy(player);
            checkForWin();
            destroying = false;
        }
    }

    IEnumerator ExampleCoroutine()
    {
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(2);
        //end turn
        waiting = false;
        stuck = 0;
        set_turn(true);
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
}
