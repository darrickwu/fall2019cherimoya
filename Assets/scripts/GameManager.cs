using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static bool playerTurn = true;
    public GameObject playerCanvas;
    public GameObject enemyCanvas;

    public GameObject enemy;
    public GameObject player;

    public GameObject aStaar;
    public Grid grid;
    public Pathfinding pathfinder;

    public List<Node> waypoints;
    private Node waypointCurrent;
    private int index = 0;
    private bool moveDone = true;
    private bool performedAction = false;

    private float distanceTraveled;
    private Vector3 lastPosition;
    public float maxDistance;

    // Start is called before the first frame update
    void Start()
    {
        grid = aStaar.GetComponent<Grid>();
        pathfinder = aStaar.GetComponent<Pathfinding>();
        lastPosition = enemy.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        playerCanvas.SetActive(get_turn());
        enemyCanvas.SetActive(!get_turn());
        //Perform AI Action and set player turn again
        if (!get_turn())
        {
            if(!performedAction)
            {
                AI_To_Player();
            }

            if (moveDone)
            {
                Debug.Log("DONE");
                set_turn(true);
                performedAction = false;
            }
            else
            {
                AI_Move();
            }
        }
    }

    public static bool get_turn()
    {
        return playerTurn;
    }

    public void set_turn(bool currentTurn)
    {
        playerTurn = currentTurn;
    }

    //Unused at the moment. Intended to simulate enemy AI
    //performing action.
    public void AI_To_Player()
    {
        Debug.Log("SET DESTINATION");
        moveDone = false;
        index = 0;
        waypointCurrent = null;

        pathfinder.FindPath(enemy.transform.position, player.transform.position);
        waypoints = grid.path;
        if (waypoints != null)
        {
            waypointCurrent = waypoints[0];
        }
        
        performedAction = true;
    }

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
                waypointCurrent = null;
                moveDone = true;
                distanceTraveled = 0.0f;
                return;
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
                    moveDone = true;
                }


            }
        }
    }
}
