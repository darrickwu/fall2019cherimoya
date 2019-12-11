﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    //

    //ENEMY UNITS
    public GameObject allEnemyUnits;
    private List<GameObject> enemyUnits;
    private Random rand = new Random();


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


        for(int i = 0; i < allEnemyUnits.transform.childCount; i++)
        {
            enemyUnits.Add(allEnemyUnits.transform.GetChild(i).gameObject);
        }


        for(int i = 0; i < allCover.transform.childCount; i++)
        {
            coverList.Add(allCover.transform.GetChild(i).gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        playerCanvas.SetActive(get_turn());
        enemyCanvas.SetActive(!get_turn());
        //Perform AI Action and set player turn again
        if (!get_turn())
        {

            Decision_Tree();
         
            if (moveDone)
            {
                //Debug.Log("DONE");
                //dont shoot if low health
                //fix this later
                if (enemyStats.health > 10) 
                {
                    //Debug.Log("START SHOOTING");
                    enemy.transform.LookAt(player.transform);
                    ps.playParticle();
                    performedAction = true;
                    StartCoroutine(ExampleCoroutine());

                }
                set_turn(true);
                performedAction = false;
            }
            else
            {
                if(enemy != null)
                {
                    AI_Move();
                }
            }
        }
    }

    public  bool get_turn()
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
            if(waypoints.Count > 0)
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
        RaycastHit hitPlayer;
        if (!performedAction && enemy != null && Physics.Raycast(enemy.transform.position, (player.transform.position - enemy.transform.position), out hitPlayer, Mathf.Infinity))
        {
            enemy = enemyUnits.ToArray()[Random.Range(0, 8)];
            enemyWeapon = enemy.transform.GetChild(0).gameObject;
            ps = enemyWeapon.GetComponent<PlayParticle>();

            if (enemyStats.health <= 10)
            {
                //flee
                AI_To_Dest(temporarySpot);
                
                return;
            }

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

                    if(dist < Vector3.Distance(child1.position, player.transform.position))
                    {
                        closestObj = child1.gameObject;
                    }

                    //Debug.Log("MOVING");
                    //Debug.Log(closestObj.transform);
                    AI_To_Dest(closestObj);
                }
                else
                {
                    //Debug.Log("START SHOOTING");
                    enemy.transform.LookAt(player.transform);
                    ps.playParticle();
                    performedAction = true;
                    StartCoroutine(ExampleCoroutine());
                    //no cover found, start shooting
                }
                //Check for cover
                //if(cover found within distance){
                //move to cover and shoot
                //}
                //else{
                //shoot
                //}
            }
            //i cannot see the player so i move towards players location
            else
            {
                AI_To_Dest(player);
            }

        }

    }

    IEnumerator ExampleCoroutine()
    {
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(2);
        //end turn
        set_turn(true);
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
}
