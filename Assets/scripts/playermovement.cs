using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovement : MonoBehaviour
{
    public GameManagerr manager;
    public List<Node> waypoints;
    public GameObject aStaar;
    public Grid grid;
    public Pathfinding pathfinder;
    private Node waypointCurrent;
    private int index = 0;
    private bool moveDone = true;
    private bool performedAction = true;
    public bool playerActionDone = true;

    private float distanceTraveled;
    private Vector3 lastPosition;
    public float maxDistance;

    public GameObject UNITS;
    private int currentUnit = 0;
    public GameObject player;
    private AlliedAI alliedAI;
    // Start is called before the first frame update
    public GameObject moveRadius;
    void Start()
    {
        grid = aStaar.GetComponent<Grid>();
        pathfinder = aStaar.GetComponent<Pathfinding>();
        alliedAI = UNITS.GetComponent<AlliedAI>();
        lastPosition = player.transform.position;
        player.transform.GetChild(1).gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {

        if (manager.get_turn() && playerActionDone)
        {
            moveRadius.SetActive(true);
        }

        if (Input.GetButtonDown("next") && moveDone && manager.get_turn() && playerActionDone)
        {
            player.transform.GetChild(0).gameObject.SetActive(false);
            player.transform.GetChild(1).gameObject.SetActive(false);

            if (currentUnit == UNITS.transform.childCount - 1)
            {
                currentUnit = 0;
            }
            else
            {
                currentUnit++;
            }

            player = UNITS.transform.GetChild(currentUnit).gameObject;
            lastPosition = player.transform.position;
            moveRadius = UNITS.transform.GetChild(currentUnit).GetChild(0).gameObject;
            player.transform.GetChild(0).gameObject.SetActive(true);
            player.transform.GetChild(1).gameObject.SetActive(true);
            return;
        }


        if (Input.GetButtonDown("prev") && moveDone && manager.get_turn() && playerActionDone)
        {
            player.transform.GetChild(0).gameObject.SetActive(false);
            player.transform.GetChild(1).gameObject.SetActive(false);

            if (currentUnit == 0)
            {
                currentUnit = UNITS.transform.childCount - 1;
            }
            else
            {
                currentUnit--;
            }

            player = UNITS.transform.GetChild(currentUnit).gameObject;
            lastPosition = player.transform.position;
            moveRadius = UNITS.transform.GetChild(currentUnit).GetChild(0).gameObject;
            player.transform.GetChild(0).gameObject.SetActive(true);
            player.transform.GetChild(1).gameObject.SetActive(true);
            return;
        }

        //move to the clicked location
        if (Input.GetButtonDown("Fire1") && moveDone && manager.get_turn() && playerActionDone)
        {

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                //walkable layer
                //Debug.Log(hit.collider.gameObject.layer);
                if (hit.collider.gameObject.layer == 8)
                {
                    setDestination(hit.point);
                    performedAction = true;
                    playerActionDone = false;
                    moveRadius.SetActive(false);
                    Debug.Log(distanceTraveled);

                    distanceTraveled = 0.0f;
                }

            }

        }

        if (performedAction && moveDone)
        {
            //shoot/ability/etc.
            alliedAI.alliedAction();
            //wait for manager to set turn in alliedai
            performedAction = false;
        }

        //actually move the unit
        if (waypointCurrent != null)
        {
            pathfinder.seeker.transform.position = Vector3.MoveTowards(pathfinder.seeker.transform.position, waypointCurrent.worldPosition, .1f);
            //get to the next waypoint if it is in bounds
            distanceTraveled += Vector3.Distance(player.transform.position, lastPosition);
            lastPosition = player.transform.position;

            if (distanceTraveled >= maxDistance)
            {
                Debug.Log(distanceTraveled);
                index = waypoints.Count;
                distanceTraveled = 0.0f;
            }

            if (pathfinder.seeker.transform.position == waypointCurrent.worldPosition)
            {
                index++;
                if (index < waypoints.Count)
                {
                    //Debug.Log(index);
                    //Debug.Log(waypoints.Count);
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

        //transform.Translate(Vector3.forward * Time.deltaTime);

    }

    public void setDestination(Vector3 point)
    {
        //Debug.Log("set Destination");
        moveDone = false;
        index = 0;
        waypointCurrent = null;

        pathfinder.FindPath(pathfinder.seeker.position, point);
        waypoints = grid.path;
        if (waypoints != null && waypoints.Count > 0)
        {
            //Debug.Log(waypoints.Count);
            waypointCurrent = waypoints[0];
        }
    }
}